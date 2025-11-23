using System;
using System.Collections.Generic;
using System.Linq;

namespace AnimalManager.Repositories
{
    /// <summary>
    /// Generic Repository interface
    /// </summary>
    public interface IRepository<T>
    {
        void Add(T item);
        void Remove(T item);
        IEnumerable<T> GetAll();
        T Find(Func<T, bool> predicate);  // LINQ support
        int Count();
    }

    /// <summary>
    /// In-Memory implementation
    /// </summary>
    public class InMemoryRepository<T> : IRepository<T>
    {
        private readonly List<T> _items = new List<T>();

        public void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            _items.Add(item);
        }

        public void Remove(T item)
        {
            _items.Remove(item);
        }

        public IEnumerable<T> GetAll()
        {
            return _items.ToList();
        }

        public T Find(Func<T, bool> predicate)
        {
            return _items.FirstOrDefault(predicate);
        }

        public int Count()
        {
            return _items.Count;
        }

        public void Clear()
        {
            _items.Clear();
        }
    }
}