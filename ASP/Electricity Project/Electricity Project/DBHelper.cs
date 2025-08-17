using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;


namespace Electricity_Project
{
    public class DBHelper
    {
       
        public static SqlConnection GetConnection()
        {
            SqlConnection con = new SqlConnection("Data Source=ICS-LT-2WS9J84\\SQLEXPRESS;Initial Catalog=ElectricityProject;" +
            "user ID=sa;Password=Lakshmi@1302!");
            con.Open();
            return con;
        }
    }
}