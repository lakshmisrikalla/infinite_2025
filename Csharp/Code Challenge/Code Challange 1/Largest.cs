using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code_Challange_1
{
    class Largest

    {
        static void Main()
        {
            Console.Write("Enter three numbers: ");
            string input = Console.ReadLine();
            string[] parts = input.Split(',');

            if (parts.Length == 3)
            {
                int a = int.Parse(parts[0]);
                int b = int.Parse(parts[1]);
                int c = int.Parse(parts[2]);

                int largest = a;

                if (b > largest) largest = b;
                if (c > largest) largest = c;

                Console.WriteLine("Largest number: " + largest);
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter exactly three numbers.");
            }
            Console.ReadLine();
        }
    }
}
