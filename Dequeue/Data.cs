using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Double ended queue
/// </summary>
/// <typeparam name="T">type of items in a Deque</typeparam>
public partial class Deque<T>//:IDeque<T>
{
    /// <summary>
    /// contains actuall data od deque, Deque<T> provides abstraction above this data
    /// </summary>
    /// <typeparam name="S"></typeparam>
    private class Data<S>
    {
        private static readonly int sizeOfBlock = 128;
        /// <summary>
        /// Current number of allocated references to data blocks, data blocks themselves doesn't have to be allocated yet
        /// </summary>
        private int NumOfBlockRefs { get; set; } = 2;
        /// <summary>
        /// Current number of actually allocated blocks
        /// </summary>
        private int NumOfBlockInitialized { get; set; } = 2;
        /// <summary>
        /// number of allocated indices before the Head of a Deque
        /// to be able to allocate blocks only when its necessary - only one block at time
        /// </summary>
        private int beforeFirst { get; set; } = sizeOfBlock;
        /// <summary>
        /// index of the first allocated block - in array of block references
        /// </summary>
        private int headBlockIndex { get; set; } = 0;
        /// <summary>
        /// number of allocated indices after the Tail of a Deque
        /// to be able to allocate blocks only when its necessary - only one block at time
        /// </summary>
        private int afterLast => ((NumOfBlockInitialized + headBlockIndex )* sizeOfBlock) - (HeadIndex+Count);

        /// <summary>
        /// index of the firts item in the Deque - relative to the first index in the first block there is reference to (not to the first block actually allocated)
        /// pretends that data is stored linearly
        /// </summary>
        private int HeadIndex { get; set; } = sizeOfBlock;
        /// <summary>
        /// index of last item in the Deque
        /// </summary>
        private int TailIndex 
        { get 
            { if (Count == 0) { return 0; }      
                else {return  HeadIndex + Count - 1; }
            }                   
        }
        /// <summary>
        /// Number of elements in Deque<T>
        /// </summary>
        public int Count { get; private set; } = 0;
        //private int currCapacity { get => (NumOfBlockRefs * sizeOfBlock) - Count; }

        private S[][] data = new S[2][]; 

        public Data()
        {
            this.data[0] = new S[sizeOfBlock];
            this.data[1] = new S[sizeOfBlock];
        }

