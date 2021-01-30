using System;
using System.Collections.Generic;
using System.Text;

namespace Language
{
    class HTElement
    {
        public int Key { get; private set; }
        public double Value { get; private set; }

        public HTElement(int key, double value)
        {
            Key = key;
            Value = value;
        }

        public override string ToString()
        {
            return String.Format("{0}: {1}", Key, Value);
        }
    }
}