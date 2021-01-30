using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Language
{
    class Lexer
    {
        private readonly String inputString;

        public Lexer(string inputString)
        {
            Console.WriteLine("\n\n" + inputString + "\n");
            this.inputString = inputString.Replace("\r\n", "");
            this.inputString = this.inputString.Replace("\t", "").TrimEnd(' ');
            
            
        }

        public List<Token> returnTokens()
        {
            List<Token> result = new List<Token>();
            String currentString = String.Empty;


            // Совпадения 
            List<Token> matches = new List<Token>();
            List<Token> prevMatches = new List<Token>();


            for (int i = 0; i < inputString.Length; i++)
            {
                prevMatches.Clear();
                prevMatches.AddRange(matches);
                matches.Clear();

                
                currentString += inputString[i];
                if (currentString[0] ==  ' ' )
                {
                    currentString = currentString.Substring(1);
                }
                

                //Console.WriteLine(currentString + "\n");
                foreach (Lexem l in Lexem.Values)
                {
                    if (Regex.IsMatch(currentString, l.regexp))
                    {
                       // Console.WriteLine(l.ToString() + " found");
                        matches.Add(new Token(l, currentString));
                    }
                    
                }

                if (matches.Count <= 0)
                {
                    if (prevMatches.Count > 0)
                    {
                        result.Add(prevMatches[0]);
                        if (inputString[i] != ' ')
                          i--;

                        currentString = String.Empty;

                        //Console.WriteLine("\t\t\t\t" + prevMatches[0].lexem + "  (" + prevMatches[0].value + ")");
                    }
                }

                if (i == inputString.Length - 1)
                {

                    result.Add(matches[0]);
                    currentString = String.Empty;


                    //Console.WriteLine("\t\t\t\t" + matches[0].lexem + "  (" + matches[0].value + ")");

                }


                //Console.WriteLine();

                


            }

            
            int count = 0;
            Console.WriteLine("Работа Лексера\n");
            foreach (var t in result)
            {
                Console.WriteLine(count + " ->\t" + t.value + "\t(" + t.lexem + ")");
                count++;
            }
            
            Console.WriteLine();
            return result;
        }




    }
}
