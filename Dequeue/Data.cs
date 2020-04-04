using System;
using System.Collections.Generic;
using System.Text;

namespace Dequeue
{
    public partial class Deque<T>//:IDeque<T>
    {
        private class Data<S>
        {
            private static readonly int sizeOfBlock = 128;
            private int NumOfBlockRefs { get; set; } = 2;
            ///private int NumOfUsedBlocks { get; set; } = 2;
            ///private int TailBlock { get => HeadBlock + NumOfUsedBlocks -1; }
            //private int HeadBlock { get; set; } = 0;


            private int HeadIndex { get; set; } = sizeOfBlock;
            private int TailIndex 
            { get 
                { if (Count == 0) { return 0; }      
                    else {return  HeadIndex + Count - 1; }
                }                   
            }
            public int Count { get; private set; } = 0;
            //private int currCapacity { get => (NumOfBlockRefs * sizeOfBlock) - Count; }

            private S[][] data = new S[2][]; 

            public Data()
            {
                this.data[0] = new S[sizeOfBlock];
                this.data[1] = new S[sizeOfBlock];
            }

            public S this[int i]
            {              
                get
                {
                    
                    if (i < 0)
                    {
                        throw new IndexOutOfRangeException();
                    }
                    int index = i + HeadIndex;
                    if(index > TailIndex)
                    {
                        throw new IndexOutOfRangeException();
                    }
                    return this.data[GetIndexOfBlock(index)][GetIndexInBlock(index)];
                }
                set
                {
                    if (i < 0)
                    {
                        throw new IndexOutOfRangeException();
                    }
                    int index = i + HeadIndex;
                    if (index > TailIndex)
                    {
                        throw new IndexOutOfRangeException();
                    }
                    this.data[GetIndexOfBlock(index)][GetIndexInBlock(index)] = value;
                }              
            }


            private int GetIndexOfBlock(int i) => ((i) / sizeOfBlock);
            private int GetIndexInBlock(int i) => ((i) % sizeOfBlock);

            void DoubleSize()
            {
                int oldNumOfRefs = NumOfBlockRefs;
                NumOfBlockRefs *= 2;
                S[][] newData = new S[NumOfBlockRefs][];
                int occupiedIndex = NumOfBlockRefs / 4;
                data.CopyTo(newData, occupiedIndex);
                for (int i = 0; i < occupiedIndex; i++)
                {
                    newData[i] = new S[sizeOfBlock];
                    newData[occupiedIndex+oldNumOfRefs + i] = new S[sizeOfBlock];
                }
                this.data = newData;
                this.HeadIndex = (occupiedIndex *sizeOfBlock) +this.HeadIndex;
            }

            public void AddBegining(S item)
            {
                if (this.HeadIndex <= 0)
                {
                    DoubleSize();
                }
                this.HeadIndex--;
                Count++;
                this[0] = item; //to 0 index relative to head index
            }
            public void AddEnd(S item)
            {
                if (this.TailIndex >= (NumOfBlockRefs*sizeOfBlock)-1) //-1 to avoid accessing non existing array
                {
                    DoubleSize();
                }
                Count++;
                this[Count-1] = item; //indexing relative to head index
            }
            public void Insert(int index, S item)
            {
                if(index<0 || index >= Count)
                {
                    throw new IndexOutOfRangeException("My awesome exception");
                }
                if (this.TailIndex >= (NumOfBlockRefs * sizeOfBlock) - 1) //-1 to avoid accessing non existing array
                {
                    DoubleSize();
                }
                Count++;
                for (int i = this.Count-1; i > index; i--)
                {
                    this[i] = this[i - 1];
                }
                this[index] = item;
            }
            public void RemoveAt(int index)
            {
                if (index < 0 || index >= Count)
                {
                    throw new IndexOutOfRangeException("My awesome exception");
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
            public void Clear()
            {
                for (int i = 0; i < Count; i++)
                {
                    this[i] = default(S);
                }
                Count = 0;
                // to reset a size of Deque allocated?
                this.NumOfBlockRefs = 2;
                this.HeadIndex = sizeOfBlock;
                this.data = new S[NumOfBlockRefs][];
                this.data[0] = new S[sizeOfBlock];
                this.data[1] = new S[sizeOfBlock];
            }

        }
    }
    
}
