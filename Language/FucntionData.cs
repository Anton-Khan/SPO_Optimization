using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Language
{
    public class FucntionData  : ICloneable
    {
        public FucntionData()
        {
            ParamsRaw = new List<Token>();
            Body = new List<Token>();
            LocalVariables = new Dictionary<string, string>();
        }

        public string Name { get; set; }
        public List<Token> ParamsRaw { get; set; }

        public List<Token> Body { get; set; }
        
        public (int, int) BeginEnd { get; set; }
        
        public List<Token> Params { get; set; }
        
        public List<Token> Poliz { get; set; }

        public List<Triad> Triads { get; set; }

        public Dictionary<String, String> LocalVariables { get; set; }

        public void CalculateParams()
        {
            Params = ParamsRaw.Where(x => x.lexem != Lexem.COMMA_KW).ToList();
        }

        public object Clone()
        {
            var a = new FucntionData();
            a.BeginEnd = this.BeginEnd;
            a.Body = this.Body.ToArray().ToList();
            a.LocalVariables = Util.CloneDictionaryCloningValues(this.LocalVariables);///////
            a.Name = this.Name.Clone() as string;
            a.Params = Util.CloneList(this.Params).ToList();
            a.ParamsRaw = Util.CloneList(this.ParamsRaw).ToList();
            a.Poliz = Util.CloneList(this.Poliz).ToList(); ;
            a.Triads = Util.CloneList(this.Triads).ToList(); ;
            return a;
        }
    }
}
