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
        public static bool AdminLogin(string username, string password)
        {
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
                           Console.ReadLine(); 
                            AdminModule.AdminMenu(adminId);
                            reader.Close();
                            return true;
                        }
                        else
                        {
                             Console.WriteLine(" Invalid credentials.");
                            return false;
                        }
                    }
                }
            }
        }


        public static bool CustomerLogin(string username, string password)
        {
           

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
                            int custId = Convert.ToInt32(reader["CustID"]);
                            Console.WriteLine(" Customer login successful.");
                            return true;
                            // Console.ReadLine();
                            // CustomerModule.CustomerMenu(custId);
                            // reader.Close();

                        }
                        else
                        {
                            Console.WriteLine(" Invalid credentials.");
                            Console.ReadLine();
                            return false;
                        }
                    }
                }
            }
        }

        public static void RegisterCustomer()
        {
            Console.WriteLine(" Register New Customer");

            Console.Write("Full Name: ");
            string name = Console.ReadLine();
            Console.Write("Phone Number: ");
            string phone = Console.ReadLine();
            Console.Write("Email ID: ");
            string email = Console.ReadLine();
            Console.Write("Choose Username: ");
            string username = Console.ReadLine();
            Console.Write("Choose Password: ");
            string password = Console.ReadLine();

            using (SqlConnection conn = DBHelper.GetConnection())
            {
                string query = @"INSERT INTO Customer (CustName, Phone, MailID, Username, Password)
                                 VALUES (@Name, @Phone, @Email, @Username, @Password)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Phone", phone);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);

                    try
                    {
                        int rows = cmd.ExecuteNonQuery();
                        Console.WriteLine(rows > 0 ? " Registration successful!" : " Registration failed.");
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 2627) // Unique constraint violation
                            Console.WriteLine(" Username already exists. Try a different one.");
                        else
                            Console.WriteLine($" Error: {ex.Message}");
                    }
                }
            }

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
            Program.ShowMainMenu(); // Return to main menu after registration
        }
    }

}
