using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment_5
{
        public class Books
        {
            public string BookName { get; set; }


            public string AuthorName { get; set; }
            public Books(string BookName, string AuthorName)


            {
                this.BookName = BookName;

                this.AuthorName = AuthorName;
            }

            public void Display()

            {
                Console.WriteLine($"Book Name : {BookName}  and  Author Name : {AuthorName}");
            }
        }


        public class BookShelf


        {
            private Books[] arr = new Books[5];
            public Books this[int index]
            {
                get
                {
                    if (index >= 0 && index < arr.Length)
                    { 
                        return arr[index];
                    }
                    else
                        throw new IndexOutOfRangeException("Index is Out of Range");
                }

                set

                {

                    if (index >= 0 && index < arr.Length)
                    {

                        arr[index] = value;
                    }

                    else
                        throw new IndexOutOfRangeException("Index is Out of Range");
                }


            }

        }
        public class Access

        {
            public static void Main()


            {
                try
                {
                    BookShelf bs = new BookShelf();
                    for (int i = 0; i < 5; i++)
                    {


                        Console.WriteLine("Enter Book-{0} Details: ", i + 1);


                        Console.Write("Enter Book Name: ");


                        string BookName = Console.ReadLine();


                        Console.Write("Enter Author Name: ");


                        string AuthorName = Console.ReadLine();


                        bs[i] = new Books(BookName, AuthorName);

                    }
                for (int i = 0; i < 5; i++)
                 {

                    bs[i].Display();
                 }
                }
                catch (IndexOutOfRangeException ex)


                {
                    Console.WriteLine("Exception Caught:" + ex.Message);
                }


                Console.Read();


            }


        }


    }

 