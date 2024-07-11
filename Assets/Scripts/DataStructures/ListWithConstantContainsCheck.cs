using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace DataStructures
{
    /* Basically a list, but with O(1) check for whether an object is in the list or not */
    public class ListWithConstantContainsCheck<T> : IList<T>
    {
        readonly List<T> _orderedValues;
        readonly Dictionary<T, int> _valueIndices;

        public ListWithConstantContainsCheck()
        {
            _orderedValues = new List<T>();
            _valueIndices = new Dictionary<T, int>();
        }

        public ListWithConstantContainsCheck(List<T> values)
        {
            _orderedValues = values;
            _valueIndices = new Dictionary<T, int>();

            for (int i = 0; i < values.Count; i++)
            {
                _valueIndices[values[i]] = i;
            }
        }

        public T this[int index]
        {
            get => _orderedValues[index];
            set
            {
                _orderedValues[index] = value;
                _valueIndices[value] = index;
            }
        }

        public int Count => _orderedValues.Count;

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            if (_valueIndices.ContainsKey(item)) { throw new DuplicateElementException(); }
            _orderedValues.Add(item);
            _valueIndices[item] = _orderedValues.Count - 1;
        }

        public void AddRange(T[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                T item = items[i];
                Add(item);
            }
        }

        public void AddRange(List<T> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                T item = items[i];
                Add(item);
            }
        }

        public void AddRange(ListWithConstantContainsCheck<T> items)
        {
            AddRange(items._orderedValues);
        }

        public void Clear()
        {
            _orderedValues.Clear();
            _valueIndices.Clear();
        }

        public bool Contains(T item)
        {
            return _valueIndices.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            array[arrayIndex] = this[arrayIndex];
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _orderedValues.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return _valueIndices[item];
        }

        public bool Remove(T item)
        {
            /* TODO Make a more efficient single pass version for removing multiple: RemoveRange() */
            int index = IndexOf(item);
            _orderedValues.RemoveAt(index);

            for (int i = index + 1; i < _orderedValues.Count; i++)
            {
                T item2 = _orderedValues[i];
                _valueIndices[item2] = index - 1;
            }

            return _valueIndices.Remove(item);
        }

        public void Insert(int index, T item)
        {
            if (Contains(item)) { throw new DuplicateElementException(); }
            _orderedValues.Insert(index, item);
            _valueIndices[item] = index;

            for (int i = index + 1; i < _orderedValues.Count; i++)
            {
                T item2 = _orderedValues[i];
                _valueIndices[item2] += 1;
            }
        }

        public void RemoveAt(int index)
        {
            T value = _orderedValues[index];
            _orderedValues.RemoveAt(index);
            _valueIndices.Remove(value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            string message = "ListWCCC[ ";
            foreach (T item in _orderedValues)
            {
                message += item.ToString() + ", ";
            }

            return message + "]";
        }
    }

    public class DuplicateElementException : System.Exception
    {
        public DuplicateElementException(string message) : base(message) { }
        public DuplicateElementException()
        {
            string message = "Duplicate element added to ListWithConstantContainCheck";
            throw new DuplicateElementException(message);
        }
    }
}