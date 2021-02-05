using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Language
{
    class Program
    {
        
        static void Main(string[] args)
        {
            Dictionary<String, String> variables = new Dictionary<string, string>();
            Dictionary<String, FucntionData> functions = new Dictionary<string, FucntionData>();
            bool resave = false;
            

            string input = File.ReadAllText("INPUT.txt");

            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "hash.txt"))
            {
                var hash = Util.GetHash(input);
                Util.SaveToFile(hash);
                resave = true;
            }
            else
            {
                var hash = Util.LoadHashFromFile();
                if (Util.VerifyHash(input, hash))
                {
                    resave = false;
                }
                else
                {
                    hash = Util.GetHash(input);
                    Util.SaveToFile(hash);
                    resave = true;
                }
            }

            


            

            if (resave)
            {
                Lexer lex = new Lexer(input);
                var tokens = lex.returnTokens();


                Parser parser = new Parser(tokens, functions);
                Console.WriteLine("Работа Парсера\n");
                parser.lang();
                var program_tree = parser.CutFunctions();


                foreach (var key in functions.Keys)
                {
                    SyntaxAnalyzer temp_analyzer = new SyntaxAnalyzer();
                    functions[key].Poliz = temp_analyzer.convert(functions[key].Body);
                }
                Console.WriteLine();
                Console.WriteLine("POLIZ -> ");
                SyntaxAnalyzer s = new SyntaxAnalyzer();
                tokens.Add(new Token(Lexem.END, "$"));
                var poliz = s.convert(tokens);

                Console.WriteLine();
                int q = 0;
                foreach (var item in functions["Main"].Poliz)
                {
                    Console.Write(item.value + "("+q+")  ");
                    q++;
                }
                Console.WriteLine();





                foreach (var key in functions.Keys)
                {
                    TriadsBuilder temp_builder = new TriadsBuilder(variables, functions);
                    temp_builder.Build(functions[key].Poliz);
                    Console.WriteLine("Building triads ->");
                    Console.WriteLine(key);
                    Util.ShowTriads(temp_builder);
                    temp_builder.Analyze();
                    //temp_builder.Optimize();
                    Console.WriteLine("Optimization ->");
                    Console.WriteLine(key);
                    Util.ShowTriads(temp_builder);
                    //temp_builder.DeleteC();
                    Console.WriteLine("Calculate unnecessary triads ->");
                    Console.WriteLine(key);
                    Util.ShowTriads(temp_builder);
                    functions[key].Triads = temp_builder.Triads;

                }
                Console.WriteLine("________________________________________________________________");

                
                Util.Serialize(functions);
                Util.SerializeVar(variables);

            }
            else
            {
                functions = Util.Deserialize();
                variables = Util.DeserializeVar();
            }



            TriadExecutor executor = new TriadExecutor(variables, functions, functions["Main"].Triads, functions["Main"]);
            
            Console.WriteLine("\n\nВыполнение программы->");
            
            
            ThreadManager.AddContex(executor);
            ThreadManager.RunAsync(10);
            //Console.WriteLine("\n\nProgramm output :: " + a);

            
            




            Console.WriteLine("________________________________________________________________");
            Console.WriteLine();
            Console.WriteLine("Конечные значения переменных->");

            
            Console.ReadLine();
            
            
        }

        
    }
}


/*
 a = n.Count() - 1;

while( 1 == 1 ){

	List n;
	n.Add(a * n .GetIndex(12-1)/2);
	n.Insert(a, 12*12/3);
	n.Display();
	n.Clear();

	HashTable v;
	v.Search(3);
	v.Insert(b,1);
	
	
	if( a > b)
	{
		b = 2;
		a = b + 2 * 3;
	}
	else if(b > a ) {
		b = 3;
		a= a * 2 / 1 / b;
	}else{
		a =20;
	}
}
     
      
     
     
     
     
     */



/*
        LinkedList a = new LinkedList();
        a.insertAt(new HTElement(1, 2), a.count());
        a.insertAt(new HTElement(3, 4), a.count());
        a.insertAt(new HTElement(5, 6), a.count());
        a.insertAt(new HTElement(7, 8), a.count());

        for (int i = 0; i < a.count(); i++)
        {
            Console.WriteLine( (a.getValue(i) as HTElement).Key + " " + (a.getValue(i) as HTElement).Value);
        }

        HashTable h = new HashTable();
        h.insert(1, 2);
        h.insert(3, 4);
        h.insert(5, 6);
        h.insert(6, 6);
        h.insert(10, 2);

        h.display();

        */


/*
 List a;
List b;

i = 0;
while(i < 10){

a.Insert(i,i);
b.Insert(i*i, i);
i= i+1;
}

a.Display();
b.Display();

HashTable h;

i=0;
while(i < a.Count()){

h.Insert(a.GetValue(i), b.GetValue(i));
i= i+1;
}

h.Display();


h.DeleteAt(1);
h.DeleteAt(3);
h.DeleteAt(5);

h.Display();


serach = h.Search(7);












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

/*
 
 
 work
 
 
 func Sum(a, b){
	return (a+b);
}
func Multipy(a, b){
	return (a*b);
}
func Pow(a){
	return (a*a);
}

func Main()
{
	A = 2;
	Display(A);
	B = 3;
	Display(B);
	summa = Sum(A, B);
	Display(summa);
	multiplication = Multipy(A, B);
	Display(multiplication);
	powA = Pow(A);
	Display(powA);
	powB = Pow(B);
	Display(powB);
	twoAB = 2*multiplication;
	Display(twoAB);
	tempSum = Sum(powA, twoAB);
	Display(tempSum);
	sqSum = Sum(tempSum, powB);
	Display(sqSum);

	sqSumShort = Pow(summa);
	Display(sqSumShort);
	

	return sqSumShort;	
}
 













 func Sum(a, b){
	return (a+b);
}
func Multipy(a, b){
	return (a*b);
}
func Pow(a){
	return (a*a);
}

func Main()
{
	if(1 < 2){
	Display(123);
	}

	A = Pow(3);
	if(A >= 9){
		Display(1);
	}else{
		Display(0);
	}

	
	return 0;
}
 
 */