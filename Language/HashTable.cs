using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Language
{
    class HashTable
    {
        private readonly int tableSize = 300;

        private LinkedList[] Table;

        public HashTable()
        {
            Table = new LinkedList[tableSize];
        }

        public double Search(int key)
        {
            int index = getHash(key);

            if (Table[index] != null)
            {
                for (int i = 0; i < Table[index].Size; i++)
                {
                    if (Table[index].GetValue(i).Key == key)
                    {
                        return Table[index].GetValue(i).Value;
                    }
                }
            }

            throw new ArgumentException($"Элемент с ключом {key} не найден(", nameof(key));
        }

        public void Delete(int key)
        {
            int index = getHash(key);

            if (Table[index] == null)
                return;

            for (int i = 0; i < Table[index].Size; i++)
            {
                if (Table[index].GetValue(i).Key == key)
                {
                    Table[index].DeleteAt(i);
                    break;
                }
            }
        }
        

        public void Display()
        {
            Console.WriteLine("Hash\tkey-value\tetc->");
            for (int i = 0; i < Table.Length; i++)
            {
                if (Table[i] == null || Table[i].Size == 0) continue;

                Console.Write("{0}\t", i);
                for (int j = 0; j < Table[i].Size; j++)
                {
                    Console.Write("{0}:{1}\t", Table[i].GetValue(j).Key, Table[i].GetValue(j).Value);
                }
                Console.WriteLine();
                
            }
            Console.WriteLine();
        }

        public void Insert(int key, double value)
        {
            HTElement newItem = new HTElement(key, value);

            int index = getHash(newItem.Key);

            if (Table[index] != null)
            {
                bool isContkey = false;

                for (int i = 0; i < Table[index].Size; i++)
                {
                    if (Table[index].GetValue(i).Key == key)
                    {
                        isContkey = true;
                        break;
                    }
                }

                if (isContkey)
                {
                    throw new ArgumentException($"Элемент с ключом {key} уже существует.", nameof(key));
                }

                Table[index].InsertAt(newItem, Table[index].Size);
            }
            else
            {
                Table[index] = new LinkedList();
                Table[index].InsertAt(newItem, 0);
            }
        }

        private int getHash(int key)
        {
            return key % tableSize;
        }
    }
}