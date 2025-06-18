using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sample
{
    class Program
    {
       
            static void Main(string[] args)

            {




                CheckIfEqual();
                CheckIfPositiveOrNegative();
                PerformOperations();
                Multiplication();
                Console.ReadLine();

                Console.Write(" Enter the first number");
                int a = Convert.ToInt32(Console.ReadLine());
                Console.Write("enter the second number");
                int b = Convert.ToInt32(Console.ReadLine());
                int ans = ReturnFun(a, b);
                Console.WriteLine("result:" + ans);
                Console.ReadLine();
            }


            public static void CheckIfEqual()
            {
                Console.WriteLine(" ===check if two numbers are equal=== ");


                Console.Write("Enter first number:");
                int n1 = Convert.ToInt32(Console.ReadLine());

                Console.Write("Enter second number:");
                int n2 = Convert.ToInt32(Console.ReadLine());

                if (n1 == n2)

                    Console.WriteLine($"Both numbers are equal {n1}={n2}.\n");
                else
                    Console.WriteLine($"{n1} and {n2} are not equal \n");


            }



            public static void CheckIfPositiveOrNegative()
            {
                Console.WriteLine("=== If the number is positive or negative===");

                Console.WriteLine("Enter your number");
                int n3 = Convert.ToInt32(Console.ReadLine());

                if (n3 > 0)
                    Console.WriteLine($"{n3} is positive");
                else
                    Console.WriteLine($"{n3} is negative");

            }

            public static void PerformOperations()
            {
                Console.WriteLine("===Performing operations for two input values===");
                Console.WriteLine("Enter first number");
                int n4 = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Enter second number");
                int n5 = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Enter operation");
                Char O = Convert.ToChar(Console.ReadLine());

                int result;

                switch (O)
                {
                    case '+':
                        result = n4 + n5;
                        Console.WriteLine($" {n4} +{n5}={result}");
                        break;
                    case '-':
                        result = n4 - n5;
                        Console.WriteLine($"{n4}-{n5}={result}");
                        break;
                    case '*':
                        result = n4 * n5;
                        Console.WriteLine($"{n4}*{n5}={result}");
                        break;
                    case '/':
                        result = n4 / n5;
                        Console.WriteLine($"{n4}/{n5}={result}");
                        break;

                }

            }


            public static void Multiplication()
            {
                Console.WriteLine("===Multipilication of the given number===");

                Console.WriteLine("Enter the number");
                int n6 = Convert.ToInt32(Console.ReadLine());

                for (int i = 0; i < 11; i++)

                {
                    Console.WriteLine($"{n6}*{i}={ n6 * i}");

                }

            }

            public static int ReturnFun(int a, int b)
            {
                if (a == b)
                {
                    return 3 * (a + b);
                }
                return a + b;
            }

        }
    }