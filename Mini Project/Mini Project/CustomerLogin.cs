using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Mini_Project
{
        public static class CustomerModule
        {
            public static void CustomerMenu(int customerId)
            {
                bool exit = false;

                while (!exit)
                {
                    Console.Clear();
                    Console.WriteLine("\n===  Customer Menu ===");
                    Console.WriteLine("1. Book Ticket");
                    Console.WriteLine("2. Cancel Ticket");
                    Console.WriteLine("3. View My Reservations");
                    Console.WriteLine("4. View Available Trains");
                    Console.WriteLine("5. View Refund Details");
                    Console.WriteLine("6. Exit");
                    Console.Write("Choose option: ");
                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            ReservationModule.BookTicket(customerId);
                            break;
                        case "2":
                            CancellationModule.CancelTicket(customerId);
                            break;
                        case "3":
                            ReportModule.ViewCustomerReservations(customerId);
                            break;
                        case "4":
                            ReportModule.ViewAvailableTrains();
                            break;
                        case "5":
                            CancellationModule.ViewRefundDetails(customerId);
                            break;
                        case "6":
                            exit = true;
                            Console.WriteLine(" Exiting from customer menu...");
                            break;
                        default:
                            Console.WriteLine(" Invalid choice.");
                            break;
                    }

                    if (!exit)
                    {
                        Console.WriteLine("\nPress Enter to return to menu...");
                        Console.ReadLine();
                    }
                }
            }

        }
    }
