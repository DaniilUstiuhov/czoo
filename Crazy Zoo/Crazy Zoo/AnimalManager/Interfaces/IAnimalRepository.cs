using System;
using System.Collections.Generic;

namespace AnimalManager.Interfaces
{
    /// <summary>
    /// Repository interface for Animal storage
    /// Can be implemented with different storage backends (LocalDB, SQL Server, etc.)
    /// </summary>
    public interface IAnimalRepository
    {
        // Animal operations
        void AddAnimal(Animal animal);
        void RemoveAnimal(int id);
        void UpdateAnimal(Animal animal);
        Animal GetAnimalById(int id);
        IEnumerable<Animal> GetAllAnimals();
        
        // Enclosure operations
        void AddEnclosure(string name, int capacity);
        void RemoveEnclosure(int id);
        IEnumerable<(int Id, string Name, int Capacity)> GetAllEnclosures();
        
        // Animal-Enclosure relationship
        void AssignAnimalToEnclosure(int animalId, int enclosureId);
        void RemoveAnimalFromEnclosure(int animalId);
        IEnumerable<Animal> GetAnimalsByEnclosure(int enclosureId);
        
        // Utility
        void ClearAll();
        int GetNextAnimalId();
    }
}
