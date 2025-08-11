using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Mini_Project
{
        public static class ReportModule
        {
            public static void ViewCustomerReservations(int custId)
            {
                Console.WriteLine(" Your Reservations:");

                using (SqlConnection conn = DBHelper.GetConnection())
                {
                    string query = @"SELECT R.BookingID, R.PassengerName, R.TravelDate, R.Class, R.BerthAllotment, R.TotalCost, R.IsCancelled, R.BookingDate
                                 FROM Reservation R
                                 WHERE R.CustID = @CustID
                                 ORDER BY R.BookingDate DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@CustID", custId);

                    SqlDataReader reader = cmd.ExecuteReader();
                    bool hasRows = false;
                    while (reader.Read())
                    {
                        hasRows = true;
                        string status = Convert.ToBoolean(reader["IsCancelled"]) ? "Cancelled" : "Confirmed";

                        Console.WriteLine($"\nBooking ID: {reader["BookingID"]}");
                        Console.WriteLine($"Passenger: {reader["PassengerName"]}");
                        Console.WriteLine($"Travel Date: {Convert.ToDateTime(reader["TravelDate"]).ToShortDateString()}");
                        Console.WriteLine($"Class: {reader["Class"]} | Berth: {reader["BerthAllotment"]}");
                        Console.WriteLine($"Fare: Rs{reader["TotalCost"]} | Status: {status}");
                        Console.WriteLine($"Booked On: {Convert.ToDateTime(reader["BookingDate"]).ToShortDateString()}");
                    }

                    if (!hasRows)
                    {
                        Console.WriteLine(" No reservations found.");
                    }
                }

                Console.WriteLine("\nPress Enter to continue...");
                Console.ReadLine();
            }

        public static void ViewAllReservations()
        {
            Console.WriteLine(" All Reservations:");

            using (SqlConnection conn = DBHelper.GetConnection())
            {
                string query = @"
            SELECT R.BookingID, R.CustID, C.CustName, R.PassengerName, R.TravelDate, R.Class, R.BerthAllotment, 
                   R.TotalCost, R.IsCancelled, R.BookingDate, Can.RefundAmount, Can.CancellationDate
            FROM Reservation R
            JOIN Customer C ON R.CustID = C.CustID
            LEFT JOIN Cancellation Can ON R.BookingID = Can.BookingID
            ORDER BY R.BookingDate DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    bool isCancelled = reader["IsCancelled"] != DBNull.Value && Convert.ToBoolean(reader["IsCancelled"]);
                    string status = isCancelled ? " Cancelled" : " Confirmed";

                    Console.WriteLine($"\nBooking ID: {reader["BookingID"]}");
                    Console.WriteLine($"Customer: {reader["CustName"]} (ID: {reader["CustID"]})");
                    Console.WriteLine($"Passenger: {reader["PassengerName"]}");
                    Console.WriteLine($"Travel Date: {Convert.ToDateTime(reader["TravelDate"]).ToShortDateString()}");
                    Console.WriteLine($"Class: {reader["Class"]} | Berth: {reader["BerthAllotment"]}");
                    Console.WriteLine($"Fare: Rs{reader["TotalCost"]} | Status: {status}");
                    Console.WriteLine($"Booked On: {Convert.ToDateTime(reader["BookingDate"]).ToShortDateString()}");

                    if (isCancelled)
                    {
                        decimal refund = reader["RefundAmount"] != DBNull.Value ? Convert.ToDecimal(reader["RefundAmount"]) : 0;
                        DateTime cancelDate = reader["CancellationDate"] != DBNull.Value ? Convert.ToDateTime(reader["CancellationDate"]) : DateTime.MinValue;

                        Console.WriteLine($"Refund: Rs{refund}");
                        Console.WriteLine($"Cancelled On: {cancelDate.ToShortDateString()}");
                    }
                }
            }

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }


        public static void ViewRevenueReport()
        {
            Console.WriteLine(" Revenue Report by Class (Includes Cancelled):");

            decimal totalRevenue = 0;

            using (SqlConnection conn = DBHelper.GetConnection())
            {
                string query = @"
            SELECT 
                Class,
                SUM(CASE WHEN IsCancelled = 0 THEN TotalCost ELSE 0 END) AS ConfirmedRevenue,
                SUM(CASE WHEN IsCancelled = 1 THEN TotalCost * 0.5 ELSE 0 END) AS CancelledRevenue,
                COUNT(*) AS TotalBookings
            FROM Reservation
            GROUP BY Class
            ORDER BY Class";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string cls = reader["Class"].ToString();
                    decimal confirmed = Convert.ToDecimal(reader["ConfirmedRevenue"]);
                    decimal cancelled = Convert.ToDecimal(reader["CancelledRevenue"]);
                    int bookings = Convert.ToInt32(reader["TotalBookings"]);

                    decimal classRevenue = confirmed + cancelled;
                    totalRevenue += classRevenue;

                    Console.WriteLine($"\nClass: {cls}");
                    Console.WriteLine($"Bookings: {bookings} | Revenue: Rs{classRevenue} (Confirmed: Rs{confirmed}, Cancelled: Rs{cancelled})");
                }
            }

            Console.WriteLine($"\n Total Revenue from All Classes: Rs{totalRevenue}");
            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }


        public static void ViewAvailableTrains()
        {
            Console.WriteLine(" Available Trains for Booking:");

            using (SqlConnection conn = DBHelper.GetConnection())
            {
                string query = @"SELECT TrainNo, TrainName, Source, Destination, SleeperSeats, SecondACSeats, ThirdACSeats, SleeperFare, SecondACFare, ThirdACFare
                         FROM Train
                         WHERE IsActive = 1";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    bool hasRows = false;
                    while (reader.Read())
                    {
                        hasRows = true;
                        Console.WriteLine($"\nTrain ID: {reader["TrainNo"]}");
                        Console.WriteLine($"Name: {reader["TrainName"]}");
                        Console.WriteLine($"Route: {reader["Source"]} To {reader["Destination"]}");
                        Console.WriteLine($"Sleeper: {reader["SleeperSeats"]} seats | Rs{reader["SleeperFare"]}");
                        Console.WriteLine($"2nd AC: {reader["SecondACSeats"]} seats | Rs{reader["SecondACFare"]}");
                        Console.WriteLine($"3rd AC: {reader["ThirdACSeats"]} seats | Rs{reader["ThirdACFare"]}");
                    }

                    if (!hasRows)
                    {
                        Console.WriteLine(" No active trains available.");
                    }
                }
            }

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

    }
}

