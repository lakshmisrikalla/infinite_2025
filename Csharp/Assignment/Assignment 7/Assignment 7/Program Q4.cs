using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary;
namespace Assignment_7
{
    class Program_Q4
    {
       public static void Main()
            {
                Console.Write("Enter your name: ");
                string Name = Console.ReadLine();
                Console.Write("Enter your age: ");
                int Age = int.Parse(Console.ReadLine());
                Console.Write("Enter the total fare: ");
                double TotalFare = double.Parse(Console.ReadLine());

                ClassLibrary.TravelFare travelConcession = new TravelFare();
                string result = travelConcession.CalculateConcession(Age, TotalFare);
                Console.WriteLine($"Hello {Name}, {result}");
                Console.Read();
            }
        }

    }
