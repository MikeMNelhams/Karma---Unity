using System;
using System.Collections.Generic;

namespace DataStructures
{
    public class FrozenMultiSet<T> : MultiSet<T>, IEquatable<FrozenMultiSet<T>>
    {
        public FrozenMultiSet() : base() { }
        public FrozenMultiSet(IEnumerable<T> items) : base(items) { }
        public FrozenMultiSet(Dictionary<T, int> items)
        {
            _dict = items;
        }
        public FrozenMultiSet(T item, int count)
        {
            _dict = new Dictionary<T, int> { [item] = count };
        }

        public override int GetHashCode()
        {
            return GetOrderIndependentHashCode(_dict);
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
            if (_dict.Count != other._dict.Count) { return false; }
            foreach (T key in _dict.Keys)
            {
                if (!other._dict.ContainsKey(key)) { return false; }
                if (other._dict[key] != _dict[key]) { return false; };
            }
            return true;
        }

        public static bool operator ==(FrozenMultiSet<T> x, FrozenMultiSet<T> y) { return x.Equals(y); }
        public static bool operator !=(FrozenMultiSet<T> x, FrozenMultiSet<T> y) { return !x.Equals(y); }

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