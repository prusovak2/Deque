using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Deque
{
    public partial class Deque<T>
    {
        private class Enumerator<S> : IEnumerator<S>
        {
            private int curIndex { get; set; } = -1;
            long version { get; set; }
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
}
