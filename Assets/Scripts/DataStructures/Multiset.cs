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

    public class FrozenMultiSet<T> : MultiSet<T>, IEquatable<FrozenMultiSet<T>>
    {
        public FrozenMultiSet() : base() { }
        public FrozenMultiSet(IEnumerable<T> items) : base(items) { }
        
        public override int GetHashCode()
        {
            return GetOrderIndependentHashCode(_dict);
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

        public override string ToString()
        {
            string output = "FMS[ ";
            foreach (KeyValuePair<T, int> kvp in _dict)
            {
                output += kvp.Key + " : " + kvp.Value + " ";
            }
            return output + "]";
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) { return false; }
            return Equals(obj as FrozenMultiSet<T>);
        }

        public bool Equals(FrozenMultiSet<T> other)
        {
            if (_dict.Count != other._dict.Count) {  return false; }
            foreach (T key in _dict.Keys)
            {
                if (!other._dict.ContainsKey(key)) { return false; }
                if (other._dict[key]  != _dict[key]) { return false; };
            }
            return true;
        }

        public static bool operator==(FrozenMultiSet<T> x, FrozenMultiSet<T> y) { return x.Equals(y); }
        public static bool operator!=(FrozenMultiSet<T> x, FrozenMultiSet<T> y) { return !x.Equals(y); }

        static int GetOrderIndependentHashCode<G>(IEnumerable<G> source)
        {
            // https://stackoverflow.com/questions/670063/getting-hash-of-a-list-of-strings-regardless-of-order/
            int hash = 0;
            int curHash;
            int bitOffset = 0;
            // Stores number of occurences so far of each value.
            var valueCounts = new Dictionary<G, int>();

            foreach (G element in source)
            {
                curHash = EqualityComparer<G>.Default.GetHashCode(element);
                if (valueCounts.TryGetValue(element, out bitOffset))
                    valueCounts[element] = 0 + 1;
                else
                    valueCounts.Add(element, 0);

                // The current hash code is shifted (with wrapping) one bit
                // further left on each successive recurrence of a certain
                // value to widen the distribution.
                // 37 is an arbitrary low prime number that helps the
                // algorithm to smooth out the distribution.
                hash = unchecked(hash + ((curHash << 0) | (curHash >> (32 - 0))) * 37);
            }

            return hash;
        }
    }
}