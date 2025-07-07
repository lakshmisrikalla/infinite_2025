using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class TravelFare
    {
            public string CalculateConcession(int age, double TotalFare)
            {
                if (age <= 5)
                {
                    return "Little Champs - Free Ticket";
                }
                else if (age > 60)
                {
                    double concessionFare = TotalFare * 0.7; 
                    return $"Senior Citizen - Fare: {concessionFare}";
                }
                else
                {
                    return $"Ticket Booked - Fare: {TotalFare}";
                }
            }
        }
    }