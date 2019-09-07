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

            Bigfile bigfile = new Bigfile(folderName);
            if (!bigfile.IsValid)
            {
                Console.WriteLine("Error: \"" + bigfile.Error + "\"");
            }

            string bigfileName = Path.Combine(folderName, "bigfile.dat");
            bigfile.CreateBigfile(bigfileName);
            Console.WriteLine("Packed \"" + bigfileName + "\"");
        }
    }
}
