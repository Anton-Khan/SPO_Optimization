using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Language
{
    public class TriadExecutor
    {
        private List<Lexem> _operators;
        private Dictionary<String, String> global_variables;
        private Dictionary<String, String> variables;
        private Dictionary<String, FucntionData> functions;
        public FucntionData This;
        private List<Triad> Triads;
        private List<Triad> first_triads;
        private int index;
        int current_step=0;
        private int step;
        public int priority = 0;

        public TriadExecutor()
        {
            _operators = new List<Lexem> { Lexem.ASSIGN_OP, Lexem.COMPARE_OP, Lexem.OP, Lexem.F_T, Lexem.UNC_T, Lexem.DOT, Lexem.FUNCTION_CALL, Lexem.RETURN_KW, Lexem.SYSTEM_FUNC };
        }

        public TriadExecutor(Dictionary<string, string> v, Dictionary<string, FucntionData> funct, List<Triad> triads, FucntionData t)
        {
            _operators = new List<Lexem> { Lexem.ASSIGN_OP, Lexem.COMPARE_OP, Lexem.OP, Lexem.F_T, Lexem.UNC_T, Lexem.DOT, Lexem.FUNCTION_CALL, Lexem.RETURN_KW, Lexem.SYSTEM_FUNC };
            global_variables = v;
            functions = funct;
            Triads = triads;
            first_triads = Util.CloneList(Triads).ToList();
            This = t;
            variables = This.LocalVariables;
            index = 0;
            foreach (var item in This.Params)
            {
                if (!variables.ContainsKey(item.value))
                    variables.Add(item.value, "");
            }

        }

        public TriadExecutor GetContext()
        {
            var a = new TriadExecutor();

            a._operators = _operators;
            a.global_variables = global_variables;
            a.variables = variables;
            a.functions = functions;
            a.This = This;
            a.Triads = Triads;
            a.first_triads = first_triads;
            a.index = index;
            a.priority = priority;
            a.current_step = 0;
            return a;
        }

        public void SetContext(TriadExecutor context, int s)
        {
            _operators = context._operators; 
             global_variables = context.global_variables;
            variables = context.variables;
            functions = context.functions;
            This = context.This;
            Triads = context.Triads;
            first_triads = context.first_triads;
            index = context.index;
            priority = context.priority;
            current_step = 0;
            step = s;//index + s;
        }


        public string Exucute()
        {
            string main_result = "";
            
            for (; index < Triads.Count; index++)
            {
                current_step++;
                if (current_step > step && This.Async == true)
                {
                    ThreadManager.AddContex(GetContext());
                    return "Wait";
                }
                Console.Write(This.Name + " :: ");
                Console.WriteLine(Triads[index].Operator.lexem + " -> " + Triads[index].Operator.value);
                //Util.ShowTriads(Triads,index, ConsoleColor.Green);
                if (_operators.Contains(Triads[index].Operator.lexem))
                {
                    if (Triads[index].Operator.lexem == Lexem.F_T)
                    {
                        if (Triads[index].Operand1.lexem == Lexem.BOOL)
                        {
                            if (!bool.Parse(Triads[index].Operand1.value))
                            {
                                index = int.Parse(Triads[index].Operand2.value);
                            }
                        }
                        else if (Triads[index].Operand1.lexem == Lexem.TRIAD_LBL)
                        {
                            if (Triads[int.Parse(Triads[index].Operand1.value)].Operator.lexem == Lexem.CONST)
                            {
                                if (!bool.Parse(Triads[int.Parse(Triads[index].Operand1.value)].Operand1.value))
                                {
                                    index = int.Parse(Triads[index].Operand2.value);
                                }
                            }
                        }
                    }
                    if (Triads[index].Operator.lexem == Lexem.UNC_T)
                    {
                        if (Triads[index].Operand2.lexem == Lexem.TRIAD_LBL)
                        {
                            index = int.Parse(Triads[index].Operand2.value);
                            Triads = Util.CloneList(first_triads).ToList();                                 //////////////////////////////////////////////////////////////////////////////////// Выключить, если есть оптимизация для циклов
                        }

                    }
                    if (Triads[index].Operator.lexem == Lexem.FUNCTION_CALL)
                    {
                        if (functions.ContainsKey(Triads[index].Operator.value))
                        {
                            if (functions[Triads[index].Operator.value].Async == false)
                            {


                                var exec = new TriadExecutor(Util.CloneDictionaryCloningValues(This.LocalVariables), Util.CloneDictionaryCloningValues(functions), Util.CloneList(functions[Triads[index].Operator.value].Triads).ToList(), functions[Triads[index].Operator.value].Clone() as FucntionData);
                                //var exec = new TriadExecutor(This.LocalVariables, functions, functions[Triads[i].Operator.value].Triads, functions[Triads[i].Operator.value]);
                                for (int j = 0; j < Triads[index].Params.Count; j++)
                                {
                                    if (exec.variables.ContainsKey(exec.functions[Triads[index].Operator.value].Params[j].value))
                                    {
                                        if (Triads[index].Params[j].lexem == Lexem.VAR)
                                        {
                                            if (variables.ContainsKey(Triads[index].Params[j].value))
                                            {
                                                exec.variables[exec.functions[Triads[index].Operator.value].Params[j].value] = variables[Triads[index].Params[j].value];
                                            }
                                            else if (global_variables.ContainsKey(Triads[index].Params[j].value))
                                            {
                                                exec.variables[exec.functions[Triads[index].Operator.value].Params[j].value] = global_variables[Triads[index].Params[j].value];
                                            }
                                            else
                                            {
                                                throw new Exception("ad");
                                            }

                                        }
                                        else if (Triads[index].Params[j].lexem == Lexem.DIGIT)
                                        {
                                            exec.variables[exec.functions[Triads[index].Operator.value].Params[j].value] = Triads[index].Params[j].value;
                                        }

                                    }
                                }

                                var result = exec.Exucute();
                                ///////////////////////////////////////////////////// ХЗ НУЖНО ПРОВЕРЯТЬ 

                                Triads.Insert(index + 1, new Triad(new Token(Lexem.DIGIT, result.ToString()), new Token(Lexem.DIGIT, "0"), new Token(Lexem.CONST, "C")));
                                Triads.RemoveAt(index);

                                Console.WriteLine();
                            }
                            else
                            {
                                var exec = new TriadExecutor(Util.CloneDictionaryCloningValues(This.LocalVariables), Util.CloneDictionaryCloningValues(functions), Util.CloneList(functions[Triads[index].Operator.value].Triads).ToList(), functions[Triads[index].Operator.value].Clone() as FucntionData);
                                
                                //var exec = new TriadExecutor(This.LocalVariables, functions, functions[Triads[i].Operator.value].Triads, functions[Triads[i].Operator.value]);
                                for (int j = 0; j < Triads[index].Params.Count; j++)
                                {
                                    if (exec.variables.ContainsKey(exec.functions[Triads[index].Operator.value].Params[j].value))
                                    {
                                        if (Triads[index].Params[j].lexem == Lexem.VAR)
                                        {
                                            if (variables.ContainsKey(Triads[index].Params[j].value))
                                            {
                                                exec.variables[exec.functions[Triads[index].Operator.value].Params[j].value] = variables[Triads[index].Params[j].value];
                                            }
                                            else if (global_variables.ContainsKey(Triads[index].Params[j].value))
                                            {
                                                exec.variables[exec.functions[Triads[index].Operator.value].Params[j].value] = global_variables[Triads[index].Params[j].value];
                                            }
                                            else
                                            {
                                                throw new Exception("ad");
                                            }

                                        }
                                        else if (Triads[index].Params[j].lexem == Lexem.DIGIT)
                                        {
                                            exec.variables[exec.functions[Triads[index].Operator.value].Params[j].value] = Triads[index].Params[j].value;
                                        }

                                    }
                                }
                                exec.priority = priority + 1;
                                ThreadManager.AddContex(exec.GetContext());
                                //return "Run Async";
                            }
                            
                        }
                        
                    }
                    else if (Triads[index].Operator.lexem == Lexem.SYSTEM_FUNC)
                    {
                        if (Triads[index].Operator.value == Lexem.Display_FUNC.name)
                        {
                            string print = "";
                            if (Triads[index].Operand2.lexem == Lexem.TRIAD_LBL)
                            {
                                if (Triads[int.Parse(Triads[index].Operand2.value)].Operator.lexem == Lexem.CONST)
                                {
                                    print = Triads[int.Parse(Triads[index].Operand2.value)].Operand1.value;
                                }
                            }
                            else if (Triads[index].Operand2.lexem == Lexem.VAR)
                            {
                                if (variables.ContainsKey(Triads[index].Operand2.value))
                                {
                                    print = variables[Triads[index].Operand2.value];
                                }
                                else if (global_variables.ContainsKey(Triads[index].Operand2.value))
                                {
                                    print = global_variables[Triads[index].Operand2.value];
                                }
                                else
                                {
                                    throw new Exception("Missing VARIABLE"); // TODO Missing VARIABLE
                                }
                            }
                            else if (Triads[index].Operand2.lexem == Lexem.DIGIT)
                            {
                                print = Triads[index].Operand2.value;
                            }


                            Console.BackgroundColor = ConsoleColor.DarkYellow;
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            Console.WriteLine(print);
                            Console.ResetColor();
                        }
                    }
                    else if (Triads[index].Operator.lexem == Lexem.COMPARE_OP)
                    {
                        bool r = false;

                        if (Triads[index].Operand1.lexem == Lexem.DIGIT)
                        {
                            if (Triads[index].Operand2.lexem == Lexem.DIGIT)
                            {
                                r = Util.executeLogicOperation(Double.Parse(Triads[index].Operand1.value), Double.Parse(Triads[index].Operand2.value), Triads[index].Operator.value);
                            }
                            else if (variables.ContainsKey(Triads[index].Operand2.value))
                            {
                                r = Util.executeLogicOperation(Double.Parse(Triads[index].Operand1.value), Double.Parse(variables[Triads[index].Operand2.value]), Triads[index].Operator.value);
                            }
                            else if (global_variables.ContainsKey(Triads[index].Operand2.value))
                            {
                                r = Util.executeLogicOperation(Double.Parse(Triads[index].Operand1.value), Double.Parse(global_variables[Triads[index].Operand2.value]), Triads[index].Operator.value);
                            }
                            else
                            {
                                throw new Exception("Missing VARIABLE"); // TODO Missing VARIABLE
                            }
                        }
                        else if (Triads[index].Operand1.lexem == Lexem.VAR)
                        {
                            if (Triads[index].Operand2.lexem == Lexem.DIGIT)
                            {
                                if (variables.ContainsKey(Triads[index].Operand1.value))
                                {
                                    r = Util.executeLogicOperation(Double.Parse(variables[Triads[index].Operand1.value]), Double.Parse(Triads[index].Operand2.value), Triads[index].Operator.value);
                                }
                                else if(global_variables.ContainsKey(Triads[index].Operand1.value))
                                {
                                    r = Util.executeLogicOperation(Double.Parse(global_variables[Triads[index].Operand1.value]), Double.Parse(Triads[index].Operand2.value), Triads[index].Operator.value);
                                }
                                else
                                {
                                    throw new Exception("Missing VARIABLE"); // TODO Missing VARIABLE
                                }
                            }
                            else if (Triads[index].Operand2.lexem == Lexem.VAR)//variables.ContainsKey(Triads[i].Operand1.value))
                            {
                                if (variables.ContainsKey(Triads[index].Operand2.value))
                                {
                                    r = Util.executeLogicOperation(Double.Parse(variables[Triads[index].Operand1.value]), Double.Parse(variables[Triads[index].Operand2.value]), Triads[index].Operator.value);
                                }
                                else if (global_variables.ContainsKey(Triads[index].Operand2.value))
                                {
                                    r = Util.executeLogicOperation(Double.Parse(variables[Triads[index].Operand1.value]), Double.Parse(global_variables[Triads[index].Operand2.value]), Triads[index].Operator.value);
                                }
                                else
                                {
                                    throw new Exception("Missing VARIABLE"); // TODO Missing VARIABLE
                                }
                            }
                            else if (Triads[index].Operand2.lexem == Lexem.TRIAD_LBL)
                            {
                                if (Triads[int.Parse(Triads[index].Operand2.value)].Operator.lexem == Lexem.CONST)
                                {
                                    r = Util.executeLogicOperation(Double.Parse(variables[Triads[index].Operand1.value]), Double.Parse(Triads[int.Parse(Triads[index].Operand2.value)].Operand1.value), Triads[index].Operator.value);
                                }
                            }
                            else if (global_variables.ContainsKey(Triads[index].Operand1.value))
                            {
                                if (variables.ContainsKey(Triads[index].Operand2.value))
                                {
                                    r = Util.executeLogicOperation(Double.Parse(global_variables[Triads[index].Operand1.value]), Double.Parse(variables[Triads[index].Operand2.value]), Triads[index].Operator.value);
                                }
                                else if (global_variables.ContainsKey(Triads[index].Operand2.value))
                                {
                                    r = Util.executeLogicOperation(Double.Parse(global_variables[Triads[index].Operand1.value]), Double.Parse(global_variables[Triads[index].Operand2.value]), Triads[index].Operator.value);
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
                        //Console.WriteLine();
                        //Console.WriteLine("({0} {1} {2}) {3}", Triads[index].Operand1.value, Triads[index].Operand2.value, Triads[index].Operator.value, index);
                        //Console.WriteLine("({0} {1} {2}) {3}", result.Operand1.value, result.Operand2.value, result.Operator.value, index);
                        //Console.WriteLine();
                        Triads.RemoveAt(index);
                        Triads.Insert(index, result);
                        
                    }
                    else if (Triads[index].Operator.lexem == Lexem.OP)
                    {
                        double r = 0;
                        if (Triads[index].Operand1.lexem == Lexem.DIGIT)
                        {
                            if (Triads[index].Operand2.lexem == Lexem.DIGIT)
                            {
                                r = Util.executeArithmeticOperation(Double.Parse(Triads[index].Operand1.value), Double.Parse(Triads[index].Operand2.value), Triads[index].Operator.value);
                            }
                            else if (variables.ContainsKey(Triads[index].Operand2.value))
                            {
                                r = Util.executeArithmeticOperation(Double.Parse(Triads[index].Operand1.value), Double.Parse(variables[Triads[index].Operand2.value]), Triads[index].Operator.value);
                            }
                            else if (global_variables.ContainsKey(Triads[index].Operand2.value))
                            {
                                r = Util.executeArithmeticOperation(Double.Parse(Triads[index].Operand1.value), Double.Parse(global_variables[Triads[index].Operand2.value]), Triads[index].Operator.value);
                            }
                            else
                            {
                                throw new Exception("Missing VARIABLE"); // TODO Missing VARIABLE
                            }
                        }
                        else if (Triads[index].Operand1.lexem == Lexem.VAR)
                        {
                            if (Triads[index].Operand2.lexem == Lexem.DIGIT)
                            {
                                if (variables.ContainsKey(Triads[index].Operand1.value))
                                {
                                    r = Util.executeArithmeticOperation(Double.Parse(variables[Triads[index].Operand1.value]), Double.Parse(Triads[index].Operand2.value), Triads[index].Operator.value);
                                }
                                else if (global_variables.ContainsKey(Triads[index].Operand1.value))
                                {
                                    r = Util.executeArithmeticOperation(Double.Parse(global_variables[Triads[index].Operand1.value]), Double.Parse(Triads[index].Operand2.value), Triads[index].Operator.value);
                                }
                                else
                                {
                                    throw new Exception("Missing VARIABLE"); // TODO Missing VARIABLE
                                }
                            }
                            else if (Triads[index].Operand2.lexem == Lexem.VAR)//variables.ContainsKey(Triads[i].Operand1.value))
                            {
                                if (variables.ContainsKey(Triads[index].Operand2.value))
                                {
                                    r = Util.executeArithmeticOperation(Double.Parse(variables[Triads[index].Operand1.value]), Double.Parse(variables[Triads[index].Operand2.value]), Triads[index].Operator.value);
                                }
                                else if (global_variables.ContainsKey(Triads[index].Operand2.value))
                                {
                                    r = Util.executeArithmeticOperation(Double.Parse(variables[Triads[index].Operand1.value]), Double.Parse(global_variables[Triads[index].Operand2.value]), Triads[index].Operator.value);
                                }
                                else
                                {
                                    throw new Exception("Missing VARIABLE"); // TODO Missing VARIABLE
                                }
                            }else if (Triads[index].Operand2.lexem == Lexem.TRIAD_LBL)
                            {
                                if (Triads[int.Parse(Triads[index].Operand2.value)].Operator.lexem == Lexem.CONST)
                                {
                                    r = Util.executeArithmeticOperation(Double.Parse(variables[Triads[index].Operand1.value]), Double.Parse(Triads[int.Parse(Triads[index].Operand2.value)].Operand1.value), Triads[index].Operator.value);
                                }
                            }
                            else if (global_variables.ContainsKey(Triads[index].Operand1.value))
                            {
                                if (variables.ContainsKey(Triads[index].Operand2.value))
                                {
                                    r = Util.executeArithmeticOperation(Double.Parse(global_variables[Triads[index].Operand1.value]), Double.Parse(variables[Triads[index].Operand2.value]), Triads[index].Operator.value);
                                }
                                else if (global_variables.ContainsKey(Triads[index].Operand2.value))
                                {
                                    r = Util.executeArithmeticOperation(Double.Parse(global_variables[Triads[index].Operand1.value]), Double.Parse(global_variables[Triads[index].Operand2.value]), Triads[index].Operator.value);
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
                        Triads.RemoveAt(index);
                        Triads.Insert(index, result);
                    }
                    else if (Triads[index].Operator.lexem == Lexem.ASSIGN_OP)
                    {

                        if (global_variables.ContainsKey(Triads[index].Operand1.value))
                        {
                            if (Triads[index].Operand2.lexem == Lexem.TRIAD_LBL)
                            {
                                if (Triads[int.Parse(Triads[index].Operand2.value)].Operator.lexem == Lexem.CONST)
                                {
                                    global_variables[Triads[index].Operand1.value] = Triads[int.Parse(Triads[index].Operand2.value)].Operand1.value;
                                }
                            }
                            else if (Triads[index].Operand2.lexem == Lexem.DIGIT)
                            {
                                global_variables[Triads[index].Operand1.value] = Triads[index].Operand2.value;
                            }
                        }
                        else if (variables.ContainsKey(Triads[index].Operand1.value))
                        {
                            if (Triads[index].Operand2.lexem == Lexem.TRIAD_LBL)
                            {
                                if (Triads[int.Parse(Triads[index].Operand2.value)].Operator.lexem == Lexem.CONST)
                                {
                                    variables[Triads[index].Operand1.value] = Triads[int.Parse(Triads[index].Operand2.value)].Operand1.value;
                                }//////////////
                            }
                            else if (Triads[index].Operand2.lexem == Lexem.DIGIT)
                            {
                                variables[Triads[index].Operand1.value] = Triads[index].Operand2.value;
                            }
                        }
                        else
                        {
                            if (Triads[index].Operand2.lexem == Lexem.TRIAD_LBL)
                            {
                                if (Triads[int.Parse(Triads[index].Operand2.value)].Operator.lexem == Lexem.CONST)
                                {
                                    variables.Add(Triads[index].Operand1.value, Triads[int.Parse(Triads[index].Operand2.value)].Operand1.value);
                                }//////////////
                            }
                            else if (Triads[index].Operand2.lexem == Lexem.DIGIT)
                            {
                                variables.Add(Triads[index].Operand1.value, Triads[index].Operand2.value);
                            }
                        }
                            
                           
                    }
                    if (Triads[index].Operator.lexem == Lexem.RETURN_KW)
                    {
                        if (Triads[index].Operand2.lexem == Lexem.TRIAD_LBL)
                        {
                            if (Triads[int.Parse(Triads[index].Operand2.value)].Operator.lexem == Lexem.CONST)
                            {
                                main_result = Triads[int.Parse(Triads[index].Operand2.value)].Operand1.value;
                            }
                        }else if (Triads[index].Operand2.lexem == Lexem.VAR)
                        {
                            if (variables.ContainsKey(Triads[index].Operand2.value))
                            {
                                main_result = variables[Triads[index].Operand2.value];
                            }
                            else if(global_variables.ContainsKey(Triads[index].Operand2.value))
                            {
                                main_result = global_variables[Triads[index].Operand2.value];
                            }
                            else
                            {
                                throw new Exception("Missing VARIABLE"); // TODO Missing VARIABLE
                            }
                        }
                        else if (Triads[index].Operand2.lexem == Lexem.DIGIT)
                        {
                            main_result = Triads[index].Operand2.value;
                        }


                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine();
                        return main_result;
                        
                    }
                    

                }
                //Console.WriteLine(This.Name);
                //Util.ShowTriads(Triads,index,ConsoleColor.Red);
                
                //Console.WriteLine("---------------     step        ---------------");
            }
            throw new Exception("Can't reach RETURN");
           // return main_result;

        }


    }
}
