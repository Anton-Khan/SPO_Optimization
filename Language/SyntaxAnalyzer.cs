using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Language
{
    class SyntaxAnalyzer
    {

        
        
        private List<Token> POLIS;
        private List<Token> tokens;
        private int pointer;
        private Stack<Token> stack;

        private int bracketIndex;

        public SyntaxAnalyzer()
        {
            POLIS = new List<Token>();
            stack = new Stack<Token>();
        }

        public List<Token> convert(List<Token> tokens)
        {
            this.tokens = tokens;

            POLIS.Clear();
            stack.Clear();
            pointer = 0;

            expression();

            return POLIS;
        }

        private void expression()
        {
            while (simpleExpression()) { }
            POLIS.Add(new Token(Lexem.END, "$"));
        }

        private bool simpleExpression()
        {
            Lexem currentLexem = tokens[pointer].lexem;

            if (currentLexem == Lexem.DIGIT || currentLexem == Lexem.VAR)
            {
                POLIS.Add(tokens[pointer]);
                pointer++;

                CustomFunction();
            }
            else if (currentLexem == Lexem.OP || currentLexem == Lexem.ASSIGN_OP || currentLexem == Lexem.COMPARE_OP)
            {
                while (stack.Count != 0 && compareOperators(tokens[pointer], stack.Peek()))
                {
                    POLIS.Add(stack.Pop());
                }

                stack.Push(tokens[pointer]);
                pointer++;
            }
            else if (currentLexem == Lexem.L_B)
            {
                bracketIndex++;
                stack.Push(tokens[pointer]);
                pointer++;
            }
            else if (currentLexem == Lexem.R_B)
            {
                bracketIndex--;

                while (stack.Count != 0 && stack.Peek().lexem != Lexem.L_B)
                {
                    POLIS.Add(stack.Pop());
                }

                stack.Pop();
                pointer++;
            }
            else if (currentLexem == Lexem.DISPLAY_KW)
            {
                DisplayFunc();
            }
            else if (currentLexem == Lexem.END)
            {
                freeStack();
                return false;
            }
            else if (currentLexem == Lexem.RETURN_KW)
            {
                stack.Push(tokens[pointer]);
                pointer++;
            }
            else if (currentLexem == Lexem.IF_KW)
            {
                ifExpression();
            }
            else if (currentLexem == Lexem.WHILE_KW)
            {
                whileExpression();
            }
            else if (currentLexem == Lexem.OUT_KW)
            {
                pointer++;
                stack.Push(tokens[pointer]);
                pointer++;
                while (tokens[pointer].lexem != Lexem.SEMICOLON)
                {
                    simpleExpression();
                }
                POLIS.Add(new Token(Lexem.FUNC, "<-->"));
                POLIS.Add(new Token(Lexem.OUT_KW, "out"));
                pointer++;
            }
            else if (currentLexem == Lexem.SEMICOLON)
            {
                freeStack();
                pointer++;
            }
            else if (currentLexem == Lexem.DOT)
            {
                functionExpression();
            }
            else if (currentLexem == Lexem.LIST_KW || currentLexem == Lexem.HT_KW)
            {
                POLIS.Add(tokens[pointer]);
                while (tokens[pointer].lexem != Lexem.VAR)
                {
                    pointer++;
                }
                POLIS.Add(tokens[pointer]);
                pointer++;
            }
            else if (currentLexem == Lexem.L_SB)
            {
                pointer++;
            }

            return true;
        }

        private void functionExpression()
        {
            Token var = tokens[pointer - 1];
            Token func = tokens[pointer + 1];
            POLIS.RemoveAt(POLIS.Count - 1);
            pointer += 2;
            stack.Push(tokens[pointer]);
            pointer++;

            if (func.lexem == Lexem.INSERT_KW)
            {
                while (tokens[pointer].lexem != Lexem.COMMA_KW)
                {
                    simpleExpression();
                }
                while (stack.Peek().lexem != Lexem.L_B)
                {
                    POLIS.Add(stack.Pop());
                }
                pointer++;

                while (tokens[pointer].lexem != Lexem.SEMICOLON)
                {
                    simpleExpression();
                }
                pointer++;
            }
            else
            {
                bracketIndex = 1;

                while (bracketIndex != 0)
                {
                    simpleExpression();
                }
            }

            POLIS.Add(var);
            POLIS.Add(new Token(Lexem.FUNC, "<-->"));
            POLIS.Add(func);
        }

        private void freeStack()
        {
            while (stack.Count != 0)
            {
                POLIS.Add(stack.Pop());
            }
        }

        private void ifExpression()
        {
            conditionalExpression();

            int ifStartPosition = POLIS.Count;
            POLIS.Add(new Token(Lexem.TRANS_LBL, ""));
            POLIS.Add(new Token(Lexem.F_T, "!F"));

            innerExpression();
            /*
            while (tokens[pointer].lexem == Lexem.SPC)
            {
                pointer++;
            }
            */
            if (tokens[pointer].lexem == Lexem.ELSE_KW)
            {
                pointer++;
                int elseStartPosition = POLIS.Count;
                POLIS.Add(new Token(Lexem.TRANS_LBL, ""));
                POLIS.Add(new Token(Lexem.UNC_T, "!"));

                POLIS[ifStartPosition].value = POLIS.Count.ToString();

                innerExpression();

                POLIS[elseStartPosition].value = POLIS.Count.ToString();
            }
            else
            {
                POLIS[ifStartPosition].value = POLIS.Count.ToString();
            }
        }

        private void whileExpression()
        {
            int startingPosition = POLIS.Count;

            conditionalExpression();

            int endPosition = POLIS.Count;
            POLIS.Add(new Token(Lexem.TRANS_LBL, ""));
            POLIS.Add(new Token(Lexem.F_T, "!F"));

            innerExpression();

            POLIS.Add(new Token(Lexem.TRANS_LBL, startingPosition.ToString()));
            POLIS.Add(new Token(Lexem.UNC_T, "!"));

            POLIS[endPosition].value = POLIS.Count.ToString();
        }

        private void forExpression()
        {
            int startPosition;
            int conditionTransition;
            int bodyTransition;
            int iterationPosition;

            while (tokens[pointer].lexem != Lexem.L_B)
            {
                pointer++;
            }
            pointer++;

            // for (i = 0;
            while (tokens[pointer].lexem != Lexem.SEMICOLON)
            {
                simpleExpression();
            }
            freeStack();
            pointer++;

            startPosition = POLIS.Count;

            // i < n;
            while (tokens[pointer].lexem != Lexem.SEMICOLON)
            {
                simpleExpression();
            }
            freeStack();
            pointer++;

            conditionTransition = POLIS.Count;
            POLIS.Add(new Token(Lexem.TRANS_LBL, ""));
            POLIS.Add(new Token(Lexem.F_T, "!F"));

            bodyTransition = POLIS.Count;
            POLIS.Add(new Token(Lexem.TRANS_LBL, ""));
            POLIS.Add(new Token(Lexem.UNC_T, "!"));

            // i = i + 1)
            iterationPosition = POLIS.Count;
            stack.Push(new Token(Lexem.L_B, "("));
            bracketIndex = 1;
            while (bracketIndex != 0)
            {
                simpleExpression();
            }

            POLIS.Add(new Token(Lexem.TRANS_LBL, startPosition.ToString()));
            POLIS.Add(new Token(Lexem.UNC_T, "!"));
            POLIS[bodyTransition].value = POLIS.Count.ToString();

            // { body }
            while (tokens[pointer].lexem != Lexem.R_SB)
            {
                simpleExpression();
            }
            pointer++;

            POLIS.Add(new Token(Lexem.TRANS_LBL, iterationPosition.ToString()));
            POLIS.Add(new Token(Lexem.UNC_T, "!"));
            POLIS[conditionTransition].value = POLIS.Count.ToString();
        }

        private void conditionalExpression()
        {
            pointer++;
            while (tokens[pointer].lexem != Lexem.L_SB)
            {
                simpleExpression();
            }
        }

        private void innerExpression()
        {
            pointer++;
            while (tokens[pointer].lexem != Lexem.R_SB)
            {
                simpleExpression();
            }
            freeStack();
            pointer++;
        }

        private void CustomFunction()
        {
            if (tokens[pointer].lexem == Lexem.L_B && tokens[pointer - 1].lexem == Lexem.VAR)
            {
                var func_name = POLIS.Last();
                POLIS.RemoveAt(POLIS.Count - 1);
                int pointer2 = pointer + 1;
                if (tokens[pointer2].lexem != Lexem.R_B)
                {
                    while (tokens[pointer2].lexem != Lexem.R_B)
                    {
                        if (tokens[pointer2].lexem == Lexem.DIGIT || tokens[pointer2].lexem == Lexem.VAR)
                        {
                            POLIS.Add(tokens[pointer2]);
                            //if (tokens[pointer2].lexem == Lexem.VAR && tokens[pointer2 + 1].lexem == Lexem.L_B)
                            //{
                            //    pointer++;
                            //    CustomFunction();
                            //}
                            //  TO DO Функция как аргумент функции
                        }
                        pointer2++;
                    }
                    pointer = pointer2 + 1;
                }
                POLIS.Add(func_name);
                POLIS.Last().lexem = Lexem.FUNCTION_CALL;

            }
        }

        private void DisplayFunc()
        {

            var func_name = tokens[pointer];
            func_name.lexem = Lexem.SYSTEM_FUNC;
            pointer += 2;
            while (!(tokens[pointer].lexem == Lexem.R_B && tokens[pointer+1].lexem == Lexem.SEMICOLON))
            {
                if (tokens[pointer].lexem == Lexem.DIGIT || tokens[pointer].lexem == Lexem.VAR)
                {
                    POLIS.Add(tokens[pointer]);
                    pointer++;

                    CustomFunction();
                }
                else if (tokens[pointer].lexem == Lexem.OP )
                {
                    while (stack.Count != 0 && compareOperators(tokens[pointer], stack.Peek()))
                    {
                        POLIS.Add(stack.Pop());
                    }

                    stack.Push(tokens[pointer]);
                    pointer++;
                }
                else if (tokens[pointer].lexem == Lexem.L_B)
                {
                    bracketIndex++;
                    stack.Push(tokens[pointer]);
                    pointer++;
                }
                else if (tokens[pointer].lexem == Lexem.R_B)
                {
                    bracketIndex--;

                    while (stack.Count != 0 && stack.Peek().lexem != Lexem.L_B)
                    {
                        POLIS.Add(stack.Pop());
                    }

                    stack.Pop();
                    pointer++;
                }
            }
            freeStack();
            POLIS.Add(func_name);
            pointer+=2;
            


        }
        private bool compareOperators(Token op1, Token op2)
        {
            int op1Weight = 0;
            int op2Weight = 0;

            switch (op1.value)
            {
                case "=": op1Weight = 1; break;
                case ">": op1Weight = 1; break;
                case "<": op1Weight = 1; break;
                case ">=": op1Weight = 1; break;
                case "<=": op1Weight = 1; break;
                case "!=": op1Weight = 1; break;
                case "==": op1Weight = 1; break;
                case "+": op1Weight = 2; break;
                case "-": op1Weight = 2; break;
                case "*": op1Weight = 5; break;
                case "/": op1Weight = 4; break;
            }

            switch (op2.value)
            {
                case "=": op2Weight = 1; break;
                case ">": op1Weight = 1; break;
                case "<": op1Weight = 1; break;
                case ">=": op1Weight = 1; break;
                case "<=": op1Weight = 1; break;
                case "!=": op1Weight = 1; break;
                case "==": op1Weight = 1; break;
                case "+": op2Weight = 2; break;
                case "-": op2Weight = 2; break;
                case "*": op2Weight = 5; break;
                case "/": op2Weight = 4; break;
            }

            return op1Weight <= op2Weight;
        }
     
    }
}
