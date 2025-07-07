using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Assignment_7
{
    class Numbers_Q1
    {
        static void Main(string[] args)
        {

            List<int> nums = new List<int>();
            Console.Write("Enter count of numbers: ");
            int n = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter list of numbers: ");
            for (int i = 0; i < n; i++)
            {
                int a = Convert.ToInt32(Console.ReadLine());
                nums.Add(a);
            }
            var res = nums.Where(x => x * x > 20).Select(x => $"{x} - {x * x}");

            foreach (var i in res)
            {
                Console.WriteLine(i);
            }
            Console.Read();
        }
    }
}