using System;

public class Accounts
{
    int AccNo;
    string CustName;
    string AccType;
    char TransactionType;
    int Amount;
    int Balance;

    public void ProcessAccount()
    {
        Console.Write("Enter Account Number: ");
        AccNo = Convert.ToInt32(Console.ReadLine());

        Console.Write("Enter Customer Name: ");
        CustName = Console.ReadLine();

        Console.Write("Enter Account Type: ");
        AccType = Console.ReadLine();

        Console.Write("Enter Transaction Type (D/W): ");
        TransactionType = Convert.ToChar(Console.ReadLine());

        Console.Write("Enter Transaction Amount: ");
        Amount = Convert.ToInt32(Console.ReadLine());

        Console.Write("Enter Current Balance: ");
        Balance = Convert.ToInt32(Console.ReadLine());

        UpdateBalance();
        ShowData();
    }

    void Credit(int amount) => Balance += amount;

    void Debit(int amount)
    {
        if (amount <= Balance)
            Balance -= amount;
        else
            Console.WriteLine("Insufficient balance.");
    }

    void UpdateBalance()
    {
        if (TransactionType == 'D' || TransactionType == 'd')
            Credit(Amount);
        else if (TransactionType == 'W' || TransactionType == 'w')
            Debit(Amount);
    }

    public void ShowData()
    {
        Console.WriteLine("\n--- Account Details ---");
        Console.WriteLine($"Account No: {AccNo}\nCustomer Name: {CustName}\nAccount Type: {AccType}\nTransaction Type: {TransactionType}\nAmount: {Amount}\nBalance: {Balance}");
    }
}
