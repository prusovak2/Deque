using System;
using System.Collections;
using System.Collections.Generic;

public interface IDeque<T> : IList<T>
{
    T First { get; }
    T Last { get;  }
    void AddHead(T item);
    T RemoveHead();
    T RemoveTail();
    IDeque<T> Reverse();
}
public partial class Deque<T> :IDeque<T>
{
    /// <summary>
    /// actuall data of this Deque<T>
    /// </summary>
    private Data<T> data = new Data<T>();

    /// <summary>
    /// to detect changes during enumeration
    /// </summary>
    public long version { get; private set; } = 0;
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
    /// <summary>
    /// peek the firts element of the Deque<T>
    /// </summary>
    public T First { 
        get
        {
            return this.data[0];
        } 
    }
    /// <summary>
    /// peek the last element of the Deque<T>
    /// </summary>
    public T Last
    {
        get
        {
            return this.data[Count-1];
        }
    }
    /// <summary>
    /// Number of elements in Deque<T>
    /// </summary>
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
    /// <summary>
    /// Adds an element to the beggining of the Deque<T>
    /// </summary>
    /// <param name="item"></param>
    public void AddHead(T item)
    {
        this.data.AddBegining(item);
        version++;
    }
    /// <summary>
    /// returns the firts element of the Deque<T> and removes it from Deque<T>
    /// </summary>
    /// <returns></returns>
    public T RemoveHead()
    {
        T item = data.RemoveHead();
        version++;
        return item;
    }
    /// <summary>
    /// returns the last element of the Deque<T> and removes it from Deque<T>
    /// </summary>
    /// <returns></returns>
    public T RemoveTail()
    {
        T item = data.RemoveTail();
        version++;
        return item;
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
    /// <summary>
    /// Copies the entire Deque<T> to a compatible one-dimensional array, starting at the specified index of the target array.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="arrayIndex"></param>
    public void CopyTo(T[] array, int arrayIndex)
    {
        data.CopyTo(array, arrayIndex, false);
    }
    /// <summary>
    /// Copies the entire Deque<T> to a compatible one-dimensional array, starting at the specified index of the target array, in reversed order
    /// </summary>
    /// <param name="array"></param>
    /// <param name="arrayIndex"></param>
    public void CopyToReversed(T[] array, int arrayIndex)
    {
        data.CopyTo(array, arrayIndex, true);
    }

    public IEnumerator<T> GetEnumerator()
    {
        long version = this.version;
        // return new Enumerator<T>(this, this.version);
        for (int i = 0; i < Count; i++)
        {
            if (version != this.version)
            {
                throw new InvalidOperationException();
            }
            yield return data[i];
            //to throw an exeption even when the Deque<T> has been Cleared and therefore no other iteration of for loop
            //is not gonna take a place due to Count=0
            if (version != this.version)
            {
                throw new InvalidOperationException();
            }
        }
    }
    /*private IEnumerator<T> ActuallyGetEnumerator()
    {
        for (int i = 0; i < Count; i++)
        {

        }
    }*/

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
    /// <summary>
    /// returns reverse view on this instance of Deque<T>
    /// </summary>
    /// <returns></returns>
    public IDeque<T> Reverse()
    {
        return new ReverseView<T>(this);
    }
}

