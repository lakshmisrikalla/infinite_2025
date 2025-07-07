using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment_7
{
    class Word_Q2
    {
        public static void Main()
        {
            List<string> words = new List<string>();
            Console.WriteLine("Enter number of strings");
            int n = Convert.ToInt32(Console.ReadLine());
            for (int i = 0; i < n; i++)
            {
                Console.Write("Enter String {0}: ", i + 1);
                string s = Console.ReadLine();
                words.Add(s);
            }
            var result = words.Where(word => word.StartsWith("a") && word.EndsWith("m"));
            Console.WriteLine("Output Strings: ");
            foreach (var res in result)
                Console.WriteLine(res);
            Console.Read();
        }

    }
}