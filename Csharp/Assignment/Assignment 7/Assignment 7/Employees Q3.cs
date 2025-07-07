using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment_7
{
    class Employee
    {
        public int EmpId;
        public string Name;
        public string City;
        public double Salary;

        public Employee(int id, string name, string city, double salary)
        {
            EmpId = id;
            Name = name;
            City = city;
            Salary = salary;
        }
    }

    class Employess_Q3
    {
        public static void Main()
        {
            Console.Write("Enter number of employees: ");
            List<Employee> employees = new List<Employee>();
            int n = Convert.ToInt32(Console.ReadLine());
            for (int i = 0; i < n; i++)
            {
                Console.Write("Enter ID of employee-{0}: ", i + 1);
                int id = Convert.ToInt32(Console.ReadLine());
                Console.Write("Enter Name of employee-{0}: ", i + 1);
                string name = Console.ReadLine();
                Console.Write("Enter City of employee-{0}: ", i + 1);
                string city = Console.ReadLine();
                Console.Write("Enter Salary of employee-{0}:", i + 1);
                double salary = Convert.ToDouble(Console.ReadLine());
                Employee e = new Employee(id, name, city, salary);
                employees.Add(e);
            }

            Console.WriteLine("======================All Employees========================");
            foreach (var emp in employees)
            {
                Console.WriteLine($"Id:{emp.EmpId}, Name:{emp.Name}, City:{emp.City}, Salary:{emp.Salary}");
            }

            Console.WriteLine("======================Employees with salary greater than 45000=============");
            var HighSalaryEmployees = employees.Where(emp => emp.Salary > 45000);
            foreach (var emp in HighSalaryEmployees)
            {
                Console.WriteLine($"Id:{emp.EmpId}, Name:{emp.Name}, City:{emp.City}, Salary:{emp.Salary}");
            }

            Console.WriteLine("======================Employees from Vizag===================");
            var BangaloreEmployees = employees.Where(emp => emp.City == "Vizag");
            foreach (var emp in BangaloreEmployees)
            {
                Console.WriteLine($"Id:{emp.EmpId}, Name:{emp.Name}, City:{emp.City}, Salary:{emp.Salary}");
            }

            Console.WriteLine("===========Employees sorted by name in ascending order============");
            var SortedEmployees = employees.OrderBy(emp => emp.Name);
            foreach (var emp in SortedEmployees)
            {
                Console.WriteLine($"Id:{emp.EmpId}, Name:{emp.Name}, City:{emp.City}, Salary:{emp.Salary}");
            }

            Console.Read();
        }
    }
}