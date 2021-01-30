using System;
using System.Collections.Generic;
using System.Text;
/*
     VAR -> ^([a-zA-Z]+)$
    DIGIT -> ^(0|[1-9][0-9]*)$
    ASSIGN_OP -> ^=$
    OP -> ^(\+|-|\*|\/)$

        new 
    IF_KW -> ^if$
    WHILE_KW -> ^while$
    L_B -> ^\($
    R_B -> ^\)$
    L_SB -> ^\{$
    R_SB -> ^\}$
    COMPARE_OP -> ^(>=|<=|>|<|!=|==)$

    count = 11
 */

namespace Language
{
    public class Lexem
    {
        public String regexp { get; }
        public String name { get; }


        public Lexem(String regexp, String name)
        {
            this.regexp = regexp;
            this.name = name;
        }

        public static readonly Lexem SEMICOLON = new Lexem(@"^;$", "SEMICOLON");
        public static readonly Lexem VAR = new Lexem(@"^([a-zA-Z]+)$", "VAR");
        public static readonly Lexem DIGIT = new Lexem(@"^(0|[1-9][0-9]*)$", "DIGIT");
        public static readonly Lexem ASSIGN_OP = new Lexem(@"^=$", "ASSIGN_OP");
        public static readonly Lexem OP = new Lexem(@"^(\+|-|\*|\/)$", "OP");
        public static readonly Lexem IF_KW = new Lexem(@"^if$", "IF_KW");
        public static readonly Lexem ELSE_KW = new Lexem(@"^else$", "ELSE_KW");
        public static readonly Lexem ELIF_KW = new Lexem(@"^elif$", "ELIF_KW");
        public static readonly Lexem WHILE_KW = new Lexem(@"^while$", "WHILE_KW");
        public static readonly Lexem L_B = new Lexem(@"^\($", "L_B");
        public static readonly Lexem R_B = new Lexem(@"^\)$", "R_B");
        public static readonly Lexem L_SB = new Lexem(@"^\{$", "L_SB");
        public static readonly Lexem R_SB = new Lexem(@"^\}$", "R_SB");
        public static readonly Lexem COMPARE_OP = new Lexem(@"^(>=|<=|>|<|!=|==)$", "COMPARE_OP");
        public static readonly Lexem DOT = new Lexem(@"^\.$", "DOT");
        public static readonly Lexem LIST_KW = new Lexem(@"^List$", "LIST_KW");
        public static readonly Lexem HT_KW = new Lexem(@"^HashTable$", "HT_KW");
        public static readonly Lexem ADD_KW = new Lexem(@"^Add$", "ADD_KW");
        public static readonly Lexem IS_EMPTY_KW = new Lexem(@"^IsEmpty$", "IS_EMPTY_KW");
        public static readonly Lexem CLEAR_KW = new Lexem(@"^Clear$", "CLEAR_KW");
        public static readonly Lexem DISPLAY_KW = new Lexem(@"^Display$", "DISPLAY_KW");
        public static readonly Lexem DELETE_AT_KW = new Lexem(@"^DeleteAt$", "DELETE_AT_KW");
        public static readonly Lexem GET_VALUE_KW = new Lexem(@"^GetValue$", "GET_VALUE_KW");
        public static readonly Lexem GET_INDEX_KW = new Lexem(@"^GetIndex$", "GET_INDEX_KW");
        public static readonly Lexem COUNT_KW = new Lexem(@"^Count$", "COUNT_KW");
        public static readonly Lexem INSERT_KW = new Lexem(@"^Insert$", "INSERT_KW");
        public static readonly Lexem COMMA_KW = new Lexem(@"^,$", "COMMA_KW");
        public static readonly Lexem SEARCH_KW = new Lexem(@"^Search$", "SEARCH_KW");

        public static readonly Lexem BOOL = new Lexem("", "BOOL");
        public static readonly Lexem OUT_KW = new Lexem(@"^out$", "OUT_KW");

        public static readonly Lexem END = new Lexem("", "END");
        public static readonly Lexem F_T = new Lexem("", "F_T");
        public static readonly Lexem T_T = new Lexem("", "T_T");
        public static readonly Lexem UNC_T = new Lexem("", "UNC_T");
        public static readonly Lexem TRANS_LBL = new Lexem("", "TRANS_LBL");
        public static readonly Lexem TRIAD_LBL = new Lexem("", "TRIAD_LBL");
        public static readonly Lexem CONST = new Lexem("", "C");
        public static readonly Lexem FUNC = new Lexem("", "FUNC");

        public static readonly Lexem FUNC_KW = new Lexem("^func$", "FUNC_KW");
        public static readonly Lexem FUNCTION_CALL = new Lexem("", "FUNCTION_CALL");
        public static readonly Lexem SYSTEM_FUNC = new Lexem("", "SYSTEM_FUNC");
        public static readonly Lexem Display_FUNC = new Lexem("", "Display");
        public static readonly Lexem RETURN_KW = new Lexem("^return$", "RETURN_KW");



        //public static readonly int Count = 16;

        public static IEnumerable<Lexem> Values
        {
            get
            {
                yield return SEMICOLON;
                yield return IF_KW;
                yield return ELSE_KW;
                yield return ELIF_KW;
                yield return WHILE_KW;
                

                yield return DIGIT;
                yield return ASSIGN_OP;
                yield return OP;
                yield return R_B;
                yield return L_B;
                yield return COMPARE_OP;
                yield return R_SB;
                yield return L_SB;
                yield return DOT;
                

                yield return LIST_KW;
                yield return HT_KW;

                yield return ADD_KW;
                yield return IS_EMPTY_KW;
                yield return CLEAR_KW;
                yield return DISPLAY_KW;
                yield return DELETE_AT_KW;
                yield return GET_VALUE_KW;
                yield return GET_INDEX_KW;
                yield return COUNT_KW;
                yield return INSERT_KW;
                yield return COMMA_KW;
                yield return SEARCH_KW;

                yield return RETURN_KW;
                yield return FUNC_KW;
                yield return VAR;


            }
        }
        public static IEnumerable<Lexem> Extra
        {
            get
            {
                yield return END;
                yield return TRIAD_LBL;
                yield return SYSTEM_FUNC;
                yield return Display_FUNC;
                yield return FUNCTION_CALL;
            }
        }

        public override string ToString() => name;
    }
}

