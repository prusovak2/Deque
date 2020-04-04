using System;
using System.Collections.Generic;

namespace Dequeue
{
    public interface IDeque<T> : IList<T>
    {
        
    }
    public partial class Deque<T> //:IDeque<T>
    {
        private Data<T> data = new Data<T>();

        public T this [int i]
        {
            get 
            {
                return this.data[i];
            }
            set 
            {
                this.data[i] = value;
            }
        }
        public int Count => this.data.Count;
        public void Add(T item)
        {
            this.data.AddEnd(item);
        }
        public void AddHead(T item)
        {
            this.data.AddBegining(item);
        }

    }
}
