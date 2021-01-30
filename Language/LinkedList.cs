using System;

using System.Text;

namespace Language
{
    class LinkedList
    {
        private LLElement First;
        private LLElement Current;
        private LLElement Last;
        public int Size;

        public LinkedList()
        {
            Size = 0;
            First = Current = Last = null;
        }

        public bool IsEmpty() => Size == 0;

        public void InsertAt(dynamic newElement, int index)
        {
            if (index < 0 || index > Size && Size != 0)
            {
                throw new InvalidOperationException();
            }
            else if (index == 0)
            {
                LLElement newNode = new LLElement(newElement);

                if (First == null)
                {
                    First = Last = newNode;
                }
                else
                {
                    newNode.Next = First;
                    First = newNode;
                    newNode.Next.Previous = First;
                }
                Size++;
            }
            else if (index == Size)
            {
                LLElement newNode = new LLElement(newElement);

                if (First == null)
                {
                    First = Last = newNode;
                }
                else
                {
                    Last.Next = newNode;
                    newNode.Previous = Last;
                    Last = newNode;
                }
                Size++;
            }
            else
            {
                int count = 0;
                Current = First;
                while (Current != null && count != index)
                {
                    Current = Current.Next;
                    count++;
                }
                LLElement newNode = new LLElement(newElement);
                Current.Previous.Next = newNode;
                newNode.Previous = Current.Previous;
                Current.Previous = newNode;
                newNode.Next = Current;
                Size++;
            }
        }

        public void Clear()
        {
            while (!IsEmpty())
            {
                LLElement temp = First;
                if (First.Next != null)
                {
                    First.Next.Previous = null;
                }
                First = First.Next;
                temp = null;
                Size--;
            }
        }

        public void Display()
        {
            if (First == null)
            {
                Console.WriteLine("Empty(");
                return;
            }
            Current = First;
            int count = 0;
            while (Current != null)
            {
                Console.WriteLine("Node [{0}] = {1}", count, Current.Value);
                count++;
                Current = Current.Next;
            }
            Console.WriteLine();
        }

        public void DeleteAt(int index)
        {
            if (index < 0 || index > Size && Size != 0)
            {
                throw new InvalidOperationException();
            }
            else if (index == 0)
            {
                if (First == null)
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    LLElement temp = First;
                    if (First.Next != null)
                    {
                        First.Next.Previous = null;
                    }
                    First = First.Next;
                    temp = null;
                    Size--;
                }
            }
            else if (index == Size - 1)
            {
                if (Last == null)
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    LLElement temp = Last;
                    if (Last.Previous != null)
                    {
                        Last.Previous.Next = null;
                    }
                    Last = Last.Previous;
                    temp = null;
                    Size--;
                }
            }
            else
            {
                int count = 0;
                Current = First;
                while (Current != null && count != index)
                {
                    Current = Current.Next;
                    count++;
                }
                Current.Previous.Next = Current.Next;
                Current.Next.Previous = Current.Previous;
                Size--;
            }
        }

        public dynamic GetValue(int index)
        {
            Current = First;
            int count = 0;
            while (Current != null && count != index)
            {
                Current = Current.Next;
                count++;
            }

            if (Current != null)
            {
                return Current.Value;
            }
            else
            {
                return 0;
            }
        }

        public int GetIndex(dynamic value)
        {
            Current = First;
            int index = 0;
            while (Current != null && Current.Value != value)
            {
                Current = Current.Next;
                index++;
            }

            if (index != Size)
            {
                return index;
            }
            else
            {
                throw new IndexOutOfRangeException("Нет такого значения");
            }
        }
        
        public int Count()
        {
            return Size;
        }
    }
}