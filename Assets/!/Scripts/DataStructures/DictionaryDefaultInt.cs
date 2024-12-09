using System;
using System.Collections.Generic;

namespace DataStructures
{
    public class DictionaryDefaultInt<TKey> : IEquatable<DictionaryDefaultInt<TKey>>
    {
        readonly IDictionary<TKey, int> _dictionary;
        readonly int _defaultValue;

        public DictionaryDefaultInt(int defaultValue = 0)
        {
            _dictionary = new Dictionary<TKey, int>();
            _defaultValue = defaultValue;
        }

        public IEnumerator<KeyValuePair<TKey, int>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, int> item)
        {
            if (!_dictionary.ContainsKey(item.Key)) { _dictionary[item.Key] = _defaultValue; }
            _dictionary[item.Key] += item.Value;
        }

        /* Uses the defaultValue of THIS dictionary. Returns new dictionaryIntDefault. */
        public DictionaryDefaultInt<TKey> Union(DictionaryDefaultInt<TKey> other)
        {
            DictionaryDefaultInt<TKey> combinedDictionary = new (_defaultValue);
            if (other.Count == 0) { return combinedDictionary; }

            foreach (KeyValuePair<TKey, int> item in other)
            {
                combinedDictionary.Add(item);
            }

            return combinedDictionary;
        }

        /* Uses the defaultValue of THIS dictionary */
        public void UnionInPlace(DictionaryDefaultInt<TKey> other)
        {
            if (other.Count == 0) { return; }

            foreach (KeyValuePair<TKey, int> item in other)
            {
                Add(item);
            }
        }

        public void SubtractInPlace(DictionaryDefaultInt<TKey> other)
        {
            if (other.Count == 0) { return; }

            foreach (KeyValuePair<TKey, int> item in other)
            {
                if (!_dictionary.ContainsKey(item.Key)) { _dictionary[item.Key] = _defaultValue; }
                _dictionary[item.Key] -= item.Value;
            }
        }

        public override bool Equals(Object other)
        {
            if (other is null) { return false; }
            if (other is not DictionaryDefaultInt<TKey>) { return false; }
            return Equals(other as DictionaryDefaultInt<TKey>);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(DictionaryDefaultInt<TKey> other)
        {
            if (other is null) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            if (other._defaultValue != _defaultValue) { return false; }
            return _dictionary == other._dictionary;
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, int> item)
        {
            return _dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, int>[] array, int arrayIndex)
        {
            _dictionary.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, int> item)
        {
            return _dictionary.Remove(item);
        }

        public int Count
        {
            get { return _dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return _dictionary.IsReadOnly; }
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void Add(TKey key, int count)
        {
            if (!_dictionary.ContainsKey(key)) { _dictionary[key] = _defaultValue; }
            _dictionary[key] += count;
        }

        public bool Remove(TKey key)
        {
            return _dictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out int value)
        {
            if (!_dictionary.TryGetValue(key, out value))
            {
                value = _defaultValue;
            }

            return true;
        }

        public int this[TKey key]
        {
            get
            {
                TryGetValue(key, out int value);
                return value;
            }

            set { _dictionary[key] = value; }
        }

        public ICollection<TKey> Keys
        {
            get { return _dictionary.Keys; }
        }

        public ICollection<int> Values
        {
            get
            {
                var values = new List<int>(_dictionary.Values) {
                    _defaultValue
                };
                return values;
            }
        }

        public override string ToString()
        {
            if (Count == 0) { return string.Empty; }

            string message = "DictionaryDefaultInt[";
            foreach (var kvp in _dictionary)
            {
                message += kvp.Key + ": " + kvp.Value + ", ";
            }
            return message + "]";
        }
        public static bool operator ==(DictionaryDefaultInt<TKey> x, DictionaryDefaultInt<TKey> y) => x.Equals(y);
        public static bool operator !=(DictionaryDefaultInt<TKey> x, DictionaryDefaultInt<TKey> y) => !(x == y);
    }
}
