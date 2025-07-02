using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment_5
{
    public class InsufficientBalanceException : ApplicationException
    {
        public InsufficientBalanceException(string message) : base(message) { }
    }

    public class Accounts
    {
        public int AccountNumber { get; set; }
        public string CustomerName { get; set; }
        public string AccountType { get; set; }
        public char TransactionType { get; set; }
        public int TransactionAmount { get; set; }
        public int Balance { get; private set; }

        public Accounts(int accNo, string name, string accType, char transType, int amount, int balance)
        {
            AccountNumber = accNo;
            CustomerName = name;
            AccountType = accType;
            TransactionType = transType;
            TransactionAmount = amount;
            Balance = balance;

            try
            {
                if (TransactionType == 'c' || TransactionType == 'C')
                    Deposit(TransactionAmount);
                else if (TransactionType == 'W' || TransactionType == 'w')
                    Withdraw(TransactionAmount);
                else
                    Console.WriteLine("Invalid Transaction Type!");
            }
            catch (InsufficientBalanceException ex)
            {
                Console.WriteLine($"Transaction Failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
            }
        }

        public void Deposit(int amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.");

            Balance += amount;
        }

        public void Withdraw(int amount)
        {
            if (amount > Balance)
                throw new InsufficientBalanceException("Insufficient balance.");

            Balance -= amount;
        }

        public void ShowData()
        {
            Console.WriteLine($"\nAccount Summary:\nAccount No: {AccountNumber}\nCustomer: {CustomerName}\nAccount Type: {AccountType}\nBalance: {Balance}");
        }
        class MainProgram
        {
            static void Main()
            {
                try
                {
                    Console.WriteLine("===== Account Transaction =====");

                    Console.Write("Enter Account Number: ");
                    int accNo = int.Parse(Console.ReadLine());

                    Console.Write("Enter Customer Name: ");
                    string custName = Console.ReadLine();

                    Console.Write("Enter Account Type: ");
                    string accType = Console.ReadLine();

                    Console.Write("Enter Transaction Type (C/W): ");
                    char transType = char.Parse(Console.ReadLine());

                    Console.Write("Enter Transaction Amount: ");
                    int amount = int.Parse(Console.ReadLine());

                    Console.Write("Enter Initial Balance: ");
                    int balance = int.Parse(Console.ReadLine());

                    Accounts acc = new Accounts(accNo, custName, accType, transType, amount, balance);
                    acc.ShowData();
                }
                catch (InsufficientBalanceException ex)
                {
                    Console.WriteLine($"[Custom Exception] {ex.Message}");
                }
                catch (FormatException ex)
                {
                    Console.WriteLine($"[Input Error] Invalid input format: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Unhandled Exception] {ex.Message}");
                }

                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
        }

    }

}
