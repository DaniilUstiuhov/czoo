
using System;

namespace AnimalManager
{
    // Abstraktne baasklass
    public abstract class Animal
    {
        public string Name { get; set; }
        public int Age { get; set; }

        protected Animal(string name, int age)
        {
            Name = name;
            Age = age;
        }

        // Virtuaalne meetod - saab üle kirjutada
        public virtual string Describe()
        {
            return $"{Name} on {Age} aastat vana";
        }

        // Abstraktne meetod - peab realiseerima
        public abstract string MakeSound();

        public override string ToString()
        {
            return $"{Name} ({this.GetType().Name})";
        }
    }

    // Liides 1: Lendamine
    public interface IFlyable
    {
        void Fly();
        bool IsFlying { get; set; }
    }

    // Liides 2: Hullumeelne tegevus
    public interface ICrazyAction
    {
        string ActCrazy();
    }

    // Klass 1: Kass
    public class Cat : Animal, ICrazyAction
    {
        public string FavoriteFood { get; set; }

        public Cat(string name, int age, string favoriteFood = "kala")
            : base(name, age)
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
    }

    // Klass 2: Koer
    public class Dog : Animal, ICrazyAction
    {
        public string Breed { get; set; }

        public Dog(string name, int age, string breed = "Segaverelne")
            : base(name, age)
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
    }

    // Klass 3: Lind
    public class Bird : Animal, IFlyable, ICrazyAction
    {
        public bool IsFlying { get; set; }
        public string Color { get; set; }

        public Bird(string name, int age, string color = "sinine")
            : base(name, age)
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
    }

    // Klass 4: Pesukaru
    public class Raccoon : Animal, ICrazyAction
    {
        public int ThingsStolen { get; set; }

        public Raccoon(string name, int age)
            : base(name, age)
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
    }

    // Klass 5: Ahv
    public class Monkey : Animal, ICrazyAction
    {
        public bool IsMischievous { get; set; }

        public Monkey(string name, int age)
            : base(name, age)
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
    }
}