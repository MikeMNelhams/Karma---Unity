using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataStructures 
{
    public class MultiSet<T> : IEnumerable<T>
    {
        protected Dictionary<T, int> _dict;
        public int this[T key] {get => _dict[key]; }

        public MultiSet()
        {
            _dict = new Dictionary<T, int>();
        }

        public MultiSet(IEnumerable<T> items) : this()
        {
            Add(items);
        }

        public bool Contains(T item)
        {
            return _dict.ContainsKey(item);
        }

        public void Add(T item)
        {
            if (Contains(item)) { _dict[item]++; }
            else { _dict[item] = 1; }   
        }

        public void Add(IEnumerable<T> items)
        {
            foreach (var item in items) { Add(item); }
        }

        public void Remove(T item)
        {
            if (!_dict.ContainsKey(item)) { throw new ArgumentException(); }

            if (--_dict[item] == 0) { _dict.Remove(item); }
        }

        // Return the last value in the multiset
        public T Peek()
        {
            if (!_dict.Any()) { throw new NullReferenceException(); }
               
            return _dict.Last().Key;
        }

        public T Pop()
        {
            T item = Peek();
            Remove(item);
            return item;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (KeyValuePair<T, int> kvp in _dict) 
            {
                yield return kvp.Key;
            }           
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}