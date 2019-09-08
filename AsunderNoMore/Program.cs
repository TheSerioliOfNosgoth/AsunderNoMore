using System;
using System.IO;

namespace AsunderNoMore
{
    class Program
    {
        static void Main(string[] args)
        {
            string outputFolderName = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();
            if (!Directory.Exists(outputFolderName))
            {
                Console.WriteLine("Error: Invalid output folder \"" + outputFolderName + "\".");
                return;
            }

            string inputFolderName = args.Length > 1 ? args[1] : Path.Combine(outputFolderName, "kain2");
            if (!Directory.Exists(inputFolderName))
            {
                Console.WriteLine("Error: Invalid input folder \"" + inputFolderName + "\".");
                return;
            }

            string compareFolderName = args.Length > 2 ? args[2] : Path.Combine(outputFolderName, "compare");
            if (!Directory.Exists(compareFolderName))
            {
                Console.WriteLine("Error: Invalid compare folder \"" + compareFolderName + "\".");
                return;
            }

            string compareBigfileName = Path.Combine(compareFolderName, "bigfile.dat");
            if (!File.Exists(compareBigfileName))
            {
                Console.WriteLine("Error: Invalid compare bigfile \"" + compareBigfileName + "\".");
                return;
            }

            Bigfile bigfile = new Bigfile();

            if (!bigfile.Import(inputFolderName, compareBigfileName))
            {
                Console.WriteLine("Error importing files: \"" + bigfile.Error + "\".");
                return;
            }

            string bigfileName = Path.Combine(outputFolderName, "bigfile.dat");
            if (!bigfile.Save(bigfileName))
            {
                Console.WriteLine("Error saving bigfile: \"" + bigfile.Error + "\".");
                return;
            }

            Console.WriteLine("Packed \"" + bigfileName + "\".");
        }
    }
}
