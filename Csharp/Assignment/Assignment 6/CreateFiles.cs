using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Assignment_6
{

    class CreateFiles
    {
        static void WriteStream(string[] lines)
        {
            FileStream fs = new FileStream("C:\\Project\\Csharp\\Assignment\\Assignment 6\\Assignment text file.txt", FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            foreach (var str in lines)
            {
                sw.WriteLine(str);
            }
            sw.Flush();
            sw.Close();
        }


        static void Main(string[] args)
        {
            List<string> lines = new List<string>();
            Console.Write("Enter number of strings: ");
            int n = Convert.ToInt32(Console.ReadLine());
            for (int i = 0; i < n; i++)
            {
                Console.WriteLine("Enter string {0}", i + 1);
                string s = Console.ReadLine();
                lines.Add(s);
            }
            WriteStream(lines.ToArray());
            Console.WriteLine("Strings successfully added in the file");
            Console.Read();
        }
    }
}