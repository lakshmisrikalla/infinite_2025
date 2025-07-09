using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Code_Challenge_3
{

    // the file i have given here is under the csharp folder named test file and in that it appended the given text //

    class FileAppender_Q3
    {
        public static void Main()
        {
            string fileName = "C:\\Project\\Test file.txt";
            string textToAppend = "This is the new line of text to append.\n";

            bool fileExists = false;

            
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    fileExists = true;
                }
            }
            catch (FileNotFoundException)
            {
                fileExists = false;
            }

            FileMode mode = fileExists ? FileMode.Append : FileMode.Create;

            using (FileStream fs = new FileStream(fileName, mode, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.Write(textToAppend);
            }

            if (fileExists)
                Console.WriteLine("Text appended to existing file.");
            else
                Console.WriteLine("File not found. New file created and text written.");
            Console.ReadLine();
        }
    }
}

