
using System;

namespace AnimalManager
{
    // ============= EVENT ARGS =============
    public class AnimalEventArgs : EventArgs
    {
        public Animal Animal { get; set; }
        public string EnclosureName { get; set; }
    }

    public class FoodEventArgs : EventArgs
    {
        public string FoodType { get; set; }
        public string EnclosureName { get; set; }
    }

    // ============= БАЗОВЫЙ КЛАСС (БЕЗ ИЗМЕНЕНИЙ) =============
    public abstract class Animal
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string EnclosureId { get; set; }  // NEW: для вольеров
        public double EatingSpeed { get; set; }  // NEW: скорость поедания (секунды)

        protected Animal(string name, int age, double eatingSpeed = 2.0)
        {
            Name = name;
            Age = age;
            EatingSpeed = eatingSpeed;
        }

        public virtual string Describe()
        {
            return $"{Name} on {Age} aastat vana";
        }

        public abstract string MakeSound();

        // NEW: Реакция на нового соседа
        public virtual string ReactToNewNeighbor(Animal newAnimal)
        {
            return $"{Name}: Oh, uus naaber {newAnimal.Name}!";
        }

        // NEW: Реакция на еду
        public virtual string ReactToFood(string foodType)
        {
            return $"{Name} hakkab sööma {foodType}...";
        }

        public override string ToString()
        {
            return $"{Name} ({this.GetType().Name})";
        }
    }

    // ============= ИНТЕРФЕЙСЫ (БЕЗ ИЗМЕНЕНИЙ) =============
    public interface IFlyable
    {
        void Fly();
        bool IsFlying { get; set; }
    }

    public interface ICrazyAction
    {
        string ActCrazy();
    }

    // ============= КЛАССЫ ЖИВОТНЫХ (УЛУЧШЕННЫЕ) =============

    public class Cat : Animal, ICrazyAction
    {
        public string FavoriteFood { get; set; }

        public Cat(string name, int age, string favoriteFood = "kala")
            : base(name, age, 1.5)  // Быстро ест
        {
            FavoriteFood = favoriteFood;
        }

        public override string MakeSound()
        {
            return "Mjäu! Mjäu!";
        }

        public override string Describe()
        {
            return base.Describe() + $" ja armastab {FavoriteFood}";
        }

        public string ActCrazy()
        {
            string[] foods = { "juustu", "vorsti", "kala", "piima", "kanafilee" };
            string stolen = foods[new Random().Next(foods.Length)];
            return $"{Name} varastas köögist {stolen}!";
        }

        public override string ReactToNewNeighbor(Animal newAnimal)
        {
            if (newAnimal is Dog)
                return $"{Name}: Pah, koer! *sosistab*";
            return base.ReactToNewNeighbor(newAnimal);
        }

        public override string ReactToFood(string foodType)
        {
            return $"{Name} nuusutab ettevaatlikult {foodType}...";
        }
    }

    public class Dog : Animal, ICrazyAction
    {
        public string Breed { get; set; }

        public Dog(string name, int age, string breed = "Segaverelne")
            : base(name, age, 1.0)  // Очень быстро ест
        {
            Breed = breed;
        }

        public override string MakeSound()
        {
            return "Auh! Auh!";
        }

        public override string Describe()
        {
            return base.Describe() + $", tõug: {Breed}";
        }

        public string ActCrazy()
        {
            return $"{Name} haugub hullult: WOOF! WOOF! WOOF! WOOF! WOOF!";
        }

        public override string ReactToNewNeighbor(Animal newAnimal)
        {
            return $"{Name}: *lehvitab sabaga* Tere, {newAnimal.Name}!";
        }

        public override string ReactToFood(string foodType)
        {
            return $"{Name} hüppab rõõmsalt {foodType} kallale!";
        }
    }

    public class Bird : Animal, IFlyable, ICrazyAction
    {
        public bool IsFlying { get; set; }
        public string Color { get; set; }

        public Bird(string name, int age, string color = "sinine")
            : base(name, age, 0.5)  // Очень быстро ест
        {
            Color = color;
            IsFlying = false;
        }

        public override string MakeSound()
        {
            return "Tširp! Tširp!";
        }

        public override string Describe()
        {
            string status = IsFlying ? "lendab" : "istub oksake peal";
            return base.Describe() + $", värv: {Color}, praegu {status}";
        }

        public void Fly()
        {
            IsFlying = !IsFlying;
        }

        public string ActCrazy()
        {
            Fly();
            string action = IsFlying ? "hakkas hullult lendama" : "kukkus järsku maha";
            return $"{Name} {action} ja kriiskab: CHIRP!!! CHIRP!!! CHIRP!!!";
        }

        public override string ReactToNewNeighbor(Animal newAnimal)
        {
            return $"{Name}: *tsirutab rõõmsalt* Uus sõber!";
        }

        public override string ReactToFood(string foodType)
        {
            return $"{Name} nokib kiiresti {foodType}!";
        }
    }

    public class Raccoon : Animal, ICrazyAction
    {
        public int ThingsStolen { get; set; }

        public Raccoon(string name, int age)
            : base(name, age, 2.5)  // Медленно ест
        {
            ThingsStolen = 0;
        }

        public override string MakeSound()
        {
            return "Trrrr! Khhhh!";
        }

        public override string Describe()
        {
            return base.Describe() + $", on varastanud {ThingsStolen} asja";
        }

        public string ActCrazy()
        {
            string[] items = {
                "sädeleva vidina",
                "läikiva nööbi",
                "kuldse mündi",
                "peegli",
                "hõbedase lusikat",
                "kristallklaasi"
            };
            string item = items[new Random().Next(items.Length)];
            ThingsStolen++;
            return $"{Name} leidis {item} ja peitis oma salajasse kohta!";
        }

        public override string ReactToNewNeighbor(Animal newAnimal)
        {
            return $"{Name}: *uurib uue naabri tasku*";
        }

        public override string ReactToFood(string foodType)
        {
            return $"{Name} peseb {foodType} vees enne söömist...";
        }
    }

    public class Monkey : Animal, ICrazyAction
    {
        public bool IsMischievous { get; set; }

        public Monkey(string name, int age)
            : base(name, age, 1.2)
        {
            IsMischievous = true;
        }

        public override string MakeSound()
        {
            return "Uh-uh-ah-ah-ah!";
        }

        public override string Describe()
        {
            string mood = IsMischievous ? "vägagi pahanäoline" : "rahulik";
            return base.Describe() + $", on {mood}";
        }

        public string ActCrazy()
        {
            return $"{Name} hüppab ringi nagu hull ja viskab banaane!";
        }

        public override string ReactToNewNeighbor(Animal newAnimal)
        {
            return $"{Name}: *matkib uut naabrit ja teeb grimasse*";
        }

        public override string ReactToFood(string foodType)
        {
            return $"{Name} haarab {foodType} ja põgeneb puule!";
        }
    }
}