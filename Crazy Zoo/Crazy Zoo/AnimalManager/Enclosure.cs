using System;
using System.Collections.Generic;
using System.Linq;

namespace AnimalManager
{
    /// <summary>
    /// Generic Enclosure (Voljeer) для животных
    /// T должен быть Animal или его наследником
    /// </summary>
    public class Enclosure<T> where T : Animal
    {
        public string Name { get; set; }
        public int Capacity { get; set; }
        private List<T> _animals = new List<T>();

        // СОБЫТИЯ
        public event EventHandler<AnimalEventArgs> AnimalJoinedInSameEnclosure;
        public event EventHandler<FoodEventArgs> FoodDropped;

        public Enclosure(string name, int capacity = 10)
        {
            Name = name;
            Capacity = capacity;
        }

        public IEnumerable<T> Animals => _animals.AsReadOnly();
        public int Count => _animals.Count;
        public bool IsFull => _animals.Count >= Capacity;

        /// <summary>
        /// Добавить животное - вызывает событие AnimalJoinedInSameEnclosure
        /// </summary>
        public bool AddAnimal(T animal)
        {
            if (IsFull)
                return false;

            _animals.Add(animal);
            animal.EnclosureId = Name;

            // Вызываем событие
            OnAnimalJoined(new AnimalEventArgs
            {
                Animal = animal,
                EnclosureName = Name
            });

            return true;
        }

        public bool RemoveAnimal(T animal)
        {
            animal.EnclosureId = null;
            return _animals.Remove(animal);
        }

        /// <summary>
        /// Бросить еду - вызывает событие FoodDropped
        /// </summary>
        public void DropFood(string foodType)
        {
            OnFoodDropped(new FoodEventArgs
            {
                FoodType = foodType,
                EnclosureName = Name
            });
        }

        protected virtual void OnAnimalJoined(AnimalEventArgs e)
        {
            AnimalJoinedInSameEnclosure?.Invoke(this, e);
        }

        protected virtual void OnFoodDropped(FoodEventArgs e)
        {
            FoodDropped?.Invoke(this, e);
        }

        public override string ToString()
        {
            return $"{Name} ({Count}/{Capacity})";
        }
    }
}