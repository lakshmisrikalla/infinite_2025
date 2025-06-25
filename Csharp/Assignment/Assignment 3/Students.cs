using System;

public class Student
{
    int RollNo;
    string Name;
    string ClassName;
    int Semester;
    string Branch;
    int[] Marks = new int[5];

    public void ProcessStudent()
    {
        Console.WriteLine("");
        Console.Write("Enter Roll Number: ");
        RollNo = Convert.ToInt32(Console.ReadLine());

        Console.Write("Enter Name: ");
        Name = Console.ReadLine();

        Console.Write("Enter Class: ");
        ClassName = Console.ReadLine();

        Console.Write("Enter Semester: ");
        Semester = Convert.ToInt32(Console.ReadLine());

        Console.Write("Enter Branch: ");
        Branch = Console.ReadLine();

        GetMarks();
        DisplayResult();
        DisplayData();
    }

    void GetMarks()
    {
        Console.WriteLine("Enter marks for 5 subjects:");
        for (int i = 0; i < 5; i++)
        {
            Console.Write($"Subject {i + 1}: ");
            Marks[i] = Convert.ToInt32(Console.ReadLine());
        }
    }

    void DisplayResult()
    {
        int total = 0;
        bool fail = false;

        foreach (int mark in Marks)
        {
            if (mark < 35)
                fail = true;
            total += mark;
        }

        double average = total / 5.0;

        Console.WriteLine(fail || average < 50 ? "Result: Failed" : "Result: Passed");
    }

    void DisplayData()
    {
        Console.WriteLine("\n--- Student Details ---");
        Console.WriteLine($"Roll No: {RollNo}, Name: {Name}, Class: {ClassName}, Semester: {Semester}, Branch: {Branch}");
        Console.WriteLine("Marks: " + string.Join(", ", Marks));
    }
}
