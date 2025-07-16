using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csharp_Question
{
    class Query
    {
        static void Main(string[] args)
        {
            var empList = DataSetup.GetEmployees();

            Console.WriteLine("\n a. All Employee Details:");
            foreach (var emp in empList)
                Print(emp);

            Console.WriteLine("\nb. Employees not from Mumbai:");
            var locationNotMumbai = empList.Where(e => e.City != "Mumbai");
            foreach (var emp in locationNotMumbai)
            {
                Console.WriteLine($"{emp.EmployeeID}, {emp.FirstName}, {emp.LastName} ,{emp.Title} ,{emp.DOB} ,{emp.DOJ}, {emp.City}");
            }

            Console.WriteLine("\n  c. Employees with Title 'AsstManager':");
            var titleAsstManagers = empList.Where(e => e.Title == "AsstManager");
            foreach (var emp in titleAsstManagers)
            {
                Console.WriteLine($"{emp.EmployeeID}, {emp.FirstName}, {emp.LastName} ,{emp.Title} ,{emp.DOB} ,{emp.DOJ}, {emp.City}");
            }

            Console.WriteLine("\n d. Employees whose Last Name starts with 'S':");
            var lastNameWithS = empList.Where(e => e.LastName.StartsWith("S"));
            foreach (var emp in lastNameWithS)
            {
                Console.WriteLine($"{emp.EmployeeID}, {emp.FirstName}, {emp.LastName} ,{emp.Title} ,{emp.DOB} ,{emp.DOJ}, {emp.City}");
            }
            Console.ReadLine();
        }

        static void Print(Employee emp)
        {
            Console.WriteLine($"{emp.EmployeeID}, {emp.FirstName} {emp.LastName}, {emp.Title}, DOB: {emp.DOB}, DOJ: {emp.DOJ}, City: {emp.City}");
        }
    }
}

