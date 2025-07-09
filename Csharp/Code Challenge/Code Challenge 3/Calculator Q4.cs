using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code_Challenge_3
{

        public delegate int CalculatorDelegate(int a, int b);

        class Calculator_Q4
        {
            
            public static int Add(int a, int b)
            {
                return a + b;
            }

            public static int Subtract(int a, int b)
            {
                return a - b;
            }

            public static int Multiply(int a, int b)
            {
                return a * b;
            }

           
            public static void PerformOperation(string operationName, CalculatorDelegate operation, int x, int y)
            {
                int result = operation(x, y);
                Console.WriteLine($"{operationName} of {x} and {y} is: {result}");
            }

            static void Main(string[] args)
            {
                Console.WriteLine("Enter the first integer:");
                int num1 = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Enter the second integer:");
                int num2 = Convert.ToInt32(Console.ReadLine());

                
                PerformOperation("Addition", Add, num1, num2);
                PerformOperation("Subtraction", Subtract, num1, num2);
                PerformOperation("Multiplication", Multiply, num1, num2);

                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