        /// <summary>
        /// allows to treat the Deque<T> as if it stored data linearly
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public S this[int i]
        {              
            get
            {
                if (i < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                int index = i + HeadIndex;
                if(index > TailIndex)
                {
                    throw new ArgumentOutOfRangeException();
                }
                return this.data[GetIndexOfBlock(index)][GetIndexInBlock(index)];
            }
            set
            {
                if (i < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                int index = i + HeadIndex;
                if (index > TailIndex)
                {
                    throw new ArgumentOutOfRangeException();
                }
                this.data[GetIndexOfBlock(index)][GetIndexInBlock(index)] = value;
            }              
        }


        private int GetIndexOfBlock(int i) => ((i) / sizeOfBlock);
        private int GetIndexInBlock(int i) => ((i) % sizeOfBlock);

        /// <summary>
        /// Doubles the number of references to data blocks, copies existing data blocks to the middle of new reference array of double size
        /// do not actually allocate any data blocks
        /// </summary>
        void DoubleSize()
        {
            int oldNumOfRefs = NumOfBlockRefs;
            NumOfBlockRefs *= 2;
            S[][] newData = new S[NumOfBlockRefs][];
            int occupiedIndex = NumOfBlockRefs / 4;
            data.CopyTo(newData, occupiedIndex);
            this.data = newData;
            this.HeadIndex = (occupiedIndex *sizeOfBlock) +this.HeadIndex;
            this.headBlockIndex = occupiedIndex + this.headBlockIndex;
        }
        /// <summary>
        /// allocs one data block in front of the first block currently allocated
        /// doubles the size of reference array if necessary
        /// </summary>
        private void AllocBlockBeginning()
        {
            if (this.HeadIndex <= 0)
            {
                DoubleSize();
            }
            this.headBlockIndex--;
            this.data[headBlockIndex] = new S[sizeOfBlock];
            this.beforeFirst = sizeOfBlock;
            this.NumOfBlockInitialized++;
        }
        /// <summary>
        /// allocs one data block in front of the first block currently allocated
        /// doubles the size of reference array if necessary
        /// </summary>
        private void AllocBlockEnd()
        {
            if (this.TailIndex >= (NumOfBlockRefs * sizeOfBlock) - 1) //-1 to avoid accessing non existing array
            {
                DoubleSize();
            }
            this.data[headBlockIndex + NumOfBlockInitialized] = new S[sizeOfBlock];
            this.NumOfBlockInitialized++; //this will increment afterLast by  128 - size of block
        }
        /// <summary>
        /// Adds Item as a new Head of the Deque<T>, Count is incremented
        /// </summary>
        /// <param name="item"></param>
        public void AddBegining(S item)
        {
            if (this.beforeFirst <= 0)
            {
                AllocBlockBeginning();
            }
            this.beforeFirst--;
            this.HeadIndex--;
            Count++;
            this[0] = item; //to 0 index relative to head index
        }
        /// <summary>
        /// Adds Item as a new Tail of the Deque<T>, Count is incremented
        /// </summary>
        /// <param name="item"></param>
        public void AddEnd(S item)
        {
            if (this.afterLast <= 0)
            {
                AllocBlockEnd();
            }
            Count++; //incrementing count without dekrementing head index will decrement afterLast
            this[Count-1] = item; //indexing relative to head index
        }
        /// <summary>
        /// returns the firts element of the Deque<T> while removing it from Deque<T>
        /// </summary>
        /// <returns></returns>
        public S RemoveHead()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Deque<T> is empty");
            }
            S item = this[0];
            this[0] = default(S);
            this.HeadIndex++;
            this.Count--;
            this.beforeFirst++;
            return item;
        }
        /// <summary>
        /// returns the last element of the Deque<T> and removes it from Deque<T>
        /// </summary>
        /// <returns></returns>
        public S RemoveTail()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Deque<T> is empty");
            }
            S item = this[Count - 1];
            this[Count - 1] = default(S);
            this.Count--;
            return item;
        }
        /// <summary>
        /// Inserts element on a specified index id Deque<T>
        /// Insert at the beggining or end in O(1)
        /// Allows to insert item on index 0 of an empty Deque<T> - List<T> behaves the same way, I recon
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, S item)
        {
            /*if (this.Count == 0)
            {
                AddBegining(item);
                return;
            }*/
            if (index == Count)
            {
                AddEnd(item);
                return;
            }
            if (index == 0)
            {
                AddBegining(item);
                return;
            }
            if(index<0 || index > Count)
            {
                throw new ArgumentOutOfRangeException("My awesome exception");
            }
            if (this.afterLast <= 0)
            {
                AllocBlockEnd();
            }
            Count++;
            //shift element to tail of the Deque<T> to make space to insert the new element to
            for (int i = this.Count-1; i > index; i--)
            {
                this[i] = this[i - 1];
            }
            this[index] = item;
        }
        /// <summary>
        /// Removes an element from the specified index of Deque<T>, removal of the first and the last item in O(1)
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException("My awesome exception");
            }
            if (index == 0)
            {
                RemoveHead();
                    return;
            }
            if (index == Count-1)
            {
                RemoveTail();
                return;
            }
            for (int i = index; i < Count-1; i++)
            {
                this[i] = this[i + 1];
            }
            this[Count - 1] = default(S);
            Count--;
        }
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire Deque<T>.
        /// </summary>
        /// <param name="S"></param>
        /// <returns>e zero-based index of the first occurrence of item within the entire Deque<T>, if found; otherwise, -1.</returns>
        public int IndexOf(S item)
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
        /// Removes item from the Deque<T>, removal of the first and the last item in O(1)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(S item)
        {
            int index = IndexOf(item);
            if (index == -1)
            {
                return false;
            }
            RemoveAt(index);
            return true;
        }
        /// <summary>
        /// Determines whether an element is in the Deque<T>.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>true if item is found in the List<T>; otherwise, false</returns>
        public bool Contains(S item)
        {
            for (int index = 0; index < Count; index++)
            {
                if (object.Equals(this[index], item))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Removes all elements from the Deque<T>.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < Count; i++)
            {
                this[i] = default(S);
            }
            Count = 0;
            HeadIndex = ((NumOfBlockRefs * sizeOfBlock) / 2)+1;
            // to reset a size of Deque allocated?
            /* this.NumOfBlockRefs = 2;
            this.HeadIndex = sizeOfBlock;
            this.headBlockIndex = 0;
            this.beforeFirst = sizeOfBlock;
            this.NumOfBlockInitialized = 2;
            this.data = new S[NumOfBlockRefs][];
            this.data[0] = new S[sizeOfBlock];
            this.data[1] = new S[sizeOfBlock];*/
        }
        /// <summary>
        /// /// <summary>
        /// Copies the entire Deque<T> to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        /// <param name="reversed"></param>
        public void CopyTo(S[] array, int arrayIndex, bool reversed)
        {
            if (array == null)
            {
                throw new ArgumentNullException();
            }
            if (arrayIndex < 0 || arrayIndex >= array.Length)
            {
                throw new ArgumentOutOfRangeException();
            }
            if(arrayIndex+this.Count>= array.Length)
            {
                throw new ArgumentException("Target array is not long enought to contain source array beggining at given arrayIndex");
            }
            if (reversed)
            {
                for (int i = this.Count-1; i >=0 ; i--)
                {
                    array[arrayIndex + Count-1-i] = this[i];
                }
            }
            else
            {
                for (int i = 0; i < this.Count; i++)
                {
                    array[arrayIndex + i] = this[i];
                }
            }
        }
    }
}
    
    
