using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Mini_Project
{
    public static class TrainModule
    {
        public static void RegisterNewTrain()
        {
            Console.WriteLine(" Register a New Train");
            Console.Write("Train Name: ");
            string name = Console.ReadLine();
            Console.Write("Source Station: ");
            string source = Console.ReadLine();
            Console.Write("Destination Station: ");
            string destination = Console.ReadLine();
            Console.Write("Sleeper Seats: ");
            int sleeper = int.Parse(Console.ReadLine());
            Console.Write("2nd AC Seats: ");
            int ac2 = int.Parse(Console.ReadLine());
            Console.Write("3rd AC Seats: ");
            int ac3 = int.Parse(Console.ReadLine());
            Console.Write("Sleeper Fare: ₹");
            decimal fareSleeper = decimal.Parse(Console.ReadLine());
            Console.Write("2nd AC Fare: ₹");
            decimal fare2AC = decimal.Parse(Console.ReadLine());
            Console.Write("3rd AC Fare: ₹");
            decimal fare3AC = decimal.Parse(Console.ReadLine());

            using (SqlConnection conn = DBHelper.GetConnection())
            {
                string query = @"INSERT INTO Train (TrainName, Source, Destination, SleeperSeats, SecondACSeats, ThirdACSeats, SleeperFare, SecondACFare, ThirdACFare, IsActive)
                                 VALUES (@Name, @Source, @Destination, @Sleeper, @AC2, @AC3, @FareSleeper, @Fare2AC, @Fare3AC, 1)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Source", source);
                    cmd.Parameters.AddWithValue("@Destination", destination);
                    cmd.Parameters.AddWithValue("@Sleeper", sleeper);
                    cmd.Parameters.AddWithValue("@AC2", ac2);
                    cmd.Parameters.AddWithValue("@AC3", ac3);
                    cmd.Parameters.AddWithValue("@FareSleeper", fareSleeper);
                    cmd.Parameters.AddWithValue("@Fare2AC", fare2AC);
                    cmd.Parameters.AddWithValue("@Fare3AC", fare3AC);

                    int rows = cmd.ExecuteNonQuery();
                    Console.WriteLine(rows > 0 ? " Train registered successfully." : " Failed to register train.");
                }
            }
        }

        public static void ModifyTrainDetails()
        {
            Console.Write("Enter Train ID to modify: ");
            if (!int.TryParse(Console.ReadLine(), out int trainId))
            {
                Console.WriteLine(" Invalid Train ID.");
                return;
            }

            bool exitUpdate = false;

            while (!exitUpdate)
            {
                Console.Clear();
                Console.WriteLine($" Modify Details for Train ID: {trainId}");
                Console.WriteLine("1. Change Train Name");
                Console.WriteLine("2. Update Fare");
                Console.WriteLine("3. Change Source & Destination");
                Console.WriteLine("4. Update Berth Availability by Class");
                Console.WriteLine("5. Exit Modification");
                Console.Write("Choose option: ");
                string choice = Console.ReadLine();

                using (SqlConnection conn = DBHelper.GetConnection())
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.Parameters.AddWithValue("@TrainNo", trainId);

                    switch (choice)
                    {
                        case "1":
                            Console.Write("New Train Name: ");
                            string newName = Console.ReadLine();
                            cmd.CommandText = "UPDATE Train SET TrainName = @Value WHERE TrainNo = @TrainNo";
                            cmd.Parameters.AddWithValue("@Value", newName);
                            break;

                        case "2":
                            Console.Write("New Sleeper Fare: Rs");
                            decimal sleeperFare = decimal.Parse(Console.ReadLine());
                            Console.Write("New 2nd AC Fare: Rs");
                            decimal ac2Fare = decimal.Parse(Console.ReadLine());
                            Console.Write("New 3rd AC Fare: Rs");
                            decimal ac3Fare = decimal.Parse(Console.ReadLine());
                            cmd.CommandText = @"UPDATE Train SET SleeperFare = @SF, SecondACFare = @AC2F, ThirdACFare = @AC3F WHERE TrainNo = @TrainNo";
                            cmd.Parameters.AddWithValue("@SF", sleeperFare);
                            cmd.Parameters.AddWithValue("@AC2F", ac2Fare);
                            cmd.Parameters.AddWithValue("@AC3F", ac3Fare);
                            break;

                        case "3":
                            Console.Write("New Source: ");
                            string newSource = Console.ReadLine();
                            Console.Write("New Destination: ");
                            string newDestination = Console.ReadLine();
                            cmd.CommandText = "UPDATE Train SET Source = @Source, Destination = @Destination WHERE TrainNo = @TrainNo";
                            cmd.Parameters.AddWithValue("@Source", newSource);
                            cmd.Parameters.AddWithValue("@Destination", newDestination);
                            break;

                        case "4":
                            Console.Write("Which class to update (Sleeper / 2nd AC / 3rd AC): ");
                            string cls = Console.ReadLine();
                            Console.Write("New seat count: ");
                            int seats = int.Parse(Console.ReadLine());
                            string column = "";

                            switch (cls)
                            {
                                case "Sleeper":
                                    column = "SleeperSeats";
                                    break;
                                case "2nd AC":
                                    column = "SecondACSeats";
                                    break;
                                case "3rd AC":
                                    column = "ThirdACSeats";
                                    break;
                                default:
                                    Console.WriteLine(" Invalid class entered.");
                                    return;
                            }

                            cmd.CommandText = $"UPDATE Train SET {column} = @Seats WHERE TrainNo = @TrainNo";
                            cmd.Parameters.AddWithValue("@Seats", seats);
                            break;

                        case "5":
                            exitUpdate = true;
                            Console.WriteLine(" Exiting modification menu...");
                            continue;

                        default:
                            Console.WriteLine(" Invalid choice.");
                            continue;
                    }

                    int rows = cmd.ExecuteNonQuery();
                    Console.WriteLine(rows > 0 ? " Train details updated." : " Update failed.");
                }

                if (!exitUpdate)
                {
                    Console.WriteLine("\nPress Enter to continue...");
                    Console.ReadLine();
                }
            }
        }

        public static void SuspendTrain()
        {
            Console.Write("Enter Train ID to suspend: ");
            if (!int.TryParse(Console.ReadLine(), out int trainId))
            {
                Console.WriteLine(" Invalid Train ID.");
                return;
            }

            using (SqlConnection conn = DBHelper.GetConnection())
            {
                string updateQuery = "UPDATE Train SET IsActive = 0 WHERE TrainNo = @TrainNo";
                using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@TrainNo", trainId);
                    int rows = cmd.ExecuteNonQuery();
                    Console.WriteLine(rows > 0 ? "Train suspended successfully." : " Suspension failed.");
                }
            }

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        public static void RestoreSuspendedTrain()
        {
            Console.Write("Enter Train ID to reactivate: ");
            if (!int.TryParse(Console.ReadLine(), out int trainId))
            {
                Console.WriteLine(" Invalid Train ID.");
                return;
            }

            using (SqlConnection conn = DBHelper.GetConnection())
            {
                string updateQuery = "UPDATE Train SET IsActive = 1 WHERE TrainNo = @TrainNo";
                using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@TrainNo", trainId);
                    int rows = cmd.ExecuteNonQuery();
                    Console.WriteLine(rows > 0 ? " Train reactivated successfully." : " Reactivation failed.");
                }
            }

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        public static void ListAllTrains()
        {
            Console.WriteLine(" All Registered Trains:");
            using (SqlConnection conn = DBHelper.GetConnection())
            {
                string query = "SELECT TrainNo, TrainName, Source, Destination, SleeperSeats, SecondACSeats, ThirdACSeats, SleeperFare, SecondACFare, ThirdACFare, IsActive FROM Train";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"\nTrain ID: {reader["TrainNo"]}");
                        Console.WriteLine($"Name: {reader["TrainName"]}");
                        Console.WriteLine($"Route: {reader["Source"]} To {reader["Destination"]}");
                        Console.WriteLine($"Sleeper Seats: {reader["SleeperSeats"]} | Fare: Rs{reader["SleeperFare"]}");
                        Console.WriteLine($"2nd AC Seats: {reader["SecondACSeats"]} | Fare: Rs{reader["SecondACFare"]}");
                        Console.WriteLine($"3rd AC Seats: {reader["ThirdACSeats"]} | Fare: Rs{reader["ThirdACFare"]}");
                        Console.WriteLine($"Status: {(Convert.ToBoolean(reader["IsActive"]) ? " Active" : "Suspended")}");
                    }
                }
            }

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }
    }
}