using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Language
{
    public class TriadsBuilder
    {
        private List<Lexem> _operators;
        private Dictionary<int, int> _refferenceForward;
        private Dictionary<int, int> _refferenceBackward;
        private Dictionary<int, int> _polisToTriad;
        private Dictionary<String, String> _variables;
        private Dictionary<String, String> _v = new Dictionary<string, string>();
        private Dictionary<String, FucntionData> functions;
        List<Token> ChangedPolis;

        public List<Triad> Triads { get; private set; }


        public TriadsBuilder(Dictionary<String, String> var, Dictionary<String, FucntionData> funct)
        {
            _operators = new List<Lexem> { Lexem.ASSIGN_OP, Lexem.COMPARE_OP, Lexem.OP, Lexem.F_T, Lexem.UNC_T, Lexem.DOT, Lexem.FUNCTION_CALL, Lexem.RETURN_KW, Lexem.SYSTEM_FUNC };
            _refferenceForward = new Dictionary<int, int>();
            _refferenceBackward = new Dictionary<int, int>();
            _polisToTriad = new Dictionary<int, int>();
            Triads = new List<Triad>();
            _variables = var;
            functions = funct;
        }
        
        public void Build(List<Token> polis)
        {
            ChangedPolis = polis.ToList();
            var forward_index = 0;
            
            for (int i = 0; i < ChangedPolis.Count; i++)
            {
                if (_refferenceForward.ContainsKey(i+forward_index))
                {
                    Triads[_refferenceForward[i + forward_index]].Operand2 = new Token(Lexem.TRIAD_LBL, Triads.Count + "");
                    _refferenceForward.Remove(i + forward_index);
                }

                if (_operators.Contains(ChangedPolis[i].lexem))
                {
                    if (ChangedPolis[i].lexem == Lexem.F_T)
                    {
                        Triads.Add( new Triad( new Token(Lexem.TRIAD_LBL, Triads.Count-1+""), ChangedPolis[i-1], ChangedPolis[i]) );
                        for (int j = 0; j < 2; j++)
                        {
                            ChangedPolis.RemoveAt(i-j);
                        }
                        i--;
                        ChangedPolis.Insert(i, new Token(Lexem.TRIAD_LBL, (Triads.Count - 1) + ""));
                        forward_index++;
                        _polisToTriad.Add(i+forward_index, Triads.Count - 1);
                    }
                    else if (ChangedPolis[i].lexem == Lexem.UNC_T)
                    {
                        Triads.Add(new Triad(ChangedPolis[i - 1], ChangedPolis[i - 1], ChangedPolis[i]));
                        for (int j = 0; j < 2; j++)
                        {
                            ChangedPolis.RemoveAt(i-j);
                        }
                        i--;
                        ChangedPolis.Insert(i, new Token(Lexem.TRIAD_LBL, (Triads.Count - 1) + ""));
                        forward_index++;
                        _polisToTriad.Add(i + forward_index, Triads.Count - 1);
                    }
                    else if(ChangedPolis[i].lexem == Lexem.FUNCTION_CALL)
                    {
                        if (!functions.ContainsKey(ChangedPolis[i].value))
                        {
                            throw new Exception("Cant't find function, called: " + ChangedPolis[i].value);
                        }
                        var params_count = functions[ChangedPolis[i].value].Params.Count;
                        List<Token> _params = new List<Token>();
                        for (int j = params_count; j > 0; j--)
                        {
                            _params.Add(ChangedPolis[i - j]);
                        }
                        Triads.Add(new Triad(_params, ChangedPolis[i]));
                        for (int j = 0; j < params_count+1; j++)
                        {
                            ChangedPolis.RemoveAt(i - j);
                        }
                        i-=params_count;
                        ChangedPolis.Insert(i, new Token(Lexem.TRIAD_LBL, (Triads.Count - 1) + ""));
                        forward_index+=params_count;
                        _polisToTriad.Add(i + forward_index, Triads.Count - 1);

                    }
                    else if (ChangedPolis[i].lexem == Lexem.RETURN_KW)
                    {
                        Triads.Add(new Triad(new Token(Lexem.END, ""), ChangedPolis[i - 1], ChangedPolis[i]));
                        for (int j = 0; j < 2; j++)
                        {
                            ChangedPolis.RemoveAt(i - j);
                        }
                        i--;
                        ChangedPolis.Insert(i, new Token(Lexem.TRIAD_LBL, (Triads.Count - 1) + ""));
                        forward_index++;
                        _polisToTriad.Add(i + forward_index, Triads.Count - 1);
                    }
                    else if (ChangedPolis[i].lexem == Lexem.SYSTEM_FUNC)
                    {
                        Triads.Add(new Triad(new Token(Lexem.END, ""), ChangedPolis[i - 1], ChangedPolis[i]));
                        for (int j = 0; j < 2; j++)
                        {
                            ChangedPolis.RemoveAt(i - j);
                        }
                        i--;
                        ChangedPolis.Insert(i, new Token(Lexem.TRIAD_LBL, (Triads.Count - 1) + ""));
                        forward_index++;
                        _polisToTriad.Add(i + forward_index, Triads.Count - 1);
                    }
                    else
                    {
                        Triads.Add(new Triad(ChangedPolis[i - 2], ChangedPolis[i - 1], ChangedPolis[i]));
                        for (int j = 0; j < 3; j++)
                        {
                            ChangedPolis.RemoveAt(i-j);
                        }
                        i -= 2;
                        ChangedPolis.Insert(i, new Token(Lexem.TRIAD_LBL, (Triads.Count - 1)+""));
                        
                        
                        _polisToTriad.Add((i + forward_index), Triads.Count - 1);
                        forward_index += 2;
                    }

                }
                else if(ChangedPolis[i].lexem == Lexem.TRANS_LBL)
                {
                    if (Int32.Parse(ChangedPolis[i].value) >= i + forward_index)
                    {
                        _refferenceForward.Add(Int32.Parse(ChangedPolis[i].value), Triads.Count);
                    }
                    else
                    {
                        _refferenceBackward.Add(Int32.Parse(ChangedPolis[i].value), Triads.Count);
                    }
                    
                }
               
            }
            Console.WriteLine();
            Util.ShowTriads(Triads);
            Console.WriteLine();
            foreach (var key in _refferenceBackward.Keys.ToArray())
            {
                
                Triads[_refferenceBackward[key]].Operand2 = new Token(Lexem.TRIAD_LBL, _polisToTriad[key] + "");
            }

            //Triads.Add(new Triad(new Token(Lexem.END, "END"),
            //           new Token(Lexem.END, "END"),
            //           new Token(Lexem.END, "END")));
        }

        public void Analyze()
        {
            foreach (var item in Triads)
            {
                if (item.Operator.lexem == Lexem.ASSIGN_OP && !_variables.ContainsKey(item.Operand1.value))
                {
                    _variables.Add(item.Operand1.value, item.Operand2.value);
                }
            }
        }


        public void Optimize()
        {
            foreach (var triad in Triads.ToArray())
            {
                if (triad.Operator.lexem != Lexem.FUNCTION_CALL) {
                    if (triad.Operand1.lexem == Lexem.VAR && _variables.ContainsKey(triad.Operand1.value) && triad.Operator.lexem != Lexem.ASSIGN_OP)
                    {
                        triad.Operand1 = new Token(Lexem.DIGIT, _variables[triad.Operand1.value]);
                    }
                    if (triad.Operand2.lexem == Lexem.VAR && _variables.ContainsKey(triad.Operand2.value))
                    {
                        triad.Operand2 = new Token(Lexem.DIGIT, _variables[triad.Operand2.value]);
                    }

                    if (triad.Operand1.lexem == Lexem.TRIAD_LBL && Triads[Int32.Parse(triad.Operand1.value)].Operator.lexem == Lexem.CONST)
                    {
                        triad.Operand1 = new Token(Lexem.DIGIT, Triads[Int32.Parse(triad.Operand1.value)].Operand1.value);
                    }
                    if (triad.Operand2.lexem == Lexem.TRIAD_LBL && Triads[Int32.Parse(triad.Operand2.value)].Operator.lexem == Lexem.CONST && (triad.Operator.lexem != Lexem.UNC_T && triad.Operator.lexem != Lexem.F_T))
                    {
                        triad.Operand2 = new Token(Lexem.DIGIT, Triads[Int32.Parse(triad.Operand2.value)].Operand1.value);
                    }


                    if (triad.Operand1.lexem == Lexem.DIGIT && triad.Operand2.lexem == Lexem.DIGIT)
                    {
                        Triads[Triads.IndexOf(triad)] = ExecuteTriad(triad);
                    }

                    if (triad.Operator.lexem == Lexem.ASSIGN_OP)
                    {
                        if (triad.Operand2.lexem == Lexem.DIGIT)
                        {///////////////////
                            if (!_variables.ContainsKey(triad.Operand1.value))
                            {
                                _variables.Add(triad.Operand1.value, triad.Operand2.value);
                            }
                            else
                            {
                                _variables[triad.Operand1.value] = triad.Operand2.value;
                            }
                        }
                        else
                        {
                            if (_variables.ContainsKey(triad.Operand1.value))
                            {
                                _variables.Remove(triad.Operand1.value);
                            }
                        }/////////////////////////////////////
                    }
                }
            }
        }

        public void DeleteC()
        {
            
            List<Triad> reff1 = Triads.Where(x => x.Operator.lexem != Lexem.FUNCTION_CALL && x.Operand1.lexem == Lexem.TRIAD_LBL).ToList();
            List<Triad> reff2 = Triads.Where(x => x.Operator.lexem != Lexem.FUNCTION_CALL && x.Operand2.lexem == Lexem.TRIAD_LBL).ToList();
            List<Triad> result = Triads.ToList();


            
            for (int i = 0; i < Triads.Count; i++)
            {
                if (Triads[i].Operator.lexem == Lexem.CONST)
                {
                    var a = reff1.Where(x => x.Operand1.value == i + "");
                    if (a.Count() > 0)
                    {
                        for (int j = 0; j < a.Count(); j++)
                        {
                            if(a.ElementAt(j).Operator.lexem != Lexem.FUNCTION_CALL)
                                a.ElementAt(j).Operand1.value = (Int32.Parse(a.ElementAt(j).Operand1.value) + 1) + "";
                        }
                    }
                    var b = reff2.Where(x => x.Operand2.value == (i + ""));
                    if (b.Count() > 0)
                    {
                        for (int j = 0; j < b.Count(); j++)
                        {
                            if (b.ElementAt(j).Operator.lexem != Lexem.FUNCTION_CALL)
                                b.ElementAt(j).Operand2.value = (Int32.Parse(b.ElementAt(j).Operand2.value) + 1) + "";
                        }
                    }
                    result.Remove(Triads[i]);
                }
            }
            for (int i = 0; i < Triads.Count; i++)
            {
                int counterC = 0;
                int counterD = 0;
                if (Triads[i].Operator.lexem != Lexem.FUNCTION_CALL && Triads[i].Operand1.lexem == Lexem.TRIAD_LBL)
                {
                    if (Int32.Parse(Triads[i].Operand1.value) >= i)
                    {
                        for (int j = 0; j < Int32.Parse(Triads[i].Operand1.value); j++)
                        {
                            if (Triads[j].Operator.lexem == Lexem.CONST)
                            {
                                counterC++;
                            }
                        }
                        Triads[i].Operand1.value = (Int32.Parse(Triads[i].Operand1.value) - counterC) + "";
                    }
                    else
                    {
                        counterC = 0;
                        counterD = 0;
                        for (int j = Int32.Parse(Triads[i].Operand1.value); j < i; j++)
                        {
                            if (Triads[j].Operator.lexem == Lexem.CONST)
                            {
                                counterC++;
                            }
                        }
                        for (int j = 0; j < i; j++)
                        {
                            if (Triads[j].Operator.lexem == Lexem.CONST)
                            {
                                counterD++;
                            }
                        }
                        Triads[i].Operand1.value = (Int32.Parse(Triads[i].Operand1.value) - (counterD-counterC)) + "";
                    }
                }
                
                if (Triads[i].Operator.lexem != Lexem.FUNCTION_CALL && Triads[i].Operand2.lexem == Lexem.TRIAD_LBL)
                {
                    if (Int32.Parse(Triads[i].Operand2.value) >= i)
                    {
                        counterC = 0;
                        for (int j = 0; j < Int32.Parse(Triads[i].Operand2.value); j++)
                        {
                            if (Triads[j].Operator.lexem == Lexem.CONST)
                            {
                                counterC++;
                            }
                        }
                        Triads[i].Operand2.value = (Int32.Parse(Triads[i].Operand2.value) - counterC) + "";
                    }
                    else
                    {
                        counterC = 0;
                        counterD = 0;
                        for (int j = Int32.Parse(Triads[i].Operand2.value); j < i; j++)
                        {
                            if (Triads[j].Operator.lexem == Lexem.CONST)
                            {
                                counterC++;
                            }
                        }
                        for (int j = 0; j < i; j++)
                        {
                            if (Triads[j].Operator.lexem == Lexem.CONST)
                            {
                                counterD++;
                            }
                        }
                        Triads[i].Operand2.value = (Int32.Parse(Triads[i].Operand2.value) - (counterD - counterC)) + "";
                    }
                }
            }
            Triads = result;
            
        }

        private Triad ExecuteTriad(Triad t)
        {
            
            Triad result;
            if (t.Operator.lexem == Lexem.COMPARE_OP)
            {
                var r = Util.executeLogicOperation(Double.Parse(t.Operand1.value), Double.Parse(t.Operand2.value), t.Operator.value);
                result = t;
                    //= new Triad(new Token(Lexem.BOOL, r.ToString()),
                    //    new Token(Lexem.DIGIT, "0"),
                    //    new Token(Lexem.CONST, "C"));
            }
            else
            {
                var r = Util.executeArithmeticOperation(Double.Parse(t.Operand1.value), Double.Parse(t.Operand2.value), t.Operator.value);
                result = new Triad(new Token(Lexem.DIGIT, r.ToString()),
                        new Token(Lexem.DIGIT, "0"),
                        new Token(Lexem.CONST, "C"));
            }
            return result;

        }


    }
}
/*
 
 
 
 
 
 if(A>1){
	a=1;
	}
a=4;
}
C=3;


while(a > 1){

a = a + 1;
}

 






if(A * B + A * B * 345 > 4){
a = 5 + 4 * 7;

}
 










func ABS(){
	return 1;
}


A = 1;
B = 2;

func FourParamsFunc(A, B, C, a){
	return (a + A + B + C);
}

C=(A+B*2) + (1+2*2)*3;

a = 0;
a = 2 * Pow(A) + 1;


while(a < 12){

	a = a + 2;
	if(a > 20){
		A = 12;
	}
	if(a > 30){
		A = 24 * A + 32 * 1;
	}else{
		B=34;
	}
}

Q = FourParamsFunc(A, B, C, a);

func Pow(a){
	return (a * a);
}






 */