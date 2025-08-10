using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Mini_Project
{
    public static class CancellationModule
    {
        public static void CancelTicket(int customerId)
        {
            Console.Write("Enter Booking ID to cancel: ");
            if (!int.TryParse(Console.ReadLine(), out int bookingId))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            using (SqlConnection conn = DBHelper.GetConnection())
            {
                // Check reservation
                string query = @"SELECT TotalCost FROM Reservation 
                             WHERE BookingID = @BookingID AND CustID = @CustID AND IsCancelled = 0";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BookingID", bookingId);
                cmd.Parameters.AddWithValue("@CustID", customerId);

                SqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    Console.WriteLine(" Reservation not found or already cancelled.");
                    return;
                }

                decimal fare = Convert.ToDecimal(reader["TotalCost"]);
                reader.Close();

                decimal refund = fare * 0.5m;

                // Mark reservation as cancelled
                string updateReservation = @"UPDATE Reservation SET IsCancelled = 1 WHERE BookingID = @BookingID";
                SqlCommand updateCmd = new SqlCommand(updateReservation, conn);
                updateCmd.Parameters.AddWithValue("@BookingID", bookingId);
                updateCmd.ExecuteNonQuery();

                // Insert into Cancellation table
                string insertCancel = @"INSERT INTO Cancellation (BookingID, RefundAmount) 
                                    VALUES (@BookingID, @RefundAmount)";
                SqlCommand cancelCmd = new SqlCommand(insertCancel, conn);
                cancelCmd.Parameters.AddWithValue("@BookingID", bookingId);
                cancelCmd.Parameters.AddWithValue("@RefundAmount", refund);
                cancelCmd.ExecuteNonQuery();

                Console.WriteLine($" Ticket cancelled. Refund: Rs{refund}");
            }
        }

        public static void ViewRefundDetails(int customerId)
        {
            Console.WriteLine(" Refund Details:");

            using (SqlConnection conn = DBHelper.GetConnection())
            {
                string query = @"SELECT C.BookingID, R.PassengerName, C.RefundAmount, C.CancellationDate
                             FROM Cancellation C
                             JOIN Reservation R ON C.BookingID = R.BookingID
                             WHERE R.CustID = @CustID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CustID", customerId);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"\nBooking ID: {reader["BookingID"]}");
                    Console.WriteLine($"Passenger: {reader["PassengerName"]}");
                    Console.WriteLine($"Refund Amount: Rs{reader["RefundAmount"]}");
                    Console.WriteLine($"Cancelled On: {Convert.ToDateTime(reader["CancellationDate"]).ToShortDateString()}");
                }
            }

            Console.WriteLine("\nPress Enter to continue...");
            Console.ReadLine();
        }
    }
}