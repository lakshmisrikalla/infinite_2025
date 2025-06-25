using System;

class Accounts
{
    public int AccountNo;
    public string CustomerName;
    public string AccountType;
    public char TransactionType; // D/W
    public int Amount;
    public int Balance;

    public Accounts(int accNo, string name, string accType, char transType, int amount, int balance)
    {
        AccountNo = accNo;
        CustomerName = name;
        AccountType = accType;
        TransactionType = transType;
        Amount = amount;
        Balance = balance;
        UpdateBalance();
    }

    private void UpdateBalance()
    {
        if (TransactionType == 'D' || TransactionType == 'd')
            Credit(Amount);
        else if (TransactionType == 'W' || TransactionType == 'w')
            Debit(Amount);
        else
            Console.WriteLine("Invalid transaction type");
    }

    public void Credit(int amount)
    {
        Balance += amount;
    }

    public void Debit(int amount)
    {
        if (amount <= Balance)
            Balance -= amount;
        else
            Console.WriteLine("Insufficient balance");
    }

    public void ShowData()
    {
        Console.WriteLine($"Account No: {AccountNo}\nName: {CustomerName}\nAccount Type: {AccountType}");
        Console.WriteLine($"Transaction: {TransactionType}\nAmount: {Amount}\nUpdated Balance: Rs{Balance}");
    }
}

