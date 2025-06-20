using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Array_Programs
{
    class Program
    {
        static void Main(string[] args)

        {
            Console.WriteLine("=== Average,min,max===");
            Console.Write("Enter the number of elements: ");
            int size = Convert.ToInt32(Console.ReadLine());

            int[] numbers = new int[size];
            int sum = 0;

            Console.WriteLine("Enter the elements of the array:");

            for (int i = 0; i < size; i++)
            {
                Console.Write($"Element {i + 1}: ");
                numbers[i] = Convert.ToInt32(Console.ReadLine());
                sum += numbers[i];
            }

            int min = numbers[0];
            int max = numbers[0];

            for (int i = 1; i < size; i++)
            {
                if (numbers[i] < min)
                    min = numbers[i];

                if (numbers[i] > max)
                    max = numbers[i];
            }

            double average = (double)sum / size;

            Console.WriteLine("\nResults:");
            Console.WriteLine("Average Value: " + average);
            Console.WriteLine("Minimum Value: " + min);
            Console.WriteLine("Maximum Value: " + max);
            Console.ReadLine();
        }

    }

}


          
    

    
