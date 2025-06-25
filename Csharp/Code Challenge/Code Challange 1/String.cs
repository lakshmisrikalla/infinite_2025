using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code_Challange_1
{
class String
        {
            static void Main()
            {
                Console.Write("Enter a string: ");
                string input = Console.ReadLine();

                Console.Write("Enter the position to remove (0-based index): ");
                int index = int.Parse(Console.ReadLine());

                if (index >= 0 && index < input.Length)
                {
                    string result = input.Remove(index, 1);
                    Console.WriteLine("Result: " + result);
                }
                else
                {
                    Console.WriteLine("Invalid position.");
                }
            Console.ReadLine();
            }
        }

   }

