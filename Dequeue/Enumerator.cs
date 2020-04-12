using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public partial class Deque<T>
{
    private class Enumerator<S> : IEnumerator<S>
    {
        //initialize with -1 to ensure that InvalidOperationException is thrown when Current is called befor the first call of MoveNext
        private int curIndex { get; set; } = -1;
        /// <summary>
        /// version of Deque<T> this Enumerator is enumerating from the moment this enumerator has been created
        /// </summary>
        long version { get; set; }
        /// <summary>
        /// Deque<T> this enumerator is enumerating
        /// </summary>
        private Deque<S> Que { get; set; }

        public Enumerator(Deque<S> que, long version)
        {
            this.version = version;
            this.Que = que;
        }
        public S Current { 
            get
            {
                if (curIndex<0 || curIndex >= this.Que.Count || this.version != Que.version)
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
            this.Que = default;
            this.curIndex = default;
            this.version = default;
        }

        public bool MoveNext()
        {
            if (this.version != this.Que.version)
            {
                throw new InvalidOperationException();
            }
            this.curIndex++;
            if (this.curIndex >= this.Que.Count)
            {
                return false;
            }
            return true;
        }

        public void Reset()
        {
            this.curIndex = 0;
        }
    }
}

