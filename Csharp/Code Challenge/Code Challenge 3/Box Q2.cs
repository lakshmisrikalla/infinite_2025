using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code_Challenge_3
{
   public class Box
   
    {
        public int Length;
        public int Breadth;

        
        public Box(int length, int breadth)
        {
            Length = length;
            Breadth = breadth;
        }

       
        public static Box Add(Box b1, Box b2)
        {
            
            int newLength = b1.Length + b2.Length;
            int newBreadth = b1.Breadth + b2.Breadth;

            return new Box(newLength, newBreadth);
        }

        
        public void Display()
        {
            Console.WriteLine("Length: " + Length);
            Console.WriteLine("Breadth: " + Breadth);
            Console.ReadLine();
        }
    }

    public class Box_Q2
    {
        public static void Main()
        {
           
            dynamic box1 = new Box(10, 20);
            dynamic box2 = new Box(30, 40);

            
            dynamic box3 = Box.Add(box1, box2);

            Console.WriteLine("Details of the third box after addition:");
            box3.Display();
           
        }
    }

}