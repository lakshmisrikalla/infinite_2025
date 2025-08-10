using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Mini_Project
{
    public class DBHelper
    {
        public static SqlConnection GetConnection()
        {
            SqlConnection con = new SqlConnection("Data Source=ICS-LT-2WS9J84\\SQLEXPRESS;Initial Catalog=Miniprojects;" +
            "user ID=sa;Password=Lakshmi@1302!");
            con.Open();
            return con;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Railway Reservation System ===");
            Console.WriteLine("1. Admin Login");
            Console.WriteLine("2. Customer Login");
            Console.WriteLine("3. Customer Registration");
            Console.Write("Select option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    UserLogins.AdminLogin();
                    break;
                case "2":
                    UserLogins.CustomerLogin();
                    break;
                case "3":
                    RegisterCustomer();
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
            Console.ReadLine();
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
    }
 }
}
