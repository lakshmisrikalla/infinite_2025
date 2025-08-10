using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Mini_Project
{
        public static class ReservationModule
        {
            public static void BookTicket(int customerId)
            {
                Console.WriteLine(" Book a Ticket");

                Console.Write("Enter Train ID: ");
                if (!int.TryParse(Console.ReadLine(), out int trainId))
                {
                    Console.WriteLine(" Invalid Train ID.");
                    return;
                }

                Console.Write("Enter Class (Sleeper / 2nd AC / 3rd AC): ");
                string cls = Console.ReadLine();

                Console.Write("Enter Passenger Name: ");
                string passengerName = Console.ReadLine();

                Console.Write("Enter Passenger Age: ");
                if (!int.TryParse(Console.ReadLine(), out int age))
                {
                    Console.WriteLine(" Invalid age.");
                    return;
                }

                Console.Write("Enter Travel Date (yyyy-mm-dd): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime travelDate))
                {
                    Console.WriteLine(" Invalid date format.");
                    return;
                }

                Console.Write("Enter Berth Preference (e.g., Lower, Middle, Upper): ");
                string berth = Console.ReadLine();

                using (SqlConnection conn = DBHelper.GetConnection())
                {
                    // Check train availability
                    string checkQuery = "SELECT * FROM Train WHERE TrainNo = @TrainNo AND IsActive = 1";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@TrainNo", trainId);

                        using (SqlDataReader reader = checkCmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                Console.WriteLine(" Train not found or inactive.");
                                return;
                            }

                            int seats = 0;
                            decimal fare = 0;
                            switch (cls)
                            {
                                case "Sleeper":
                                    seats = Convert.ToInt32(reader["SleeperSeats"]);
                                    fare = Convert.ToDecimal(reader["SleeperFare"]);
                                    break;
                                case "2nd AC":
                                    seats = Convert.ToInt32(reader["SecondACSeats"]);
                                    fare = Convert.ToDecimal(reader["SecondACFare"]);
                                    break;
                                case "3rd AC":
                                    seats = Convert.ToInt32(reader["ThirdACSeats"]);
                                    fare = Convert.ToDecimal(reader["ThirdACFare"]);
                                    break;
                                default:
                                    Console.WriteLine(" Invalid class.");
                                    return;
                            }

                            if (seats <= 0)
                            {
                                Console.WriteLine(" No seats available in selected class.");
                                return;
                            }

                            // Apply age-based concession
                            decimal finalFare = fare;
                            if (age <= 5)
                                finalFare = 0;
                            else if (age >= 60)
                                finalFare *= 0.5m;

                            reader.Close();

                            // Insert reservation (without TrainNo)
                            string insertQuery = @"
                            INSERT INTO Reservation (
                                CustID, PassengerName, TravelDate, Class, BerthAllotment, TotalCost
                            )
                            VALUES (
                                @CustID, @PassengerName, @TravelDate, @Class, @BerthAllotment, @TotalCost
                            )";

                            using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                            {
                                insertCmd.Parameters.AddWithValue("@CustID", customerId);
                                insertCmd.Parameters.AddWithValue("@PassengerName", passengerName);
                                insertCmd.Parameters.AddWithValue("@TravelDate", travelDate);
                                insertCmd.Parameters.AddWithValue("@Class", cls);
                                insertCmd.Parameters.AddWithValue("@BerthAllotment", berth);
                                insertCmd.Parameters.AddWithValue("@TotalCost", finalFare);

                                int rows = insertCmd.ExecuteNonQuery();
                                Console.WriteLine(rows > 0 ? $" Ticket booked successfully. Fare: Rs{finalFare}" : " Booking failed.");
                            }

                            // Update seat count
                            string seatColumn = cls == "Sleeper" ? "SleeperSeats" :
                                                cls == "2nd AC" ? "SecondACSeats" :
                                                "ThirdACSeats";

                            string updateSeats = $"UPDATE Train SET {seatColumn} = {seatColumn} - 1 WHERE TrainNo = @TrainNo";
                            using (SqlCommand updateCmd = new SqlCommand(updateSeats, conn))
                            {
                                updateCmd.Parameters.AddWithValue("@TrainNo", trainId);
                                updateCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }
    }

