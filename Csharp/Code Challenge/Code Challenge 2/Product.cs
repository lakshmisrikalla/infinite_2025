using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code_Challenge_2
{

    class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }

        public Product(int id, string name, double price)
        {
            ProductId = id;
            ProductName = name;
            Price = price;
        }

        public void Display()
        {
            Console.WriteLine($"ID: {ProductId}, Name: {ProductName}, Price: Rs{Price:F2}");
        }
    }

    class Products
    {
        static void Main()
        {
            List<Product> products = new List<Product>();

            Console.WriteLine(" Enter details for 10 products:");

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"\nProduct {i + 1}");

                Console.Write("Enter Product ID: ");
                int id = int.Parse(Console.ReadLine());

                Console.Write("Enter Product Name: ");
                string name = Console.ReadLine();

                Console.Write("Enter Product Price: Rs");
                double price = double.Parse(Console.ReadLine());

                products.Add(new Product(id, name, price));
            }

            
            for (int i = 0; i < products.Count - 1; i++)
            {
                for (int j = 0; j < products.Count - i - 1; j++)
                {
                    if (products[j].Price > products[j + 1].Price)
                    {
                       
                        var temp = products[j];
                        products[j] = products[j + 1];
                        products[j + 1] = temp;
                    }
                }
            }

            Console.WriteLine("\n Products sorted by price (Low to High):");
            for (int i = 0; i < products.Count; i++)
            {
                products[i].Display();
            }
            Console.ReadLine();
        }
    }

}
