using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code_Challenge_2
{
    class Check
    {
        static void CheckIfNegative(int number)
        {
            if (number < 0)
            {
                throw new ArgumentException(" Negative numbers are not allowed.");
            }
            else
            {
                Console.WriteLine($" You entered a valid number: {number}");
            }
        }

        static void Main(string[] args)
        {
            Console.Write("Enter an integer: ");
            string input = Console.ReadLine();

            try
            {
                int number = int.Parse(input);
                CheckIfNegative(number);
            }
            catch (FormatException)
            {
                Console.WriteLine(" Please enter a valid integer.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
        }
    }

}
