using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;



namespace Electricity_Project
{
    public class ElectricityValuation
    {
        public void CalculateBill(ElectricityBill ebill)
        {

            SqlConnection con = DBHelper.GetConnection();

            int units = ebill.UnitsConsumed;
            double bill = 0;

            if (units <= 100)
                bill = 0;
            else if (units <= 300)
                bill = (units - 100) * 1.5;
            else if (units <= 600)
                bill = 200 * 1.5 + (units - 300) * 3.5;
            else if (units <= 1000)
                bill = 200 * 1.5 + 300 * 3.5 + (units - 600) * 5.5;
            else
                bill = 200 * 1.5 + 300 * 3.5 + 400 * 5.5 + (units - 1000) * 7.5;

            ebill.BillAmount = bill;
        }

        public void AddBill(ElectricityBill ebill)
        {
            using (SqlConnection con = DBHelper.GetConnection())
            {
                // Check if consumer number already exists
                string checkQuery = "SELECT consumer_name FROM ElectricityBill WHERE consumer_number = @num";
                SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                checkCmd.Parameters.AddWithValue("@num", ebill.ConsumerNumber);

                object result = checkCmd.ExecuteScalar();

                if (result != null)
                {
                    string existingName = result.ToString();
                    if (!string.Equals(existingName, ebill.ConsumerName, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException($"Consumer Number '{ebill.ConsumerNumber}' already exists with a different name: '{existingName}'.");
                    }
                }

                // Insert bill
                string insertQuery = "INSERT INTO ElectricityBill (consumer_number, consumer_name, units_consumed, bill_amount) VALUES (@num, @name, @units, @amount)";
                SqlCommand insertCmd = new SqlCommand(insertQuery, con);
                insertCmd.Parameters.AddWithValue("@num", ebill.ConsumerNumber);
                insertCmd.Parameters.AddWithValue("@name", ebill.ConsumerName);
                insertCmd.Parameters.AddWithValue("@units", ebill.UnitsConsumed);
                insertCmd.Parameters.AddWithValue("@amount", ebill.BillAmount);
                insertCmd.ExecuteNonQuery();
            }
        }

        public List<ElectricityBill> GetBillsByConsumerNumber(string consumerNumber, out string consumerName)
        {
            List<ElectricityBill> bills = new List<ElectricityBill>();
            consumerName = null;

            using (SqlConnection con = DBHelper.GetConnection())
            {
                string query = "SELECT consumer_name, units_consumed, bill_amount FROM ElectricityBill WHERE consumer_number = @num";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@num", consumerNumber);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (consumerName == null)
                        consumerName = reader["consumer_name"].ToString();

                    ElectricityBill eb = new ElectricityBill
                    {
                        ConsumerNumber = consumerNumber,
                        ConsumerName = consumerName,
                        UnitsConsumed = Convert.ToInt32(reader["units_consumed"]),
                        BillAmount = Convert.ToDouble(reader["bill_amount"])
                    };
                    bills.Add(eb);
                }
            }

            return bills;
        }

        public List<ElectricityBill> Generate_N_BillDetails(int n)
        {
            List<ElectricityBill> bills = new List<ElectricityBill>();
            using (SqlConnection con = DBHelper.GetConnection())
            {
                string query = "SELECT TOP (@n) consumer_number, consumer_name, units_consumed, bill_amount FROM ElectricityBill ORDER BY bill_id DESC";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@n", n);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ElectricityBill eb = new ElectricityBill
                    {
                        ConsumerNumber = reader["consumer_number"].ToString(),
                        ConsumerName = reader["consumer_name"].ToString(),
                        UnitsConsumed = Convert.ToInt32(reader["units_consumed"]),
                        BillAmount = Convert.ToDouble(reader["bill_amount"])
                    };
                    bills.Add(eb);
                }
            }
            return bills;

        }
    }

}