using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code_Challange_1
{
    class Exchange
    {
        static void Main()
        {
            Console.Write("Enter a string: ");
            string input = Console.ReadLine();

            string result = SwapFirstLast(input);
            Console.WriteLine("Result: " + result);
            Console.ReadLine();
        }
        
        static string SwapFirstLast(string str)
        {
            if (str.Length <= 1)
                return str;

            char[] chars = str.ToCharArray();

           
            char temp = chars[0];
            chars[0] = chars[chars.Length - 1];
            chars[chars.Length - 1] = temp;

            return new string(chars);
        }
        
    }
}
