using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Language
{
    public class Token : ICloneable
    {
        public Lexem lexem { get; set; }
        public String value { get; set; }

        public Token(Lexem lexem, string value)
        {
            var a = Lexem.Values.ToList().Where(x => x.name == lexem.name).ToList();
            a.AddRange(Lexem.Extra.ToList().Where(x => x.name == lexem.name));

            this.lexem =  a.Count() == 0 ? lexem : a.First();
            this.value = value;
        }

        public object Clone()
        {
            var a = new Token(this.lexem, value.Clone() as string);
            
            return a;
        }
    }
}
