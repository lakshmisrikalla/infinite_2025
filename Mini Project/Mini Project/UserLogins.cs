using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Mini_Project
{
    public static class UserLogins
    {
        public static void AdminLogin()
        {
            Console.Write("Enter admin username: ");
            string username = Console.ReadLine();

            Console.Write("Enter admin password: ");
            string password = Console.ReadLine();

            using (SqlConnection conn = DBHelper.GetConnection())
            {
                string query = "SELECT AdminID FROM Admin WHERE Username = @Username AND Password = @Password";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int adminId = Convert.ToInt32(reader["AdminID"]);
                            Console.WriteLine(" Admin login successful.");
                            reader.Close();
                            AdminModule.AdminMenu(adminId);
                        }
                        else
                        {
                            Console.WriteLine(" Invalid credentials.");
                        }
                    }
                }
            }
        }

        public static void CustomerLogin()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            using (SqlConnection conn = DBHelper.GetConnection())
            {
                string query = "SELECT CustID FROM Customer WHERE Username = @Username AND Password = @Password";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int CustId = Convert.ToInt32(reader["CustID"]);
                            Console.WriteLine(" Customer login successful.");
                            reader.Close();
                            CustomerModule.CustomerMenu(CustId);
                        }
                        else
                        {
                            Console.WriteLine(" Invalid credentials.");
                        }
                    }
                }
            }
        }
    }
}