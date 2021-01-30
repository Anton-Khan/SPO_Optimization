using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Language
{
    class LLElement
    {
        public dynamic Value { get; set; }
        public LLElement Next { get; set; }
        public LLElement Previous { get; set; }

        public LLElement(dynamic value)
        {
            Value = value;
        }
    }
}

