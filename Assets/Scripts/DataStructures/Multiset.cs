using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataStructures 
{
    public class SortedMultiSet<T> : IEnumerable<T>
    {
        protected SortedDictionary<T, int> _dict;
        public int this[T key] {get => _dict[key]; }

        public SortedMultiSet()
        {
            _dict = new SortedDictionary<T, int>();
        }

        public SortedMultiSet(IEnumerable<T> items) : this()
        {
            Add(items);
        }

        public bool Contains(T item)
        {
            return _dict.ContainsKey(item);
        }

        public void Add(T item)
        {
            if (_dict.ContainsKey(item)) { _dict[item]++; }
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

        // Return the last value in the multiset and remove it.
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

    public class FrozenMultiSet<T> : SortedMultiSet<T>
    {
        public FrozenMultiSet() : base() { }
        public FrozenMultiSet(IEnumerable<T> items) : base(items) { }
        
        public override int GetHashCode()
        {
            return _dict.GetHashCode();
        }
        public int TotalCount 
        { 
            get 
            {
                int total = 0;
                foreach (int value in _dict.Values) { total += value; }
                return total;
            } 
        }
    }
}