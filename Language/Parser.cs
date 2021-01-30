using System;
using System.Collections.Generic;
using System.Text;


namespace Language
{
    class Parser
    {
        private readonly List<Token> tokens;
        private Stack<Token> Stack;
        private int iterator;

        Dictionary<String, FucntionData> functions;


        public Parser(List<Token> list, Dictionary<String, FucntionData> fun)
        {
            this.tokens = list;

            Stack = new Stack<Token>();

            iterator = 0;

            functions = fun;
        }

        public List<List<Token>> CutFunctions()
        {
            var result = new List<List<Token>>();
            int cuted_index = 0;
            foreach (var key in functions.Keys)
            {
                result.Add(functions[key].Body);
                tokens.RemoveRange(functions[key].BeginEnd.Item1-cuted_index, (functions[key].BeginEnd.Item2 - cuted_index) + 1 - (functions[key].BeginEnd.Item1 - cuted_index));
                cuted_index += functions[key].BeginEnd.Item2 - functions[key].BeginEnd.Item1+1;
            }
            result.Insert(0,tokens);
            int a = 0;
            foreach (var item in tokens)
            {
                Console.WriteLine(a + ") " + item.lexem + " :: "+ item.value);
                a++;
            }
            return result;

        }

        public void lang()
        {

            try
            {
                while (true)
                {
                    expr();
                }
            }
            catch (LangException e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
                Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


        }

        private void expr()
        {
            try
            {
                assignExpr();
            }
            catch (LangException ex)
            {
                try
                {
                    iterator--;
                    ifExpr();
                }
                catch (LangException exc)
                {
                    try
                    {
                        iterator--;
                        whileExpr();
                    }
                    catch (LangException excep)
                    {
                        try
                        {
                            iterator--;
                            new_list();
                        }
                        catch (LangException except)
                        {

                            try
                            {
                                iterator--;
                                new_ht();
                            }
                            catch (LangException exception)
                            {
                                try
                                {
                                    iterator--;
                                    _func();
                                }
                                catch (LangException hte)
                                {
                                    try
                                    {
                                        iterator--;
                                        _func();
                                    }
                                    catch (LangException _f)
                                    {
                                        try
                                        {
                                            iterator--;
                                            function();
                                        }
                                        catch(LangException func)
                                        {
                                            try
                                            {
                                                iterator--;
                                                display_value();
                                                SEMICOLON();
                                                
                                            }
                                            catch (LangException func_call)
                                            {
                                                try
                                                {
                                                    iterator--;
                                                    function_call();
                                                    SEMICOLON();
                                                }
                                                catch (LangException display_ex)
                                                {
                                                    //Console.WriteLine("\t\tНичего не подошло в EXPR или Конец EXPRESSION ->  " + (iterator - 1) + "\n\t\t\t" + ex.Message + "\n\t\t\t" + exc.Message + "\n\t\t\t" + excep.Message + "\n\t\t\t" + except.Message + "\n\t\t\t" + exception.Message + "\n\t\t\t" + hte.Message);
                                                    throw new LangException("Ничего не подошло в EXPR или Конец EXPRESSION ->" + (iterator - 1) + "\n\t\t\t" + ex.Message + "\n\t\t\t" + exc.Message + "\n\t\t\t" + excep.Message + "\n\t\t\t" + except.Message + "\n\t\t\t" + exception.Message + "\n\t\t\t" + hte.Message + "\n\t\t\t" + _f.Message + "\n\t\t\t" + func.Message + "\n\t\t\t" + func_call.Message + "\n\t\t\t" + display_ex.Message);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void display_value()
        {
            DISPLAY_KW();
            RIGHT_B();
            valueExpr();
            LEFT_B();
            
        }

        private void function()
        {
            
            FUNC_KW();
            int begin = iterator-1;
            VAR();
            int start = iterator-1;
            
            functions.Add(tokens[start].value, new FucntionData());
            functions[tokens[start].value].Name = tokens[start].value;
            f_head();
            for (int i = start+2; i < iterator-1; i++)
            {
                functions[tokens[start].value].ParamsRaw.Add(tokens[i]);
            }
            functions[tokens[start].value].CalculateParams();
            int endOfHead = iterator;
            f_body();
            for (int i = endOfHead+1; i < iterator-1; i++)
            {
                functions[tokens[start].value].Body.Add(tokens[i]);
            }
            functions[tokens[start].value].Body.Add(new Token(Lexem.END, "$"));
            functions[tokens[start].value].BeginEnd = (begin, iterator-1);
        }
        private void f_head()
        {
            LEFT_B();
            
            try
            {
                while (true)
                {
                    value();
                    COMMA_KW();
                }
            }
            catch (LangException)
            {
                iterator--;
                RIGHT_B();
                //Console.WriteLine("\n\t\tvalue_exp\n\t\t\t" + (iterator - 1) + "-> " + e.Message + "\n");

                return;
            }
            
            
        }

        private void f_body()
        {
            LEFT_SB();
            try
            {
                while (true)
                {
                    expr();
                }
            }
            catch (LangException )
            {
                try
                {
                    iterator--;
                    RETURN_KW();
                    value();
                    SEMICOLON();
                    RIGHT_SB();
                    
                }
                catch (LangException ex)
                {
                    //Console.WriteLine((iterator - 1) + "-> " + e.Message);
                    //Console.WriteLine((iterator - 1) + "-> Ожидалась {0}\n\t{1}", Lexem.R_SB, ex.Message);

                    throw new LangException((iterator - 1) + "-> Ожидалась " + Lexem.RETURN_KW + "\n\t" + ex.Message);
                }
            }
            
        }
        private void function_call()
        {
            
            f_head();
            
        }


        private void assignExpr()
        {
            VAR();
            ASSIGN_OP();
            valueExpr();
            SEMICOLON();

        }

        private void value()
        {
            try
            {
                VAR();

                try
                {
                    DOT();
                    list_get_func();
                }
                catch (LangException)
                {
                    iterator--;
                    try
                    {
                        function_call();
                    }
                    catch (LangException)
                    {
                        iterator--;
                        ////////////////////////////////////////////////////////////////////////////////////////////////

                    }
                }
            }
            catch (LangException )
            {
                try
                {
                    iterator--;
                    DIGIT();
                }
                catch (LangException )
                {
                    
                    try
                    {
                        iterator--;
                        bracketBody();
                    }
                    catch (Exception)
                    {

                        throw new LangException("Не обнаружилось ни VAR ни DIGIT ни (...) в VALUE -> " + (iterator - 1));
                    }

                        
                    

                }
            }

        }

        private void valueExpr()
        {

            value();

            try
            {
                while (true)
                {
                    OP();
                    value();
                }
            }
            catch (LangException )
            {
                iterator--;
                //Console.WriteLine("\n\t\tvalue_exp\n\t\t\t" + (iterator - 1) + "-> " + e.Message + "\n");
                return;
            }



        }

        private void bracketBody()
        {
            LEFT_B();
            valueExpr();
            RIGHT_B();
        }

        private void whileExpr()
        {
            WHILE_KW();
            whileHead();
            whileBody();
        }

        private void whileHead()
        {
            LEFT_B();
            logicComp();
            RIGHT_B();
        }

        private void whileBody()
        {
            sqBracketBody();
        }

        private void ifExpr()
        {
            IF_KW();
            if_head();
            if_body();

        }

        private void if_head()
        {
            LEFT_B();
            logicComp();
            RIGHT_B();
        }

        private void if_body()
        {
            sqBracketBody();

            try
            {
                while (true)
                {
                    else_expr();
                }
            }
            catch (LangException)
            {
                iterator--;
                return;
            }
        }

        private void else_expr()
        {
            ELSE_KW();

            try
            {
                ifExpr();
            }
            catch (LangException)
            {
                //xz
                iterator--;
                sqBracketBody();
            }


        }

        private void new_ht()
        {
            HT_KW();
            VAR();
            SEMICOLON();
        }

        private void search_func()
        {
            SEARCH_KW();
            func_body();
        }

        
        private void new_list()
        {
            LIST_KW();
            VAR();
            SEMICOLON();
        }

        private void _func()
        {
            
            DOT();
            l_func();
            SEMICOLON();
        }

        private void list_get_func()
        {
            try
            {
                get_value_func();
            }
            catch (LangException e)
            {
                try
                {
                    iterator--;
                    get_index_func();
                }
                catch (LangException ex)
                {

                    try
                    {
                        iterator--;
                        isEmpty_func();
                    }
                    catch (LangException exc)
                    {

                        try
                        {
                            iterator--;
                            count();
                        }
                        catch (LangException exce)
                        {
                            try
                            {
                                iterator--;
                                search_func();
                            }
                            catch (LangException excep)
                            { 
                                iterator--;

                                throw new LangException("Ничего не подошло в Get_Func ->" + (iterator) + "\n\t\t\t\t\t" + e.Message + "\n\t\t\t\t\t" + ex.Message + "\n\t\t\t\t\t" + exc.Message + "\n\t\t\t\t\t" + exce.Message + "\n\t\t\t\t\t" + excep.Message);
                            }
                        }
                    }
                }
            }
        }

        private void l_func()
        {

            try
            {

                clear_func();
            }
            catch (LangException ex)
            {
                try
                {
                    iterator--;
                    display_func();
                }
                catch (LangException exc)
                {

                    try
                    {
                        iterator--;
                        delete_at_func();
                    }
                    catch (LangException exce)
                    {
                        try
                        {
                            iterator--;
                            insert_at_func();
                        }
                        catch (LangException excep)
                        {
                            try
                            {
                                iterator--;
                                list_get_func();
                            }
                            catch (LangException except)
                            {
                                iterator--;

                                throw new LangException("Ничего не подошло в _Func ->" + (iterator) + "\n\t\t\t\t" + ex.Message + "\n\t\t\t\t" + exc.Message + "\n\t\t\t\t" + exce.Message + "\n\t\t\t\t" + excep.Message + "\n\t\t\t\t" + except.Message);
                            }
                        }
                    }
                }
            }
        }

        

        private void isEmpty_func()
        {
            IS_EMPTY_KW();
            LEFT_B();
            RIGHT_B();
        }

        private void clear_func()
        {
            CLEAR_KW();
            LEFT_B();
            RIGHT_B();
        }

        private void display_func()
        {
            DISPLAY_KW();
            LEFT_B();
            RIGHT_B();
        }

        private void delete_at_func()
        {
            DELETE_AT_KW();
            func_body();
        }

        private void get_value_func()
        {
            GET_VALUE_KW();
            func_body();
        }

        private void get_index_func()
        {
            GET_INDEX_KW();
            func_body();
        }

        private void count()
        {
            COUNT_KW();
            LEFT_B();
            RIGHT_B();
        }

        private void insert_at_func()
        {
            INSERT_KW();
            LEFT_B();
            valueExpr();
            COMMA_KW();
            valueExpr();
            RIGHT_B();

        }
    
        private void func_body()
        {
            LEFT_B();
            valueExpr();
            RIGHT_B();
        }

        private void logicComp()
        {
            valueExpr();
            COMPARE_OP();
            valueExpr();
        }

        private void SEMICOLON()
        {
            match(getNextToken(), Lexem.SEMICOLON);
        }

        private void ASSIGN_OP()
        {
            match(getNextToken(), Lexem.ASSIGN_OP);
        }

        private void OP()
        {
            match(getNextToken(), Lexem.OP);
        }

        private void VAR()
        {
            match(getNextToken(), Lexem.VAR);
        }

        private void DIGIT()
        {
            match(getNextToken(), Lexem.DIGIT);
        }

        private void IF_KW()
        {
            match(getNextToken(), Lexem.IF_KW);
        }

        private void WHILE_KW()
        {
            match(getNextToken(), Lexem.WHILE_KW);
        }

        private void LEFT_B()
        {
            match(getNextToken(), Lexem.L_B);
        }

        private void RIGHT_B()
        {
            match(getNextToken(), Lexem.R_B);
        }

        private void LEFT_SB()
        {
            match(getNextToken(), Lexem.L_SB);
        }

        private void RIGHT_SB()
        {
            match(getNextToken(), Lexem.R_SB);
        }

        private void COMPARE_OP()
        {
            match(getNextToken(), Lexem.COMPARE_OP);
        }

        private void DOT()
        {
            match(getNextToken(), Lexem.DOT);
        }

        private void COMMA_KW()
        {
            match(getNextToken(), Lexem.COMMA_KW);
        }

        private void ELSE_KW()
        {
            match(getNextToken(), Lexem.ELSE_KW);
        }

        private void LIST_KW()
        {
            match(getNextToken(), Lexem.LIST_KW);
        }

        private void HT_KW()
        {
            match(getNextToken(), Lexem.HT_KW);
        }


        private void IS_EMPTY_KW()
        {
            match(getNextToken(), Lexem.IS_EMPTY_KW);
        }

        private void CLEAR_KW()
        {
            match(getNextToken(), Lexem.CLEAR_KW);
        }

        private void DISPLAY_KW()
        {
            match(getNextToken(), Lexem.DISPLAY_KW);
        }

        private void DELETE_AT_KW()
        {
            match(getNextToken(), Lexem.DELETE_AT_KW);
        }

        private void GET_VALUE_KW()
        {
            match(getNextToken(), Lexem.GET_VALUE_KW);
        }

        private void GET_INDEX_KW()
        {
            match(getNextToken(), Lexem.GET_INDEX_KW);
        }

        private void COUNT_KW()
        {
            match(getNextToken(), Lexem.COUNT_KW);
        }

        private void INSERT_KW()
        {
            match(getNextToken(), Lexem.INSERT_KW);
        }

        private void SEARCH_KW()
        {
            match(getNextToken(), Lexem.SEARCH_KW);
        }
        private void FUNC_KW()
        {
            match(getNextToken(), Lexem.FUNC_KW);
        }
        private void RETURN_KW()
        {
            match(getNextToken(), Lexem.RETURN_KW);
        }

        // Не уверен в этой функции, нужно тестировать (опять все на try/catch)
        private void sqBracketBody()
        {
            LEFT_SB();
            try
            {
                while (true)
                {
                    expr();
                }
            }
            catch (LangException )
            {
                try
                {
                    iterator--;
                    RIGHT_SB();
                }
                catch (LangException ex)
                {
                    //Console.WriteLine((iterator - 1) + "-> " + e.Message);
                    //Console.WriteLine((iterator - 1) + "-> Ожидалась {0}\n\t{1}", Lexem.R_SB, ex.Message);

                    throw new LangException((iterator - 1) + "-> Ожидалась " + Lexem.R_SB + "\n\t" + ex.Message);
                }
            }
        }

        private void match(Token currentToken, Lexem requiredLexem)
        {
            if (currentToken.lexem != requiredLexem)
            {
                throw new LangException(requiredLexem.ToString() + " expected, but " + currentToken.lexem + "(" + currentToken.value + ") found -> " + iterator);
            }
            Console.WriteLine(currentToken.lexem + " " + currentToken.value + " -> " + (iterator));
        }

        private Token getNextToken()
        {
            if (tokens.Count > iterator)
            {
                var res = tokens[iterator];
                iterator++;
                return res;
            }
            else
            {

                throw new Exception("Out of Tokens -> " + (iterator));

            }
        }

        
        

    }
}