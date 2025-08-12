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

            int trainId;
            SqlDataReader reader = null;
            SqlConnection conn = DBHelper.GetConnection();

            // Validate Train ID and fetch train details
            while (true)
            {
                Console.Write("Enter Train ID: ");
                if (!int.TryParse(Console.ReadLine(), out trainId))
                {
                    Console.WriteLine(" Invalid Train ID format.");
                    continue;
                }

                string checkQuery = "SELECT * FROM Train WHERE TrainNo = @TrainNo AND IsActive = 1";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@TrainNo", trainId);
                    reader = checkCmd.ExecuteReader();

                    if (!reader.Read())
                    {
                        Console.WriteLine(" Train not found or inactive. Please re-enter.");
                        reader.Close();
                        continue;
                    }
                    break;
                }
            }

            string cls = "";
            int seats = 0;
            decimal fare = 0;
            string seatColumn = "";

            // Validate Class and seat availability
            while (true)
            {
                Console.Write("Enter Class (Sleeper / 2nd AC / 3rd AC): ");
                cls = Console.ReadLine();

                switch (cls)
                {
                    case "Sleeper":
                        seats = Convert.ToInt32(reader["SleeperSeats"]);
                        fare = Convert.ToDecimal(reader["SleeperFare"]);
                        seatColumn = "SleeperSeats";
                        break;
                    case "2nd AC":
                        seats = Convert.ToInt32(reader["SecondACSeats"]);
                        fare = Convert.ToDecimal(reader["SecondACFare"]);
                        seatColumn = "SecondACSeats";
                        break;
                    case "3rd AC":
                        seats = Convert.ToInt32(reader["ThirdACSeats"]);
                        fare = Convert.ToDecimal(reader["ThirdACFare"]);
                        seatColumn = "ThirdACSeats";
                        break;
                    default:
                        Console.WriteLine(" Invalid class. Please re-enter.");
                        continue;
                }

                if (seats <= 0)
                {
                    Console.WriteLine(" No seats available in selected class. Please choose another class.");
                    continue;
                }

                break;
            }

            reader.Close();

            Console.Write("Enter Passenger Name: ");
            string passengerName = Console.ReadLine();

            int age;
            while (true)
            {
                Console.Write("Enter Passenger Age: ");
                if (!int.TryParse(Console.ReadLine(), out age))
                {
                    Console.WriteLine(" Invalid age. Please re-enter.");
                    continue;
                }
                break;
            }

            DateTime travelDate;
            while (true)
            {
                Console.Write("Enter Travel Date (yyyy-mm-dd): ");
                if (!DateTime.TryParse(Console.ReadLine(), out travelDate))
                {
                    Console.WriteLine(" Invalid date format. Please re-enter.");
                    continue;
                }

                if (travelDate.Date < DateTime.Today)
                {
                    Console.WriteLine(" Travel date cannot be in the past. Please re-enter.");
                    continue;
                }

                break;
            }

            Console.Write("Enter Berth Preference (e.g., Lower, Middle, Upper): ");
            string berth = Console.ReadLine();

            decimal finalFare = fare;
            if (age <= 5)
                finalFare = 0;
            else if (age >= 60)
                finalFare *= 0.5m;

            string insertQuery = @"
            INSERT INTO Reservation (
                CustID, TrainNo, PassengerName, TravelDate, Class, BerthAllotment, TotalCost
            )
            VALUES (
                @CustID, @TrainNo, @PassengerName, @TravelDate, @Class, @BerthAllotment, @TotalCost
            )";

            using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
            {
                insertCmd.Parameters.AddWithValue("@CustID", customerId);
                insertCmd.Parameters.AddWithValue("@TrainNo", trainId);
                insertCmd.Parameters.AddWithValue("@PassengerName", passengerName);
                insertCmd.Parameters.AddWithValue("@TravelDate", travelDate);
                insertCmd.Parameters.AddWithValue("@Class", cls);
                insertCmd.Parameters.AddWithValue("@BerthAllotment", berth);
                insertCmd.Parameters.AddWithValue("@TotalCost", finalFare);

                int rows = insertCmd.ExecuteNonQuery();
                Console.WriteLine(rows > 0 ? $" Ticket booked successfully. Fare: Rs{finalFare}" : " Booking failed.");
            }

            string updateSeats = $"UPDATE Train SET {seatColumn} = {seatColumn} - 1 WHERE TrainNo = @TrainNo";
            using (SqlCommand updateCmd = new SqlCommand(updateSeats, conn))
            {
                updateCmd.Parameters.AddWithValue("@TrainNo", trainId);
                updateCmd.ExecuteNonQuery();
            }

            conn.Close();
        }
    }
}