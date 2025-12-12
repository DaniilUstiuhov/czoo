using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using AnimalManager.Interfaces;
using Microsoft.Data.SqlClient; // FIXED: Using modern SqlClient

namespace AnimalManager.Data
{
    /// <summary>
    /// LocalDB implementation using ADO.NET
    /// Stores animals and enclosures in SQL Server LocalDB
    /// FIXED: Now properly handles Animal.Id property
    /// FIXED: UpdateAnimal is now implemented
    /// </summary>
    public class LocalDbAnimalRepository : IAnimalRepository
    {
        private readonly string _connectionString;

        public LocalDbAnimalRepository()
        {
            // LocalDB connection string
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "AnimalManager",
                "AnimalManager.mdf"
            );

            // Ensure directory exists
            var dbDir = Path.GetDirectoryName(dbPath);
            if (!Directory.Exists(dbDir))
                Directory.CreateDirectory(dbDir);

            _connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;
                                   AttachDbFilename={dbPath};
                                   Integrated Security=True;
                                   Connect Timeout=30";

            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Create Enclosures table FIRST (referenced by Animals)
                    string createEnclosuresTable = @"
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Enclosures' AND xtype='U')
                        CREATE TABLE Enclosures (
                            Id INT PRIMARY KEY IDENTITY(1,1),
                            Name NVARCHAR(100) NOT NULL UNIQUE,
                            Capacity INT NOT NULL
                        )";

                    // Create Animals table
                    string createAnimalsTable = @"
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Animals' AND xtype='U')
                        CREATE TABLE Animals (
                            Id INT PRIMARY KEY IDENTITY(1,1),
                            Name NVARCHAR(100) NOT NULL,
                            Age INT NOT NULL,
                            Type NVARCHAR(50) NOT NULL,
                            ExtraInfo NVARCHAR(200),
                            EatingSpeed FLOAT NOT NULL,
                            EnclosureId INT NULL,
                            FOREIGN KEY (EnclosureId) REFERENCES Enclosures(Id) ON DELETE SET NULL
                        )";

                    using (var cmd = new SqlCommand(createEnclosuresTable, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = new SqlCommand(createAnimalsTable, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to initialize database: {ex.Message}", ex);
            }
        }

        // ========== ANIMAL OPERATIONS ==========

        public void AddAnimal(Animal animal)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = @"INSERT INTO Animals (Name, Age, Type, ExtraInfo, EatingSpeed, EnclosureId)
                                   VALUES (@Name, @Age, @Type, @ExtraInfo, @EatingSpeed, @EnclosureId);
                                   SELECT CAST(SCOPE_IDENTITY() AS INT)";

                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Name", animal.Name);
                        cmd.Parameters.AddWithValue("@Age", animal.Age);
                        cmd.Parameters.AddWithValue("@Type", animal.GetType().Name);
                        cmd.Parameters.AddWithValue("@ExtraInfo", GetExtraInfo(animal) ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@EatingSpeed", animal.EatingSpeed);
                        cmd.Parameters.AddWithValue("@EnclosureId",
                            string.IsNullOrEmpty(animal.EnclosureId) ? (object)DBNull.Value : GetEnclosureIdByName(animal.EnclosureId));

                        // FIXED: Now properly sets the animal's ID
                        animal.Id = (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to add animal: {ex.Message}", ex);
            }
        }

        public void RemoveAnimal(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = "DELETE FROM Animals WHERE Id = @Id";

                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to remove animal: {ex.Message}", ex);
            }
        }

        // FIXED: UpdateAnimal is now fully implemented
        public void UpdateAnimal(Animal animal)
        {
            if (animal == null)
                throw new ArgumentNullException(nameof(animal));

            if (animal.Id <= 0)
                throw new ArgumentException("Animal must have a valid Id", nameof(animal));

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = @"UPDATE Animals 
                                   SET Name = @Name, 
                                       Age = @Age, 
                                       Type = @Type, 
                                       ExtraInfo = @ExtraInfo, 
                                       EatingSpeed = @EatingSpeed,
                                       EnclosureId = @EnclosureId
                                   WHERE Id = @Id";

                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", animal.Id);
                        cmd.Parameters.AddWithValue("@Name", animal.Name);
                        cmd.Parameters.AddWithValue("@Age", animal.Age);
                        cmd.Parameters.AddWithValue("@Type", animal.GetType().Name);
                        cmd.Parameters.AddWithValue("@ExtraInfo", GetExtraInfo(animal) ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@EatingSpeed", animal.EatingSpeed);
                        cmd.Parameters.AddWithValue("@EnclosureId",
                            string.IsNullOrEmpty(animal.EnclosureId) ? (object)DBNull.Value : GetEnclosureIdByName(animal.EnclosureId));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                            throw new InvalidOperationException($"Animal with Id {animal.Id} not found in database");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update animal: {ex.Message}", ex);
            }
        }

        public Animal GetAnimalById(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM Animals WHERE Id = @Id";

                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return CreateAnimalFromReader(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get animal: {ex.Message}", ex);
            }

            return null;
        }

