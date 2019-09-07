using System;
using System.IO;

namespace AsunderNoMore
{
    class Program
    {
        static void Main(string[] args)
        {
            string folderName = Directory.GetCurrentDirectory();
            if (args.Length > 0)
            {
                folderName = args[0];
            }

            if (!Directory.Exists(folderName))
            {
                Console.WriteLine("Error: Invalid folder \"" + folderName + "\"");
                return;
            }

            Bigfile bigFile = new Bigfile(folderName);
            if (!bigFile.IsValid)
            {
                Console.WriteLine("Error: \"" + bigFile.Error + "\"");
            }

            Console.WriteLine("Packed \"" + Path.Combine(folderName, "bigfile.dat\""));
        }
    }
}
