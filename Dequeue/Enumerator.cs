using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Dequeue
{
    public partial class Deque<T>
    {
        private class Enumerator<S> : IEnumerator<S>
        {
            private Deque<S> Que;
            public S Current => throw new NotImplementedException();

            object IEnumerator.Current => throw new NotImplementedException();

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public bool MoveNext()
            {
                throw new NotImplementedException();
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }
    }
}
