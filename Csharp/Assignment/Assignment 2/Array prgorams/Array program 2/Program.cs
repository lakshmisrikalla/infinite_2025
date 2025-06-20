using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Array_program_2
{
    class Program
    {
        static void Main(string[] args)
            {
                int[] marks = new int[10];

                Console.WriteLine("Enter 10 marks:");

                for (int i = 0; i < marks.Length; i++)
                {
                    Console.Write($"Mark {i + 1}: ");
                    while (!int.TryParse(Console.ReadLine(), out marks[i]))
                    {
                        Console.Write("Invalid input. Please enter a valid integer: ");
                    }
               }

                int total = marks.Sum();
                double average = marks.Average();
                int min = marks.Min();
                int max = marks.Max();

                Console.WriteLine($"\nTotal Marks: {total}");
                Console.WriteLine($"Average Marks: {average:F2}");
                Console.WriteLine($"Minimum Marks: {min}");
                Console.WriteLine($"Maximum Marks: {max}");

                Console.WriteLine("\nMarks in Ascending Order:");
                foreach (int mark in marks.OrderBy(m => m))
                {
                    Console.Write(mark + " ");
                }

                Console.WriteLine("\n\nMarks in Descending Order:");
                foreach (int mark in marks.OrderByDescending(m => m))
                {
                    Console.Write(mark + " ");
                }

                Console.WriteLine();
                Console.ReadLine();
            }
        }

    }
