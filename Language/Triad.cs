using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Language
{
    public class Triad : ICloneable
    {
        private Token operand1;
        private Token operand2;

        public Token Operand1 { get => operand1; set { operand1 = value;  } } // Params[0] = value;
        public Token Operand2 { get => operand2; set { operand2 = value;  } }
        public Token Operator { get; set; }
        public List<Token> Params { get; set; }

        public Triad(Token op1, Token op2, Token _operator)
        {
            Params = new List<Token>();
            Params.Add(op1);
            Params.Add(op2);
            Operand1 = op1;
            Operand2 = op2;
            
            Operator = _operator;
        }
        public Triad(List<Token> p, Token _operator)
        {
            Params = p;
            Operator = _operator;
        }
        public Triad() {
            Params = new List<Token>();
            
        }

        public object Clone()
        {
            Triad a;
            if (this.Operand1 == null && Params.Count > 0)
            {
                a = new Triad(this.Params, this.Operator);
            }
            else
            {
                a = new Triad(this.Operand1, this.Operand2, this.Operator);
            }
            return a;


        }
    }
}
