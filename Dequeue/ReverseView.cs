using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


public class ReverseView<T>: IDeque<T>
{
    /// <summary>
    /// Deque<T> that this instance of ReverseView wraps and allows to access in the reversed order
    /// </summary>
    private Deque<T> deque { get; set; }

    public ReverseView(Deque<T> que)
    {
        this.deque = que;
    }

    public T this[int index] { get => deque[Count - 1 - index]; set => deque[Count - 1 - index] = value; }

    /// <summary>
    /// peek the firts element of the reversed Deque<T>
    /// </summary>
    public T First => deque.Last;
    /// <summary>
    /// peek the last element of the reversed Deque<T>
    /// </summary>
    public T Last => deque.First;
    /// <summary>
    /// Number of elements in Deque<T>
    /// </summary>
    public int Count => deque.Count;

    public bool IsReadOnly => deque.IsReadOnly;
    /// <summary>
    /// Adds an object to the end of the reversed Deque<T>.
    /// </summary>
    /// <param name="item"></param>
    public void Add(T item)
    {
        deque.AddHead(item);
    }
    /// <summary>
    /// Adds an element to the beggining of the reversed Deque<T>
    /// </summary>
    /// <param name="item"></param>
    public void AddHead(T item)
    {
        deque.Add(item);
    }
    /// <summary>
    /// Removes all elements from the Deque<T>.
    /// </summary>
    public void Clear()
    {
        deque.Clear();
    }
    /// <summary>
    /// Determines whether an element is in the Deque<T>.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>true if item is found in the List<T>; otherwise, false</returns>
    public bool Contains(T item)
    {
        return deque.Contains(item);
    }
    /// <summary>
    /// Copies the entire Deque<T> to a compatible one-dimensional array, starting at the specified index of the target array.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="arrayIndex"></param>
    public void CopyTo(T[] array, int arrayIndex)
    {
        deque.CopyToReversed(array, arrayIndex);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new ReversedEnumerator<T>(this.deque, this.deque.version);
    }
    /// <summary>
    /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire Deque<T>.
    /// </summary>
    /// <param name="T"></param>
    /// <returns>e zero-based index of the first occurrence of item within the entire Deque<T>, if found; otherwise, -1.</returns>
    public int IndexOf(T item)
    {
        for (int index = 0; index < Count; index++)
        {
            if (object.Equals(this[index], item))
            {
                return index;
            }
        }
        return -1;
    }
    /// <summary>
    /// Inserts an element into the reversedDeque<T> at the specified index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    public void Insert(int index, T item)
    {
        //user wants to add item to begining of reversed list
        if (index == 0)
        {
            //I need to add it to end of actual list
            deque.Add(item);
            return;
        }
        else
        {
            deque.Insert(Count - index, item);
        }
    }
    /// <summary>
    /// /// Removes the first occurrence of a specific object from the Deque<T>.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the List<T>.</returns>
    public bool Remove(T item)
    {
        return deque.Remove(item);
    }
    /// <summary>
    /// Removes the element at the specified index of the Deque<T>.
    /// </summary>
    /// <param name="index"></param>
    public void RemoveAt(int index)
    {
        deque.RemoveAt(Count -1 - index);
    }
    /// <summary>
    /// returns the firts element of the reversed Deque<T> and removes it from Deque<T>
    /// </summary>
    /// <returns></returns>
    public T RemoveHead()
    {
        return deque.RemoveTail();
    }
    /// <summary>
    /// returns the last element of the reversed Deque<T> and removes it from Deque<T>
    /// </summary>
    /// <returns></returns>
    public T RemoveTail()
    {
        return deque.RemoveHead();
    }
    /// <summary>
    /// returns 
    /// </summary>
    /// <returns></returns>
    public IDeque<T> Reverse()
    {
        return this.deque;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();   
    }


    private class ReversedEnumerator<S> : IEnumerator<S>
    {
        private int curIndex { get; set; }
        /// <summary>
        /// version of Deque<T> this Enumerator is enumerating from the moment this enumerator has been created
        /// </summary>
        long version { get; set; }
        /// <summary>
        /// Deque<T> this enumerator is enumerating
        /// </summary>
        private Deque<S> Que { get; set; }

        public ReversedEnumerator(Deque<S> que, long version)
        {
            this.version = version;
            this.Que = que;
            //initialize with que.Count to ensure that InvalidOperationException is thrown when Current is called befor the first call of MoveNext
            this.curIndex = que.Count;
        }
        public S Current
        {
            get
            {
                if (curIndex < 0 || curIndex >= this.Que.Count || this.version != Que.version)
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    return this.Que[curIndex];
                }
            }
        }

        object IEnumerator.Current => throw new NotImplementedException();

        public void Dispose()
        {
            this.Que = default(Deque<S>);
            this.curIndex = default(int);
            this.version = default(long);
        }

        public bool MoveNext()
        {
            if (this.version != this.Que.version)
            {
                throw new InvalidOperationException();
            }
            this.curIndex--;
            if (this.curIndex < 0)
            {
                return false;
            }
            return true;
        }

        public void Reset()
        {
            this.curIndex = this.Que.Count-1;
        }
    }
}

