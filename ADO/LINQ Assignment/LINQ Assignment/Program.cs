using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQ_Assignment
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public DateTime DOB { get; set; }
        public DateTime DOJ { get; set; }
        public string City { get; set; }

        public void Display()
        {
            Console.WriteLine($"ID: {EmployeeID}, Name: {FirstName} {LastName}, Title: {Title}, DOB: {DOB:dd/MM/yyyy}, DOJ: {DOJ:dd/MM/yyyy}, City: {City}");
        }
    }

    class Program
    {
        static void Main()
        {
            List<Employee> empList = new List<Employee> {
                new Employee { EmployeeID=1001, FirstName="Malcolm", LastName="Daruwalla", Title="Manager", DOB=DateTime.Parse("16/11/1984"), DOJ=DateTime.Parse("8/6/2011"), City="Mumbai" },
                new Employee { EmployeeID=1002, FirstName="Asdin", LastName="Dhalla", Title="AsstManager", DOB=DateTime.Parse("20/08/1984"), DOJ=DateTime.Parse("7/7/2012"), City="Mumbai" },
                new Employee { EmployeeID=1003, FirstName="Madhavi", LastName="Oza", Title="Consultant", DOB=DateTime.Parse("14/11/1987"), DOJ=DateTime.Parse("12/4/2015"), City="Pune" },
                new Employee { EmployeeID=1004, FirstName="Saba", LastName="Shaikh", Title="SE", DOB=DateTime.Parse("3/6/1990"), DOJ=DateTime.Parse("2/2/2016"), City="Pune" },
                new Employee { EmployeeID=1005, FirstName="Nazia", LastName="Shaikh", Title="SE", DOB=DateTime.Parse("8/3/1991"), DOJ=DateTime.Parse("2/2/2016"), City="Mumbai" },
                new Employee { EmployeeID=1006, FirstName="Amit", LastName="Pathak", Title="Consultant", DOB=DateTime.Parse("7/11/1989"), DOJ=DateTime.Parse("8/8/2014"), City="Chennai" },
                new Employee { EmployeeID=1007, FirstName="Vijay", LastName="Natrajan", Title="Consultant", DOB=DateTime.Parse("2/12/1989"), DOJ=DateTime.Parse("1/6/2015"), City="Mumbai" },
                new Employee { EmployeeID=1008, FirstName="Rahul", LastName="Dubey", Title="Associate", DOB=DateTime.Parse("11/11/1993"), DOJ=DateTime.Parse("6/11/2014"), City="Chennai" },
                new Employee { EmployeeID=1009, FirstName="Suresh", LastName="Mistry", Title="Associate", DOB=DateTime.Parse("12/8/1992"), DOJ=DateTime.Parse("3/12/2014"), City="Chennai" },
                new Employee { EmployeeID=1010, FirstName="Sumit", LastName="Shah", Title="Manager", DOB=DateTime.Parse("12/4/1991"), DOJ=DateTime.Parse("2/1/2016"), City="Pune" }
            };

            Console.WriteLine("1. Employees who have joined before 1/1/2015:");
            foreach (var emp in empList.Where(e => e.DOJ < new DateTime(2015, 1, 1))) emp.Display();
            Console.WriteLine();

            Console.WriteLine("2. Employees born after 1/1/1990:");
            foreach (var emp in empList.Where(e => e.DOB > new DateTime(1990, 1, 1))) emp.Display();
            Console.WriteLine();

            Console.WriteLine("3. Employees who are Consultant or Associate:");
            foreach (var emp in empList.Where(e => e.Title == "Consultant" || e.Title == "Associate")) emp.Display();
            Console.WriteLine();

            Console.WriteLine($"4. Total number of employees: {empList.Count}\n");

            Console.WriteLine($"5. Total number of employees in Chennai: {empList.Count(e => e.City == "Chennai")}\n");

            Console.WriteLine($"6. Max Employee ID: {empList.Max(e => e.EmployeeID)}\n");

            Console.WriteLine($"7. Employees who joined after 1/1/2015: {empList.Count(e => e.DOJ > new DateTime(2015, 1, 1))}\n");

            Console.WriteLine($"8. Employees whose designation is not Associate: {empList.Count(e => e.Title != "Associate")}\n");

            var totalCity = empList.GroupBy(emp => emp.City).Select(g => new { city = g.Key, Count = g.Count() });
            Console.WriteLine("9. Total number of employees based on City:");
            foreach (var x in totalCity)
                Console.WriteLine($"City: {x.city}, Count: {x.Count}");
            Console.WriteLine();

            var totalEmpByCityTitle = empList.GroupBy(e => new { e.City, e.Title }).Select(g => new { g.Key.City, g.Key.Title, Count = g.Count() });
            Console.WriteLine("10. Total number of employees based on City and Title:");
            foreach (var x in totalEmpByCityTitle)
                Console.WriteLine($"City: {x.City}, Title: {x.Title}, Count: {x.Count}");
            Console.WriteLine();

            Console.WriteLine("11. The Youngest Employee:");
            var youngestDOB = empList.Max(e => e.DOB);
            foreach (var emp in empList.Where(e => e.DOB == youngestDOB)) emp.Display();

            Console.Read();
        }
    }
}