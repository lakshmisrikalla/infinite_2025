using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code_Challenge_2
{

    abstract class Student
    {
        public string Name { get; set; }
        public int StudentId { get; set; }
        public double Grade { get; set; }

        public Student(string name, int studentId, double grade)
        {
            Name = name;
            StudentId = studentId;
            Grade = grade;
        }

        public abstract bool IsPassed();
    }

    class Undergraduate : Student
    {
        public Undergraduate(string name, int studentId, double grade)
            : base(name, studentId, grade) { }

        public override bool IsPassed()
        {
            return Grade > 70.0;
        }
    }

    class Graduate : Student
    {
        public Graduate(string name, int studentId, double grade)
            : base(name, studentId, grade) { }

        public override bool IsPassed()
        {
            return Grade > 80.0;
        }
    }

    class Students
    {
        static void Main(string[] args)
        {
            Console.Write("Enter student type (UG for Undergraduate, G for Graduate): ");
            string type = Console.ReadLine();

            Console.Write("Enter student name: ");
            string name = Console.ReadLine();

            Console.Write("Enter student ID: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("Enter student grade: ");
            double grade = double.Parse(Console.ReadLine());

            Student student;

            if (type.ToUpper() == "UG")
            {
                student = new Undergraduate(name, id, grade);
            }
            else if (type.ToUpper() == "G")
            {
                student = new Graduate(name, id, grade);
            }
            else
            {
                Console.WriteLine("Invalid student type entered!");
                return;
            }

            Console.WriteLine($"\n{student.Name} ID: {student.StudentId} has {(student.IsPassed() ? "passed" : "not passed")} the course.");
            Console.ReadLine();

        }
    }
}
