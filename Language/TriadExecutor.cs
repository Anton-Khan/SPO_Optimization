using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Language
{
    class TriadExecutor
    {
        private List<Lexem> _operators;
        private Dictionary<String, String> global_variables;
        private Dictionary<String, String> variables;
        private Dictionary<String, FucntionData> functions;
        private FucntionData This;
        private List<Triad> Triads;

        public TriadExecutor(Dictionary<string, string> v, Dictionary<string, FucntionData> funct, List<Triad> triads, FucntionData t)
        {
            _operators = new List<Lexem> { Lexem.ASSIGN_OP, Lexem.COMPARE_OP, Lexem.OP, Lexem.F_T, Lexem.UNC_T, Lexem.DOT, Lexem.FUNCTION_CALL, Lexem.RETURN_KW, Lexem.SYSTEM_FUNC };
            global_variables = v;
            functions = funct;
            Triads = triads;
            This = t;
            variables = This.LocalVariables;
            foreach (var item in This.Params)
            {
                if (!variables.ContainsKey(item.value))
                    variables.Add(item.value, "");
            }

        }


        public string Exucute()
        {
            string main_result = "";
            var first_triads = Util.CloneList(Triads).ToList();
            for (int i = 0; i < Triads.Count; i++)
            {
                
                Console.WriteLine(This.Name);
                Util.ShowTriads(Triads,i, ConsoleColor.Green);
                if (_operators.Contains(Triads[i].Operator.lexem))
                {
                    if (Triads[i].Operator.lexem == Lexem.FUNCTION_CALL)
                    {
                        if (functions.ContainsKey(Triads[i].Operator.value))
                        {

                            var exec = new TriadExecutor(Util.CloneDictionaryCloningValues(global_variables), Util.CloneDictionaryCloningValues(functions), Util.CloneList(functions[Triads[i].Operator.value].Triads).ToList(), functions[Triads[i].Operator.value].Clone() as FucntionData);
                            //var exec = new TriadExecutor(This.LocalVariables, functions, functions[Triads[i].Operator.value].Triads, functions[Triads[i].Operator.value]);
                            for (int j = 0; j < Triads[i].Params.Count; j++)
                            {
                                if (exec.variables.ContainsKey(exec.functions[Triads[i].Operator.value].Params[j].value))
                                {
                                    if (Triads[i].Params[j].lexem == Lexem.VAR)
                                    {
                                        if (variables.ContainsKey(Triads[i].Params[j].value))
                                        {
                                            exec.variables[exec.functions[Triads[i].Operator.value].Params[j].value] = variables[Triads[i].Params[j].value];
                                        }
                                        else if (global_variables.ContainsKey(Triads[i].Params[j].value))
                                        {
                                            exec.variables[exec.functions[Triads[i].Operator.value].Params[j].value] = global_variables[Triads[i].Params[j].value];
                                        }
                                        else
                                        {
                                            throw new Exception("ad");
                                        }

                                    }
                                    else if (Triads[i].Params[j].lexem == Lexem.DIGIT)
                                    {
                                        exec.variables[exec.functions[Triads[i].Operator.value].Params[j].value] = Triads[i].Params[j].value;
                                    }

                                }
                            }
                            
                            var result = exec.Exucute();
                                                                                                                        ///////////////////////////////////////////////////// ХЗ НУЖНО ПРОВЕРЯТЬ 
                            
                            Triads.Insert(i+1, new Triad(new Token(Lexem.DIGIT, result.ToString()), new Token(Lexem.DIGIT, "0"), new Token(Lexem.CONST, "C")));
                            Triads.RemoveAt(i);
                            
                            Console.WriteLine();
                            
                        }
                        
                    }
                    if (Triads[i].Operator.lexem == Lexem.SYSTEM_FUNC)
                    {
                        if (Triads[i].Operator.value == Lexem.Display_FUNC.name)
                        {
                            string print = "";
                            if (Triads[i].Operand2.lexem == Lexem.TRIAD_LBL)
                            {
                                if (Triads[int.Parse(Triads[i].Operand2.value)].Operator.lexem == Lexem.CONST)
                                {
                                    print = Triads[int.Parse(Triads[i].Operand2.value)].Operand1.value;
                                }
                            }
                            else if (Triads[i].Operand2.lexem == Lexem.VAR)
                            {
                                if (variables.ContainsKey(Triads[i].Operand2.value))
                                {
                                    print = variables[Triads[i].Operand2.value];
                                }
                                else if (global_variables.ContainsKey(Triads[i].Operand2.value))
                                {
                                    print = global_variables[Triads[i].Operand2.value];
                                }
                                else
                                {
                                    throw new Exception("Missing VARIABLE"); // TODO Missing VARIABLE
                                }
                            }
                            else if (Triads[i].Operand2.lexem == Lexem.DIGIT)
                            {
                                print = Triads[i].Operand2.value;
                            }


                            Console.BackgroundColor = ConsoleColor.DarkYellow;
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            Console.WriteLine(print);
                            Console.ResetColor();
                        }
                    }
                    if (Triads[i].Operator.lexem == Lexem.F_T)
                    {
                        if (Triads[i].Operand1.lexem == Lexem.BOOL)
                        {
                            if (!bool.Parse(Triads[i].Operand1.value))
                            {
                                i = int.Parse(Triads[i].Operand2.value);
                            }
                        }
                        else if (Triads[i].Operand1.lexem == Lexem.TRIAD_LBL)
                        {
                            if (Triads[int.Parse(Triads[i].Operand1.value)].Operator.lexem == Lexem.CONST)
                            {
                                if (!bool.Parse(Triads[int.Parse(Triads[i].Operand1.value)].Operand1.value))
                                {
                                    i = int.Parse(Triads[i].Operand2.value);
                                }
                            }
                        }
                    }
                    if (Triads[i].Operator.lexem == Lexem.UNC_T)
                    {
                        if (Triads[i].Operand2.lexem == Lexem.TRIAD_LBL)
                        {
                            i = int.Parse(Triads[i].Operand2.value);
                            Triads = Util.CloneList(first_triads).ToList();
                        }
                        
                    }
                    if (Triads[i].Operator.lexem == Lexem.COMPARE_OP)
                    {
                        bool r = false;

                        if (Triads[i].Operand1.lexem == Lexem.DIGIT)
                        {
                            if (Triads[i].Operand2.lexem == Lexem.DIGIT)
                            {
                                r = Util.executeLogicOperation(Double.Parse(Triads[i].Operand1.value), Double.Parse(Triads[i].Operand2.value), Triads[i].Operator.value);
                            }
                            else if (variables.ContainsKey(Triads[i].Operand2.value))
                            {
                                r = Util.executeLogicOperation(Double.Parse(Triads[i].Operand1.value), Double.Parse(variables[Triads[i].Operand2.value]), Triads[i].Operator.value);
                            }
                            else if (global_variables.ContainsKey(Triads[i].Operand2.value))
                            {
                                r = Util.executeLogicOperation(Double.Parse(Triads[i].Operand1.value), Double.Parse(global_variables[Triads[i].Operand2.value]), Triads[i].Operator.value);
                            }
                            else
                            {
                                throw new Exception("Missing VARIABLE"); // TODO Missing VARIABLE
                            }
                        }
                        else if (Triads[i].Operand1.lexem == Lexem.VAR)
                        {
                            if (Triads[i].Operand2.lexem == Lexem.DIGIT)
                            {
                                if (variables.ContainsKey(Triads[i].Operand1.value))
                                {
                                    r = Util.executeLogicOperation(Double.Parse(variables[Triads[i].Operand1.value]), Double.Parse(Triads[i].Operand2.value), Triads[i].Operator.value);
                                }
                                else if(global_variables.ContainsKey(Triads[i].Operand1.value))
                                {
                                    r = Util.executeLogicOperation(Double.Parse(global_variables[Triads[i].Operand1.value]), Double.Parse(Triads[i].Operand2.value), Triads[i].Operator.value);
                                }
                                else
                                {
                                    throw new Exception("Missing VARIABLE"); // TODO Missing VARIABLE
                                }
                            }
                            else if (Triads[i].Operand2.lexem == Lexem.VAR)//variables.ContainsKey(Triads[i].Operand1.value))
                            {
                                if (variables.ContainsKey(Triads[i].Operand2.value))
                                {
                                    r = Util.executeLogicOperation(Double.Parse(variables[Triads[i].Operand1.value]), Double.Parse(variables[Triads[i].Operand2.value]), Triads[i].Operator.value);
                                }
                                else if (global_variables.ContainsKey(Triads[i].Operand2.value))
                                {
                                    r = Util.executeLogicOperation(Double.Parse(variables[Triads[i].Operand1.value]), Double.Parse(global_variables[Triads[i].Operand2.value]), Triads[i].Operator.value);
                                }
                                else
                                {
                                    throw new Exception("Missing VARIABLE"); // TODO Missing VARIABLE
                                }
                            }
                            else if (Triads[i].Operand2.lexem == Lexem.TRIAD_LBL)
                            {
                                if (Triads[int.Parse(Triads[i].Operand2.value)].Operator.lexem == Lexem.CONST)
                                {
                                    r = Util.executeLogicOperation(Double.Parse(variables[Triads[i].Operand1.value]), Double.Parse(Triads[int.Parse(Triads[i].Operand2.value)].Operand1.value), Triads[i].Operator.value);
                                }
                            }
                            else if (global_variables.ContainsKey(Triads[i].Operand1.value))
                            {
                                if (variables.ContainsKey(Triads[i].Operand2.value))
                                {
                                    r = Util.executeLogicOperation(Double.Parse(global_variables[Triads[i].Operand1.value]), Double.Parse(variables[Triads[i].Operand2.value]), Triads[i].Operator.value);
                                }
                                else if (global_variables.ContainsKey(Triads[i].Operand2.value))
                                {
                                    r = Util.executeLogicOperation(Double.Parse(global_variables[Triads[i].Operand1.value]), Double.Parse(global_variables[Triads[i].Operand2.value]), Triads[i].Operator.value);
                                }
                                else
                                {
                                    throw new Exception("Missing VARIABLE"); // TODO Missing VARIABLE
                                }
                            }
                            else
                            {
                                throw new Exception("Missing VARIABLE"); // TODO Missing VARIABLE
                            }
                        }


                        var result = new Triad(new Token(Lexem.BOOL, r.ToString()),
                                new Token(Lexem.DIGIT, "0"),
                                new Token(Lexem.CONST, "C"));
                        Console.WriteLine();
                        Console.WriteLine("({0} {1} {2}) {3}", Triads[i].Operand1.value, Triads[i].Operand2.value, Triads[i].Operator.value, i);
                        Console.WriteLine("({0} {1} {2}) {3}", result.Operand1.value, result.Operand2.value, result.Operator.value, i);
                        Console.WriteLine();
                        Triads.RemoveAt(i);
                        Triads.Insert(i, result);
                        
                    }
                    if (Triads[i].Operator.lexem == Lexem.OP)
                    {
                        double r = 0;
                        if (Triads[i].Operand1.lexem == Lexem.DIGIT)
                        {
                            if (Triads[i].Operand2.lexem == Lexem.DIGIT)
                            {
                                r = Util.executeArithmeticOperation(Double.Parse(Triads[i].Operand1.value), Double.Parse(Triads[i].Operand2.value), Triads[i].Operator.value);
                            }
                            else if (variables.ContainsKey(Triads[i].Operand2.value))
                            {
                                r = Util.executeArithmeticOperation(Double.Parse(Triads[i].Operand1.value), Double.Parse(variables[Triads[i].Operand2.value]), Triads[i].Operator.value);
                            }
                            else if (global_variables.ContainsKey(Triads[i].Operand2.value))
                            {
                                r = Util.executeArithmeticOperation(Double.Parse(Triads[i].Operand1.value), Double.Parse(global_variables[Triads[i].Operand2.value]), Triads[i].Operator.value);
                            }
                            else
                            {
                                throw new Exception("Missing VARIABLE"); // TODO Missing VARIABLE
                            }
                        }
                        else if (Triads[i].Operand1.lexem == Lexem.VAR)
                        {
                            if (Triads[i].Operand2.lexem == Lexem.DIGIT)
                            {
                                if (variables.ContainsKey(Triads[i].Operand1.value))
                                {
                                    r = Util.executeArithmeticOperation(Double.Parse(variables[Triads[i].Operand1.value]), Double.Parse(Triads[i].Operand2.value), Triads[i].Operator.value);
                                }
                                else if (global_variables.ContainsKey(Triads[i].Operand1.value))
                                {
                                    r = Util.executeArithmeticOperation(Double.Parse(global_variables[Triads[i].Operand1.value]), Double.Parse(Triads[i].Operand2.value), Triads[i].Operator.value);
                                }
                                else
                                {
                                    throw new Exception("Missing VARIABLE"); // TODO Missing VARIABLE
                                }
                            }
                            else if (Triads[i].Operand2.lexem == Lexem.VAR)//variables.ContainsKey(Triads[i].Operand1.value))
                            {
                                if (variables.ContainsKey(Triads[i].Operand2.value))
                                {
                                    r = Util.executeArithmeticOperation(Double.Parse(variables[Triads[i].Operand1.value]), Double.Parse(variables[Triads[i].Operand2.value]), Triads[i].Operator.value);
                                }
                                else if (global_variables.ContainsKey(Triads[i].Operand2.value))
                                {
                                    r = Util.executeArithmeticOperation(Double.Parse(variables[Triads[i].Operand1.value]), Double.Parse(global_variables[Triads[i].Operand2.value]), Triads[i].Operator.value);
                                }
                                else
                                {
                                    throw new Exception("Missing VARIABLE"); // TODO Missing VARIABLE
                                }
                            }else if (Triads[i].Operand2.lexem == Lexem.TRIAD_LBL)
                            {
                                if (Triads[int.Parse(Triads[i].Operand2.value)].Operator.lexem == Lexem.CONST)
                                {
                                    r = Util.executeArithmeticOperation(Double.Parse(variables[Triads[i].Operand1.value]), Double.Parse(Triads[int.Parse(Triads[i].Operand2.value)].Operand1.value), Triads[i].Operator.value);
                                }
                            }
                            else if (global_variables.ContainsKey(Triads[i].Operand1.value))
                            {
                                if (variables.ContainsKey(Triads[i].Operand2.value))
                                {
                                    r = Util.executeArithmeticOperation(Double.Parse(global_variables[Triads[i].Operand1.value]), Double.Parse(variables[Triads[i].Operand2.value]), Triads[i].Operator.value);
                                }
                                else if (global_variables.ContainsKey(Triads[i].Operand2.value))
                                {
                                    r = Util.executeArithmeticOperation(Double.Parse(global_variables[Triads[i].Operand1.value]), Double.Parse(global_variables[Triads[i].Operand2.value]), Triads[i].Operator.value);
                                }
                                else
                                {
                                    throw new Exception("Missing VARIABLE"); // TODO Missing VARIABLE
                                }
                            }
                            else
                            {
                                throw new Exception("Missing VARIABLE"); // TODO Missing VARIABLE
                            }
                        }

                        var result = new Triad(new Token(Lexem.DIGIT, r.ToString("F")),
                                new Token(Lexem.DIGIT, "0"),
                                new Token(Lexem.CONST, "C"));
                        Triads.RemoveAt(i);
                        Triads.Insert(i, result);
                    }
                    if (Triads[i].Operator.lexem == Lexem.ASSIGN_OP)
                    {

                        if (global_variables.ContainsKey(Triads[i].Operand1.value))
                        {
                            if (Triads[i].Operand2.lexem == Lexem.TRIAD_LBL)
                            {
                                if (Triads[int.Parse(Triads[i].Operand2.value)].Operator.lexem == Lexem.CONST)
                                {
                                    global_variables[Triads[i].Operand1.value] = Triads[int.Parse(Triads[i].Operand2.value)].Operand1.value;
                                }//////////////
                            }
                            else if (Triads[i].Operand2.lexem == Lexem.DIGIT)
                            {
                                global_variables[Triads[i].Operand1.value] = Triads[i].Operand2.value;
                            }
                        }
                        else if (variables.ContainsKey(Triads[i].Operand1.value))
                        {
                            if (Triads[i].Operand2.lexem == Lexem.TRIAD_LBL)
                            {
                                if (Triads[int.Parse(Triads[i].Operand2.value)].Operator.lexem == Lexem.CONST)
                                {
                                    variables[Triads[i].Operand1.value] = Triads[int.Parse(Triads[i].Operand2.value)].Operand1.value;
                                }//////////////
                            }
                            else if (Triads[i].Operand2.lexem == Lexem.DIGIT)
                            {
                                variables[Triads[i].Operand1.value] = Triads[i].Operand2.value;
                            }
                        }
                        else
                        {
                            if (Triads[i].Operand2.lexem == Lexem.TRIAD_LBL)
                            {
                                if (Triads[int.Parse(Triads[i].Operand2.value)].Operator.lexem == Lexem.CONST)
                                {
                                    variables.Add(Triads[i].Operand1.value, Triads[int.Parse(Triads[i].Operand2.value)].Operand1.value);
                                }//////////////
                            }
                            else if (Triads[i].Operand2.lexem == Lexem.DIGIT)
                            {
                                variables.Add(Triads[i].Operand1.value, Triads[i].Operand2.value);
                            }
                        }
                            
                            


                        

                    }
                    if(Triads[i].Operator.lexem == Lexem.RETURN_KW)
                    {
                        if (Triads[i].Operand2.lexem == Lexem.TRIAD_LBL)
                        {
                            if (Triads[int.Parse(Triads[i].Operand2.value)].Operator.lexem == Lexem.CONST)
                            {
                                main_result = Triads[int.Parse(Triads[i].Operand2.value)].Operand1.value;
                            }
                        }else if (Triads[i].Operand2.lexem == Lexem.VAR)
                        {
                            if (variables.ContainsKey(Triads[i].Operand2.value))
                            {
                                main_result = variables[Triads[i].Operand2.value];
                            }
                            else if(global_variables.ContainsKey(Triads[i].Operand2.value))
                            {
                                main_result = global_variables[Triads[i].Operand2.value];
                            }
                            else
                            {
                                throw new Exception("Missing VARIABLE"); // TODO Missing VARIABLE
                            }
                        }
                        else if (Triads[i].Operand2.lexem == Lexem.DIGIT)
                        {
                            main_result = Triads[i].Operand2.value;
                        }


                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        return main_result;
                        
                    }

                }
                Console.WriteLine(This.Name);
                Util.ShowTriads(Triads,i,ConsoleColor.Red);
                
                Console.WriteLine("---------------     step        ---------------");
            }
            throw new Exception("Can't reach RETURN");
           // return main_result;

        }


    }
}
