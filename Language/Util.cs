using System;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Language
{
    public class Util
    {
        public static void ShowTriads(List<Triad> Triads)
        {
            Console.WriteLine();
            string temp = "";
            foreach (var triad in Triads)
            {
                Console.Write(Triads.IndexOf(triad) + " (");
                foreach (var param in triad.Params)
                {
                    temp += (param.lexem == Lexem.TRIAD_LBL ? "^" + param.value : param.value) + ", ";
                }
                if (temp.Length > 2)
                {
                    temp = temp.Substring(0, temp.Length - 2);
                }
                Console.WriteLine(temp + ")" + triad.Operator.value);
                temp = "";
            }
        }
        public static void ShowTriads(List<Triad> Triads, int i , ConsoleColor color)
        {
            
            string temp = "";
            int counter = 0;
            foreach (var triad in Triads)
            {
                Console.ForegroundColor = (counter == i) ? color : ConsoleColor.White;
                //Console.ForegroundColor = (counter == i) ? ConsoleColor.Black : ConsoleColor.White;
                Console.Write(Triads.IndexOf(triad) + " (");
                foreach (var param in triad.Params)
                {
                    temp += (param.lexem == Lexem.TRIAD_LBL ? "^" + param.value : param.value) + ", ";
                }
                if (temp.Length > 2)
                {
                    temp = temp.Substring(0, temp.Length - 2);
                }
                Console.WriteLine(temp + ")" + triad.Operator.value);
                temp = "";
                counter++;
            }
            Console.WriteLine();
        }

        public static void ShowTriads(TriadsBuilder tb)
        {
            Console.WriteLine();
            string temp = "";
            foreach (var triad in tb.Triads)
            {
                Console.Write(tb.Triads.IndexOf(triad) + " (");
                foreach (var param in triad.Params)
                {
                    temp += (param.lexem == Lexem.TRIAD_LBL ? "^" + param.value : param.value) + ", ";
                }
                if (temp.Length > 2)
                {
                    temp = temp.Substring(0, temp.Length - 2);
                }
                Console.WriteLine(temp + ")" + triad.Operator.value);
                temp = "";
            }
            Console.WriteLine();
        }


        
        public static string GetHash(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] data = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                var sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return sBuilder.ToString();
            }
        }
        public static bool VerifyHash( string input, string hash)
        {
            var hashOfInput = GetHash(input);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            return comparer.Compare(hashOfInput, hash) == 0;
        }

        public static void SaveToFile(string text)
        {
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory+"hash.txt" , text);
        }

        public static void Serialize(Dictionary<String, FucntionData> functions)
        {
            JsonSerializer serializer = new JsonSerializer();

            using (StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "ser.txt"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, functions);
            }
        }
        public static void SerializeVar(Dictionary<String, String> variables)
        {
            JsonSerializer serializer = new JsonSerializer();

            using (StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "var.txt"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, variables);
            }
        }
        public static Dictionary<String, FucntionData> Deserialize()
        {
            string output = LoadFromFile("ser.txt");
            return JsonConvert.DeserializeObject<Dictionary<String, FucntionData>>(output);
        }
        public static Dictionary<String, String> DeserializeVar()
        {
            string output = LoadFromFile("var.txt");
            return JsonConvert.DeserializeObject<Dictionary<String, String>>(output);
        }

        public static string LoadHashFromFile()
        {
            return File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "hash.txt");
        }
        public static string LoadFromFile(string file)
        {
            return File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + file);// "ser.txt"
        }

        public static Dictionary<TKey, TValue> CloneDictionaryCloningValues<TKey, TValue>(Dictionary<TKey, TValue> original) where TValue : ICloneable
        {
            Dictionary<TKey, TValue> ret = new Dictionary<TKey, TValue>(original.Count,
                                                                    original.Comparer);
            foreach (KeyValuePair<TKey, TValue> entry in original)
            {
                ret.Add(entry.Key, (TValue)entry.Value.Clone());
            }
            return ret;
        }

        public static IList<T> CloneList<T>(IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
        public static double executeArithmeticOperation(double param1, double param2, string op)
        {
            switch (op)
            {
                case "+": return param1 + param2;
                case "-": return param1 - param2;
                case "*": return param1 * param2;
                case "/": return param1 / param2;
                default: throw new Exception("Arithmetic Operation is missing");
            }
        }

        public static bool executeLogicOperation(double param1, double param2, string op)
        {
            switch (op)
            {
                case ">": return param1 > param2;
                case "<": return param1 < param2;
                case ">=": return param1 >= param2;
                case "<=": return param1 <= param2;
                case "!=": return param1 != param2;
                case "==": return param1 == param2;
                default: throw new Exception("Logic Operation is missing");
            }
        }
    }
}
