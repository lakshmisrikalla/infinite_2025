using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mini_Project
{
    public static class AdminModule
    {
        public static void AdminMenu(int adminId)
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("===  Admin Control Panel ===");
                Console.WriteLine("1. Register New Train");
                Console.WriteLine("2. Modify Train Details");
                Console.WriteLine("3. Suspend Train");
                Console.WriteLine("4. Reactivate Train");
                Console.WriteLine("5. View All Trains");
                Console.WriteLine("6. View All Reservations");
                Console.WriteLine("7. View Revenue Report");
                Console.WriteLine("8. Exit to Main Menu");
                Console.Write("Choose option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": TrainModule.RegisterNewTrain(); break;
                    case "2": TrainModule.ModifyTrainDetails(); break;
                    case "3": TrainModule.SuspendTrain(); break;
                    case "4": TrainModule.RestoreSuspendedTrain(); break;
                    case "5": TrainModule.ListAllTrains(); break;
                    case "6": ReportModule.ViewAllReservations(); break;
                    case "7": ReportModule.ViewRevenueReport(); break;
                    case "8":
                        exit = true;
                        Console.WriteLine("Returning to main menu...");
                        Program.ShowMainMenu(); 
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }

                if (!exit)
                {
                    Console.WriteLine("\nPress Enter to return to the menu...");
                    Console.ReadLine();
                }
            }
        }
    }

}