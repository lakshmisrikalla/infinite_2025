using System;

class Saledetails
{
    public int SalesNo;
    public int ProductNo;
    public double Price;
    public DateTime DateOfSale;
    public int Qty;
    public double TotalAmount;

    public Saledetails(int salesNo, int productNo, double price, int qty, DateTime dateOfSale)
    {
        SalesNo = salesNo;
        ProductNo = productNo;
        Price = price;
        Qty = qty;
        DateOfSale = dateOfSale;
        Sales();
    }

    public void Sales()
    {
        TotalAmount = Price * Qty;
    }

    public static void ShowData(Saledetails sale)
    {
        Console.WriteLine($"Sales No: {sale.SalesNo}\nProduct No: {sale.ProductNo}\nPrice: {sale.Price}");
        Console.WriteLine($"Quantity: {sale.Qty}\nDate: {sale.DateOfSale.ToShortDateString()}\nTotal Amount: {sale.TotalAmount}");
    }
}
