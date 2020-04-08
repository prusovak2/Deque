using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Deque
{
    public class ReverseView<T>: IDeque<T>
    {
        private Deque<T> deque;

        public ReverseView(Deque<T> que)
        {
            this.deque = que;
        }

        public T this[int index] { get => deque[Count - 1 - index]; set => deque[Count - 1 - index] = value; }

        public T First => deque.Last;

        public T Last => deque.First;

        public int Count => deque.Count;

        public bool IsReadOnly => deque.IsReadOnly;

        public void Add(T item)
        {
            deque.AddHead(item);
        }

        public void AddHead(T item)
        {
            deque.Add(item);
        }

        public void Clear()
        {
            deque.Clear();
        }

        public bool Contains(T item)
        {
            return deque.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            deque.CopyToReversed(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ReversedEnumerator<T>(this.deque, this.deque.version);
        }

        public int IndexOf(T item)
        {
            return (Count - 1 - deque.IndexOf(item));
        }

        public void Insert(int index, T item)
        {
            deque.AppendAt(Count - 1 - index, item);
        }

        public bool Remove(T item)
        {
            return deque.Remove(item);
        }

        public void RemoveAt(int index)
        {
            deque.RemoveAt(Count -1 - index);
        }

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
            long version { get; set; }
            private Deque<S> Que { get; set; }

            public ReversedEnumerator(Deque<S> que, long version)
            {
                this.version = version;
                this.Que = que;
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
                if (this.curIndex <= 0)
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
}
