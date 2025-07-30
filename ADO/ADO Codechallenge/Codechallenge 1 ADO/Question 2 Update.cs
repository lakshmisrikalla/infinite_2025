using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Codechallenge_1_ADO
{
    class UpdateEmployeeSalaryProgram
    {
        public static SqlConnection con;
        public static SqlCommand cmd;
        public static SqlDataReader dr;

        static void Main(string[] args)
        {
            UpdateEmployee();
            Console.Read();
        }
        static SqlConnection getConnection()
        {
            con = new SqlConnection("Data Source=ICS-LT-2WS9J84\\SQLEXPRESS;Initial Catalog=Codechallenge;" +
         "user ID=sa;Password=Lakshmi@1302!");
            con.Open();
            return con;
        }
        static void UpdateEmployee()
        {
            try
            {
                con = getConnection();
                Console.WriteLine("Enter EmpId to update salary:");
                int empid = Convert.ToInt32(Console.ReadLine());

                cmd = new SqlCommand("UpdateSalaryByEmpId", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmpId", empid);

                SqlParameter updatedSalary = new SqlParameter("@UpdatedSalary", SqlDbType.Decimal)
                { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(updatedSalary);
                cmd.ExecuteNonQuery();

                Console.WriteLine("Updated Salary : " + updatedSalary.Value);

                cmd = new SqlCommand("SELECT * FROM Employee_Details WHERE Empid=@id", con);
                cmd.Parameters.AddWithValue("@id", empid);
                dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Console.WriteLine("Empid : " + dr["Empid"]);
                        Console.WriteLine("Name : " + dr["Name"]);
                        Console.WriteLine("Salary : " + dr["Salary"]);
                        Console.WriteLine("Net Salary : " + dr["NetSalary"]);
                        Console.WriteLine("Gender : " + dr["Gender"]);
                    }
                }
                else
                    Console.WriteLine("No record found for given EmpId.");

                dr.Close();
                con.Close();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
