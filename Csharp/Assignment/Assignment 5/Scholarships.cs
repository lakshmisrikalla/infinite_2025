using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment_5
{
    class Scholarships
    {
       
        public class NotEligibleForScholarshipException : Exception
        {
            public NotEligibleForScholarshipException(string message) : base(message) { }
        }


        public class Scholarship
        {
            public double Merit(int marks, double fees)
            {
                if (marks >= 70 && marks <= 80)
                    return fees * 0.20;
                else if (marks > 80 && marks <= 90)
                    return fees * 0.30;
                else if (marks > 90)
                    return fees * 0.50;
                else
                    throw new NotEligibleForScholarshipException("Marks are not eligible for a scholarship.");
            }
        }

       
        class MainProgram
        {
            static void Main()
            {
                

                try
                {
                   
                    Console.WriteLine("\n===== Scholarship Calculation =====");
                    Console.Write("Enter Marks: ");
                    int marks = int.Parse(Console.ReadLine());

                    Console.Write("Enter Total Fees: ");
                    double fees = double.Parse(Console.ReadLine());

                    Scholarship scholar = new Scholarship();
                    double scholarshipAmount = scholar.Merit(marks, fees);

                    Console.WriteLine($" Scholarship Granted: ₹{scholarshipAmount}");
                }
                catch (NotEligibleForScholarshipException ex)
                {
                    Console.WriteLine($" {ex.Message}");
                }
                catch (FormatException ex)
                {
                    Console.WriteLine(" Please enter valid numeric values.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" {ex.Message}");
                }

                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
        }
    }

}