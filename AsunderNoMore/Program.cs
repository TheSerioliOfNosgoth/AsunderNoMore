using System;
using System.IO;

namespace AsunderNoMore
{
    class Program
    {
        static void Main(string[] args)
        {
            bool validArgs = (args.Length >= 2);
            if (validArgs)
            {
                switch (args[0])
                {
                    case "-unpack":
                    case "-u":
                    case "-pack":
                    case "-p":
                        break;
                    default:
                        validArgs = false;
                        break;
                }
            }

            if (!validArgs)
            {
                Console.WriteLine("Error: Expected \"[-repository, -r, -archives, -a] [FOLDER NAME]\"");
                return;
            }

            if (!Directory.Exists(args[1]))
            {
                Console.WriteLine("Error: Invalid project folder \"" + args[1] + "\".");
                return;
            }

            Repository repository = new Repository(args[1]);

            if (args[0] == "-unpack" || args[0] == "-u")
            {
                repository.UnpackRepository();
            }
            else if (args[0] == "-pack" || args[0] == "-p")
            {
                if (args.Length > 2 && (args[2] == "-f" || args[2] == "forceAllFiles"))
                {
                    repository.PackRepository(true);
                }
                else
                {
                    repository.PackRepository();
                }
            }

            Console.ReadLine();
        }
    }
}
