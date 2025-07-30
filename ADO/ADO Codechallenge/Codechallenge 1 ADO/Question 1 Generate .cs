using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Codechallenge_1_ADO
{
    class InsertEmployeeDetailsProgram
    {
        public static SqlConnection con;
        public static SqlCommand cmd;
        public static SqlDataReader dr;

        static void Main(string[] args)
        {
            InsertProcedure();
            Console.Read();
        }

        static SqlConnection getConnection()
            {
            con = new SqlConnection("Data Source=ICS-LT-2WS9J84\\SQLEXPRESS;Initial Catalog=Codechallenge;" +
            "user ID=sa;Password=Lakshmi@1302!");
            con.Open();
            return con;
        }

            static void InsertProcedure()
            {
                try
                {
                    con = getConnection();
                    Console.WriteLine("Enter Name:");
                    string name = Console.ReadLine();
                    Console.WriteLine("Enter Given Salary:");
                    decimal givenSalary = Convert.ToDecimal(Console.ReadLine());
                    Console.WriteLine("Enter Gender:");
                    string gender = Console.ReadLine();

                    cmd = new SqlCommand("InsertEmployeeDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@GivenSalary", givenSalary);
                    cmd.Parameters.AddWithValue("@Gender", gender);

                    SqlParameter empOut = new SqlParameter("@GeneratedEmpId", SqlDbType.Int)
                    { Direction = ParameterDirection.Output };
                    SqlParameter salOut = new SqlParameter("@FinalSalary", SqlDbType.Decimal)
                    { Direction = ParameterDirection.Output };

                    cmd.Parameters.Add(empOut);
                    cmd.Parameters.Add(salOut);

                    cmd.ExecuteNonQuery();

                    Console.WriteLine("Generated EmpId : " + empOut.Value);
                    Console.WriteLine("Inserted Salary : " + salOut.Value);

                    cmd = new SqlCommand("SELECT NetSalary FROM Employee_Details WHERE Empid = @eid", con);
                    cmd.Parameters.AddWithValue("@eid", empOut.Value);
                    dr = cmd.ExecuteReader();
                    if (dr.Read())
                        Console.WriteLine("Net Salary : " + dr["NetSalary"]);
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
