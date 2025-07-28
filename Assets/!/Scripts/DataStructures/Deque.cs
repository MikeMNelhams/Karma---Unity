using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DataStructures
{
    public class Deque<T>
    {
        protected LinkedList<T> _items;

        public T this[int index] { get => _items.ElementAt(index);}

        public Deque(List<T> items)
        {
            _items = new LinkedList<T>(items);
        }

        public Deque()
        {
            _items = new LinkedList<T>();
        }

        public void AddLeft(T item)
        {
            _items.AddFirst(item);
        }

        public void AddRight(T item)
        {
            _items.AddLast(item);
        }

        public T PopLeft()
        {
            T item = _items.First.Value;
            _items.RemoveFirst();
            return item;
        }

        public T PopRight()
        {
            T item = _items.Last.Value;
            _items.RemoveLast();
            return item;
        }

        public void Clear()
        {
            _items.Clear();
        }

        public void Rotate(int numberOfRotations)
        {
            int adjustedRotations = numberOfRotations % _items.Count;
            adjustedRotations = adjustedRotations < 0 ? adjustedRotations + _items.Count : adjustedRotations;
            for (int i = 0; i < adjustedRotations; i++)
            {
                AddLeft(PopRight());
            }
        }

        public int Count
        {
            get
            {
                return _items.Count;
            }
        }
    }
}