        public IEnumerable<Animal> GetAllAnimals()
        {
            var animals = new List<Animal>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM Animals";

                    using (var cmd = new SqlCommand(sql, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            animals.Add(CreateAnimalFromReader(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get animals: {ex.Message}", ex);
            }

            return animals;
        }

        // ========== ENCLOSURE OPERATIONS ==========

        public void AddEnclosure(string name, int capacity)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Enclosure name cannot be empty", nameof(name));

            if (capacity <= 0)
                throw new ArgumentException("Capacity must be greater than 0", nameof(capacity));

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = "INSERT INTO Enclosures (Name, Capacity) VALUES (@Name, @Capacity)";

                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Capacity", capacity);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 2627) // Unique constraint violation
            {
                throw new InvalidOperationException($"Enclosure with name '{name}' already exists", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to add enclosure: {ex.Message}", ex);
            }
        }

        public void RemoveEnclosure(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // First, check if there are animals in this enclosure
                    string checkSql = "SELECT COUNT(*) FROM Animals WHERE EnclosureId = @Id";
                    using (var checkCmd = new SqlCommand(checkSql, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@Id", id);
                        int animalCount = (int)checkCmd.ExecuteScalar();

                        if (animalCount > 0)
                        {
                            throw new InvalidOperationException(
                                $"Cannot remove enclosure: {animalCount} animal(s) still assigned to it. Remove animals first.");
                        }
                    }

                    // Now safe to delete
                    string sql = "DELETE FROM Enclosures WHERE Id = @Id";
                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to remove enclosure: {ex.Message}", ex);
            }
        }

        public IEnumerable<(int Id, string Name, int Capacity)> GetAllEnclosures()
        {
            var enclosures = new List<(int, string, int)>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = "SELECT Id, Name, Capacity FROM Enclosures";

                    using (var cmd = new SqlCommand(sql, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            enclosures.Add((
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetInt32(2)
                            ));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get enclosures: {ex.Message}", ex);
            }

            return enclosures;
        }

        // ========== ANIMAL-ENCLOSURE RELATIONSHIP ==========

        public void AssignAnimalToEnclosure(int animalId, int enclosureId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = "UPDATE Animals SET EnclosureId = @EnclosureId WHERE Id = @AnimalId";

                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@EnclosureId", enclosureId);
                        cmd.Parameters.AddWithValue("@AnimalId", animalId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to assign animal to enclosure: {ex.Message}", ex);
            }
        }

        public void RemoveAnimalFromEnclosure(int animalId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = "UPDATE Animals SET EnclosureId = NULL WHERE Id = @AnimalId";

                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@AnimalId", animalId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to remove animal from enclosure: {ex.Message}", ex);
            }
        }

        public IEnumerable<Animal> GetAnimalsByEnclosure(int enclosureId)
        {
            var animals = new List<Animal>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM Animals WHERE EnclosureId = @EnclosureId";

                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@EnclosureId", enclosureId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                animals.Add(CreateAnimalFromReader(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get animals by enclosure: {ex.Message}", ex);
            }

            return animals;
        }

        // ========== UTILITY ==========

        public void ClearAll()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Delete in correct order (Animals first, then Enclosures)
                    using (var cmd = new SqlCommand("DELETE FROM Animals; DELETE FROM Enclosures", connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to clear database: {ex.Message}", ex);
            }
        }

        public int GetNextAnimalId()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = "SELECT ISNULL(MAX(Id), 0) + 1 FROM Animals";

                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch
            {
                return 1;
            }
        }

        // ========== HELPER METHODS ==========

        private Animal CreateAnimalFromReader(IDataReader reader)
        {
            int id = reader.GetInt32(reader.GetOrdinal("Id")); // FIXED: Now reads ID
            string name = reader.GetString(reader.GetOrdinal("Name"));
            int age = reader.GetInt32(reader.GetOrdinal("Age"));
            string type = reader.GetString(reader.GetOrdinal("Type"));
            string extraInfo = reader.IsDBNull(reader.GetOrdinal("ExtraInfo"))
                ? null
                : reader.GetString(reader.GetOrdinal("ExtraInfo"));
            double eatingSpeed = reader.GetDouble(reader.GetOrdinal("EatingSpeed"));

            Animal animal = type switch
            {
                "Cat" => new Cat(name, age, extraInfo ?? "kala"),
                "Dog" => new Dog(name, age, extraInfo ?? "Segaverelne"),
                "Bird" => new Bird(name, age, extraInfo ?? "sinine"),
                "Raccoon" => new Raccoon(name, age),
                "Monkey" => new Monkey(name, age),
                _ => throw new Exception($"Unknown animal type: {type}")
            };

            // FIXED: Now properly sets the ID from database
            animal.Id = id;

            // Set enclosure if exists
            if (!reader.IsDBNull(reader.GetOrdinal("EnclosureId")))
            {
                int enclosureId = reader.GetInt32(reader.GetOrdinal("EnclosureId"));
                animal.EnclosureId = GetEnclosureNameById(enclosureId);
            }

            return animal;
        }

        private string GetExtraInfo(Animal animal)
        {
            return animal switch
            {
                Cat cat => cat.FavoriteFood,
                Dog dog => dog.Breed,
                Bird bird => bird.Color,
                _ => null
            };
        }

        private int? GetEnclosureIdByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = "SELECT Id FROM Enclosures WHERE Name = @Name";

                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Name", name);
                        var result = cmd.ExecuteScalar();
                        return result != null ? (int)result : (int?)null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        private string GetEnclosureNameById(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = "SELECT Name FROM Enclosures WHERE Id = @Id";

                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        return cmd.ExecuteScalar()?.ToString();
                    }
                }
            }
            catch
            {
                return null;
            }
        }
    }
}