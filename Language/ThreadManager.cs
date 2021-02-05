using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Language
{
    public static class ThreadManager
    {
        private static List<TriadExecutor> calls = new List<TriadExecutor>();

        public static void AddContex(TriadExecutor t_e)
        {
            calls.Add(t_e);
        }

        public static void RunAsync(int step)
        {
            while (true)
            {
                if (calls.Count == 0)
                    break;
                SortFunc();
                System.Threading.Thread.Sleep(2500);
                Console.WriteLine();
                //Console.ReadLine();

                var a = new TriadExecutor();
                a.SetContext(calls.First(), step);
                if (a.This.Async == false)
                {
                    SortFunc();
                    a.SetContext(calls.First(), int.MaxValue);
                    a.Exucute();
                    calls.Remove(calls.First());
                }
                else
                {
                    Console.WriteLine("+++" + a.Exucute() + "+++");
                    calls.Remove(calls.First());
                }
                
            }

        }

        private static void SortFunc()
        {
            for (int i = 0; i < calls.Count; i++)
            {
                for (int j = i + 1; j < calls.Count - 1; j++)
                {
                    if (calls[i].priority >= calls[j].priority)
                    {
                        var temp = calls[i];
                        calls[i] = calls[j];
                        calls[j] = temp;
                    }
                }
            }
        }

    }
}
