using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment_3
{
    class Program
    {
        static void Main()
        {
            Accounts account = new Accounts();
            account.ProcessAccount();

            Student student = new Student();
            student.ProcessStudent();

            SaleDetails sale = new SaleDetails();
            sale.ProcessSale();
        }
    }
}
