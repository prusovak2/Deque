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
            private int NumOfUsedBlocks { get; set; } = 2;
            private int TailBlock { get => HeadBlock + NumOfUsedBlocks -1; }
            private int HeadBlock { get; set; } = 0;


            private int HeadIndex { get; set; } = sizeOfBlock;
            private int TailIndex { get => HeadIndex + Count -1; }
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
                    }
                    if (i < 0)
                    {
                        throw new IndexOutOfRangeException();
                    }
                    int index = i + HeadIndex + HeadBlock*sizeOfBlock;
                    if(index > TailIndex+HeadBlock*sizeOfBlock)
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
                    //both head and tail index are relative to begining of first existing block
                    int index = i + HeadIndex + HeadBlock* sizeOfBlock;
                    if (index > TailIndex+HeadBlock*sizeOfBlock)
                    {
                        throw new IndexOutOfRangeException();
                    }
                    this.data[GetIndexOfBlock(index)][GetIndexInBlock(index)] = value;
                }              
            }


            private int GetIndexOfBlock(int i) => ((i) / sizeOfBlock);
            private int GetIndexInBlock(int i) => ((i) % sizeOfBlock);

            private void AddBlockBegining()
            {
                if (this.HeadBlock == 0)
                {
                    //double num of references
                    this.NumOfBlockRefs *= 2;
                    S[][] newData = new S[this.NumOfBlockRefs][];
                    //place existing array in the middle of the old one and alloc one new block in front of it (-1)
                    this.HeadBlock = (this.NumOfBlockRefs / 4) -1;
                    this.data.CopyTo(newData, this.HeadBlock+1);
                    newData[HeadBlock] = new S[sizeOfBlock];
                    this.data = newData;
                    //head should point to existing item at the beggining of second bloc
                    this.HeadIndex = sizeOfBlock;
                }
                else
                {
                    this.HeadBlock--;
                    this.data[this.HeadBlock] = new S[sizeOfBlock];
                    this.HeadIndex = sizeOfBlock;
                }
                NumOfUsedBlocks++;
            }
            private void AddBlockEnd()
            {
                if(this.TailBlock== this.NumOfBlockRefs-1)
                {
                    //double num of references
                    this.NumOfBlockRefs *= 2;
                    S[][] newData = new S[this.NumOfBlockRefs][];
                    //place existing array in the middle of the new one
                    this.HeadBlock = (this.NumOfBlockRefs / 4);
                    this.data.CopyTo(newData, this.HeadBlock);
                    this.NumOfUsedBlocks++; //this does increment tailBlock index
                    newData[TailBlock] = new S[sizeOfBlock];
                    this.data = newData;
                    //do not change head index
                }
                else
                {
                    this.NumOfUsedBlocks++; //this does increment tailBlock index
                    this.data[TailBlock] = new S[sizeOfBlock];
                }
            }

            public void AddBegining(S item)
            {
                if (this.HeadIndex <= 0)
                {
                    AddBlockBegining();
                }
                this.HeadIndex--;
                Count++;
                this[0] = item; //to 0 index relative to head index
            }
            public void AddEnd(S item)
            {
                if(Count == 257)
                {

                }

                if (this.TailIndex >= (NumOfUsedBlocks*sizeOfBlock)-1)
                {
                    AddBlockEnd();
                }
                Count++;
                this[TailIndex-HeadIndex] = item; //indexing relative to head index
            }

        }
    }
    
}
