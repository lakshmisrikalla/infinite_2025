using System;

class Student
{
    public int RollNo;
    public string Name;
    public string Class;
    public string Semester;
    public string Branch;
    public int[] Marks = new int[5];

    public Student(int rollNo, string name, string cls, string sem, string branch)
    {
        RollNo = rollNo;
        Name = name;
        Class = cls;
        Semester = sem;
        Branch = branch;
    }

    public void GetMarks()
    {
        Console.WriteLine($"Enter marks for {Name}:");
        for (int i = 0; i < 5; i++)
        {
            Console.Write($"Subject {i + 1}: ");
            Marks[i] = int.Parse(Console.ReadLine());
        }
    }

    public void DisplayResult()
    {
        int total = 0;
        bool hasFailedSubject = false;

        foreach (int mark in Marks)
        {
            total += mark;
            if (mark < 35) hasFailedSubject = true;
        }

        double avg = total / 5.0;

        if (hasFailedSubject)
            Console.WriteLine("Result: Failed (Subject mark < 35)");
        else if (avg < 50)
            Console.WriteLine("Result: Failed (Average < 50)");
        else
            Console.WriteLine("Result: Passed");
    }

    public void DisplayData()
    {
        Console.WriteLine($"Roll No: {RollNo}\nName: {Name}\nClass: {Class}\nSemester: {Semester}\nBranch: {Branch}");
        Console.WriteLine("Marks: " + string.Join(", ", Marks));
    }
}
