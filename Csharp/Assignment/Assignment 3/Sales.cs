using System;

public class SaleDetails
{
    int SalesNo;
    int ProductNo;
    double Price;
    string DateOfSale;
    int Qty;
    double TotalAmount;

    public void ProcessSale()
    {
        Console.WriteLine("");

        Console.Write("Enter Sales Number: ");
        SalesNo = Convert.ToInt32(Console.ReadLine());

        Console.Write("Enter Product Number: ");
        ProductNo = Convert.ToInt32(Console.ReadLine());

        Console.Write("Enter Price: ");
        Price = Convert.ToDouble(Console.ReadLine());

        Console.Write("Enter Quantity: ");
        Qty = Convert.ToInt32(Console.ReadLine());

        Console.Write("Enter Date of Sale (dd/mm/yyyy): ");
        DateOfSale = Console.ReadLine();

        Sales();
        ShowData();
    }

    void Sales()
    {
        TotalAmount = Qty * Price;
    }

    void ShowData()
    {
        Console.WriteLine("\n--- Sales Details ---");
        Console.WriteLine($"Sales No: {SalesNo}, Product No: {ProductNo}, Price: {Price}, Qty: {Qty}, Date: {DateOfSale}, Total Amount: {TotalAmount}");
        Console.ReadLine();
    }
}
