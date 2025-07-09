using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code_Challenge_3
{
    public class Calculator
    {
        
        public delegate int Operation(int a, int b);

        
        public static int Execute(Operation op, int x, int y)
        {
            return op(x, y);
        }

        
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
            int result = 0;
            bool isNegative = false;

            if (b < 0)
            {
                b = -b;
                isNegative = true;
            }

            for (int i = 0; i < b; i++)
            {
                result += a;
            }

            return isNegative ? -result : result;
        }
    }

    public class Calculator_Q4
    {
        public static void Main()
        {
            Console.Write("Enter first number: ");
            dynamic num1 = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter second number: ");
            dynamic num2 = Convert.ToInt32(Console.ReadLine());

           
            Calculator.Operation addOp = new Calculator.Operation(Calculator.Add);
            Calculator.Operation subOp = new Calculator.Operation(Calculator.Subtract);
            Calculator.Operation mulOp = new Calculator.Operation(Calculator.Multiply);

            
            dynamic resultAdd = Calculator.Execute(addOp, num1, num2);
            dynamic resultSub = Calculator.Execute(subOp, num1, num2);
            dynamic resultMul = Calculator.Execute(mulOp, num1, num2);

            
            Console.WriteLine("\n--- Results ---");
            Console.WriteLine("Addition Result: " + resultAdd);
            Console.WriteLine("Subtraction Result: " + resultSub);
            Console.WriteLine("Multiplication Result: " + resultMul);
            Console.ReadLine();
        }
    }
}