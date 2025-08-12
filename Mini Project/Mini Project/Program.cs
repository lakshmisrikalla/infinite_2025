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
            SqlConnection con = new SqlConnection("Data Source=ICS-LT-2WS9J84\\SQLEXPRESS;Initial Catalog=Miniproject;" +
            "user ID=sa;Password=Lakshmi@1302!");
            con.Open();
            return con;
        }
    }

        public class Program
        {
            public static void Main(string[] args)
            {
                ShowMainMenu();
            }

            public static void ShowMainMenu()
            {
                bool exitApp = false;

                while (!exitApp)
                {
                    Console.Clear();
                    Console.WriteLine("=== Railway Reservation System ===");
                    Console.WriteLine("1. Admin Login");
                    Console.WriteLine("2. Customer Login");
                    Console.WriteLine("3. Customer Registration");
                    Console.WriteLine("4. Sign Out");
                    Console.Write("Select an option (1-4): ");

                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            Console.Write("Enter admin username: ");
                            string adminUser = Console.ReadLine();

                            Console.Write("Enter admin password: ");
                            string adminPass = Console.ReadLine();

                            UserLogins.AdminLogin(adminUser, adminPass);
                            break;

                        case "2":
                        Console.Write("Enter username: ");
                        string username = Console.ReadLine();

                        Console.Write("Enter password: ");
                        string password = Console.ReadLine();
                        UserLogins.CustomerLogin(username,password);
                            break;

                        case "3":
                            UserLogins.RegisterCustomer();
                            break;

                        case "4":
                            Console.WriteLine("Signing out... Happy Travel!");
                            exitApp = true;
                            break;

                        default:
                            Console.WriteLine("Invalid choice. Press Enter to try again.");
                            
                            break;
                    }
                }
            }
        }
    }