using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csharp_programs
{
    class Program
    {
        static void Main(string[] args)
           
        {
            int k, l, temp;
            Console.Write("=== swaping of numbers===");

            Console.Write("\nenter first number:");
            k = Convert.ToInt32(Console.ReadLine());

            Console.Write("enter second number:");
            l = Convert.ToInt32(Console.ReadLine());
            

            temp = k;
            k = l;
            l = temp;

            Console.WriteLine($" After swaping : First number ={k}, second number ={l}");
            Console.ReadLine();


            Console.Write(" === Display of number=== ");

            Console.Write("\nEnter a digit: ");
            int n = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("{0} {0} {0} {0}", n);
            Console.WriteLine("{0}{0}{0}{0}", n);
            Console.WriteLine("{0} {0} {0} {0}", n);
            Console.WriteLine("{0}{0}{0}{0}", n);
            Console.ReadLine();


            Console.Write("===Days===");
            Console.Write("\nEnter day number (1 to 7): ");
            int day = Convert.ToInt32(Console.ReadLine());

            switch (day)
            {
                case 1:
                    Console.WriteLine("Monday");
                    break;
                case 2:
                    Console.WriteLine("Tuesday");
                    break;
                case 3:
                    Console.WriteLine("Wednesday");
                    break;
                case 4:
                    Console.WriteLine("Thursday");
                    break;
                case 5:
                    Console.WriteLine("Friday");
                    break;
                case 6:
                    Console.WriteLine("Saturday");
                    break;
                case 7:
                    Console.WriteLine("Sunday");
                    break;
                default:
                    Console.WriteLine("Invalid ");
                    break;
               
            }

            Console.ReadLine();


        }
    }
}
