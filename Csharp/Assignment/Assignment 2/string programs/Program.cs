using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace string_program_1
{
    class Program
    {
        static void Main(string[] args)
        {
            DisplayLength();
            Reverse();
            CheckIfSame();
        }
        public static void DisplayLength()
        {
            Console.WriteLine("===Display of length==="); 
            Console.Write("\nEnter a word: ");
                string word = Console.ReadLine();

                int length = word.Length;

                Console.WriteLine($"Length of the word \"{word}\" is: {length}");
            Console.ReadLine();
            }

        public static void Reverse()
        {
            Console.WriteLine("=== Reverse of Word===");
                Console.Write("\nEnter a word: ");
                string word = Console.ReadLine();

                string reversed = "";

                for (int i = word.Length - 1; i >= 0; i--)
                {
                    reversed += word[i];
                }

                Console.WriteLine($"Reversed word: {reversed}");
            Console.ReadLine();
            }

        public static void CheckIfSame()
      
            {
            Console.WriteLine("=== Check if same===");
                Console.Write("\nEnter first word: ");
                string word1 = Console.ReadLine();

                Console.Write("Enter second word: ");
                string word2 = Console.ReadLine();

                if (word1.Equals(word2))
                {
                    Console.WriteLine("The words are the same.");
                }
                else
                {
                    Console.WriteLine("The words are different.");
                    
                }
            Console.ReadLine();
        }
        }

    }



