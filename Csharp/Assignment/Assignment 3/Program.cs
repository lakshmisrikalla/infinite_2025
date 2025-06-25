using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment_3
{

    class MainProgram
    {
        static void Main()
        {
           
            Console.WriteLine("===== Account Transaction =====");
            Console.Write("Enter Account Number: ");
            int accNo = int.Parse(Console.ReadLine());

            Console.Write("Enter Customer Name: ");
            string custName = Console.ReadLine();

            Console.Write("Enter Account Type: ");
            string accType = Console.ReadLine();

            Console.Write("Enter Transaction Type (D/W): ");
            char transType = char.Parse(Console.ReadLine());

            Console.Write("Enter Transaction Amount: ");
            int amount = int.Parse(Console.ReadLine());

            Console.Write("Enter Initial Balance: ");
            int balance = int.Parse(Console.ReadLine());

            Accounts acc = new Accounts(accNo, custName, accType, transType, amount, balance);
            acc.ShowData();


          
            Console.WriteLine("\n===== Student Result =====");
            Console.Write("Enter Roll Number: ");
            int rollNo = int.Parse(Console.ReadLine());

            Console.Write("Enter Name: ");
            string name = Console.ReadLine();

            Console.Write("Enter Class: ");
            string cls = Console.ReadLine();

            Console.Write("Enter Semester: ");
            string sem = Console.ReadLine();

            Console.Write("Enter Branch: ");
            string branch = Console.ReadLine();

            Student stu = new Student(rollNo, name, cls, sem, branch);
            stu.GetMarks();
            stu.DisplayResult();
            stu.DisplayData();


            
            Console.WriteLine("\n===== Sales Transaction =====");
            Console.Write("Enter Sales Number: ");
            int salesNo = int.Parse(Console.ReadLine());

            Console.Write("Enter Product Number: ");
            int productNo = int.Parse(Console.ReadLine());

            Console.Write("Enter Price: ");
            double price = double.Parse(Console.ReadLine());

            Console.Write("Enter Quantity: ");
            int qty = int.Parse(Console.ReadLine());

            Console.Write("Enter Date of Sale (yyyy-mm-dd): ");
            DateTime date = DateTime.Parse(Console.ReadLine());

            Saledetails sale = new Saledetails(salesNo, productNo, price, qty, date);
            Saledetails.ShowData(sale);
            Console.ReadLine();
        }
    }
}
