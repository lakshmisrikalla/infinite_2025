using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Array_program_3
{
    class Program
    {
        static void Main(string[] args)

        {
            Console.Write("Enter the number of elements: ");
            int size;
            while (!int.TryParse(Console.ReadLine(), out size) || size <= 0)
            {
                Console.Write("Invalid input. Please enter a positive integer: ");
            }

            int[] sourceArray = new int[size];
            int[] copiedArray = new int[size];

            Console.WriteLine("\nEnter the elements of the source array:");
            for (int i = 0; i < size; i++)
            {
                Console.Write($"Element {i + 1}: ");
                while (!int.TryParse(Console.ReadLine(), out sourceArray[i]))
                {
                    Console.Write("Invalid input. Please enter a valid integer: ");
                }
            }

            // Manual copy
            for (int i = 0; i < size; i++)
            {
                copiedArray[i] = sourceArray[i];
            }

            Console.WriteLine("\nSource Array: ");
            for (int i = 0; i < size; i++)
            {
                Console.Write(sourceArray[i] + " ");
            }

            Console.WriteLine("\nCopied Array: ");
            for (int i = 0; i < size; i++)
            {
                Console.Write(copiedArray[i] + " ");
            }

            Console.WriteLine();
            Console.ReadLine();
        }
    }
}
