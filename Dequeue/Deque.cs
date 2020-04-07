using System;
using System.Collections;
using System.Collections.Generic;

namespace Dequeue
{
    public interface IDeque<T> : IList<T>
    {
        T First { get; }
        T Last { get;  }
        void AddHead(T item);
    }
    public partial class Deque<T> :IDeque<T>
    {
        private Data<T> data = new Data<T>();

        private long version { get; set; } = 0;
        public T this [int i]
        {
            get 
            {
                return this.data[i];
            }
            set 
            {
                this.data[i] = value;
                version++;
            }
        }
        public T First => data[0];
        public T Last => data[data.Count - 1];
        public int Count => this.data.Count;

        public bool IsReadOnly => false;
        /// <summary>
        /// Adds an object to the end of the Deque<T>.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            this.data.AddEnd(item);
            version++;
        }
        public void AddHead(T item)
        {  
            this.data.AddBegining(item);
            version++;
        }
        /// <summary>
        /// Removes all elements from the Deque<T>.
        /// </summary>
        public void Clear()
        {
            data.Clear();
            version++;
        }
        /// <summary>
        /// Determines whether an element is in the Deque<T>.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>true if item is found in the Deque<T>; otherwise, false.</returns>
        public bool Contains(T item)
        {
            return data.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            data.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator<T>(this, this.version);
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire Deque<T>.
        /// </summary>
        /// <param name="T"></param>
        /// <returns>e zero-based index of the first occurrence of item within the entire Deque<T>, if found; otherwise, -1.</returns>
        public int IndexOf(T item)
        {
            return data.IndexOf(item);
        }
        /// <summary>
        /// Inserts an element into the Deque<T> at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, T item)
        {
            data.Insert(index, item);
            version++;
        }
        /// <summary>
        /// /// Removes the first occurrence of a specific object from the Deque<T>.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the List<T>.</returns>
        public bool Remove(T item)
        {
            //TODO: do not change version when removal fails?
            version++;
            return data.Remove(item);
        }
        /// <summary>
        /// Removes the element at the specified index of the Deque<T>.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            data.RemoveAt(index);
            version++;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
