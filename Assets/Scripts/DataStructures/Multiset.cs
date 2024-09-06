using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace DataStructures 
{
    public class MultiSet<T> : IEnumerable<T>
    {
        protected Dictionary<T, int> _dict;
        public int this[T key] { get => _dict[key]; }

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

        public void Add(T item, int count)
        {
            if (Contains(item)) { _dict[item] += count; }
            else { _dict[item] = count; }
        }

        public void Add(IEnumerable<T> items)
        {
            foreach (var item in items) { Add(item); }
        }

        public void AddMax(T item, int count)
        {
            if (Contains(item)) { _dict[item] = Math.Max(count, _dict[item]); }
            else { _dict[item] = count; }
        }

        public void AddMax(MultiSet<T> other)
        {
            foreach (T item in other)
            {
                AddMax(item, other[item]);
            }
        }

        public void Remove(T item)
        {
            if (!_dict.ContainsKey(item)) { throw new ArgumentException(); }

            if (--_dict[item] == 0) { _dict.Remove(item); }
        }

        public void Remove(T item, int count)
        {
            if (!_dict.ContainsKey(item)) { throw new ArgumentException(); }
            int countDict = _dict[item];
            countDict -= count;
            if (countDict <= 0) { _dict.Remove(item); }
            else { _dict[item] = countDict; }
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

        public bool IsSubset(MultiSet<T> other)
        {
            foreach (var kvp in _dict)
            {
                int thisCount = _dict[kvp.Key];
                if (thisCount == 0) { continue; }

                if (!other.Contains(kvp.Key)) { return false; }
                int otherCount = other[kvp.Key];
                if (thisCount > otherCount) { return false; }
            }

            return true;
        }

        public int KeyCount { get => _dict.Keys.Count; }

        public Dictionary<T, int>.KeyCollection Keys { get => _dict.Keys; }

        public void UnionWith(MultiSet<T> other)
        {
            foreach (T key in other)
            {
                Add(key, other[key]);
            }
        }

        public void ExceptWith(MultiSet<T> other)
        {
            foreach (T key in other)
            {
                Remove(key, other[key]);
            }
        }

        public void UnionMaxWith(MultiSet<T> other)
        {
            foreach(T key in other)
            {
                AddMax(key, other[key]);
            }
        }

        public int Max()
        {
            int maximum = 0;
            foreach (T key in _dict.Keys)
            {
                maximum = Math.Max(_dict[key], maximum);
            }
            return maximum;
        }
    }
}