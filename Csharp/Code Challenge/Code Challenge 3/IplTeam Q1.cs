using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code_Challenge_3
{

    class CricketTeam
    {
        public void PointsCalculation(int no_of_matches)
        {
            int[] scores = new int[no_of_matches];
            int sum = 0;

            for (int i = 0; i < no_of_matches; i++)
            {
                Console.Write($"Enter score for match {i + 1}: ");
                scores[i] = Convert.ToInt32(Console.ReadLine());
                sum += scores[i];
            }

            double average = (double)sum / no_of_matches;

            Console.WriteLine("\n--- Results ---");
            Console.WriteLine($"Total Matches Played: {no_of_matches}");
            Console.WriteLine($"Sum of Scores: {sum}");
            Console.WriteLine($"Average Score: {average:F2}");
            Console.ReadLine();
        }
    }

    class IplTeam_Q1
    {
        static void Main(string[] args)
        {
            Console.Write("Enter the number of matches played: ");
            int matches = Convert.ToInt32(Console.ReadLine());

            CricketTeam team = new CricketTeam();
            team.PointsCalculation(matches);
            

        }
       
    }
   
}