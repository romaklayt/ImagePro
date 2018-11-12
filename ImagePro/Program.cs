using System;
using System.IO;
using System.Threading;
using AddWatermark;
using RenamePhoto;
using SortPhoto;

namespace ImagePro
{
    internal static class Program
    {
        private static string _path;
        private static string _subpath;

        private static void Main(string[] args)
        {
            ChangeFolder();

            do
            {
                Console.Clear();
                Console.WriteLine($"Folder: {_path}");
                Console.WriteLine("Menu:");
                Console.WriteLine("1 - Rename Files on the Date");
                Console.WriteLine("2 - Create Watermark");
                Console.WriteLine("3 - Sort by years");
                Console.WriteLine("4 - Sort by city");
                Console.WriteLine("5 - Change folder");
                Console.WriteLine("6 - Exit");
                Console.Write("Your choice: ");
                switch (Console.ReadLine())
                {
                    case "1":
                        RenameByDate.Rename(_path);
                        Success("\nComplete");
                        break;
                    case "2":
                        Watermark.Add(_path);
                        Success("\nComplete");
                        break;
                    case "3":
                        SortByYears.Sort(_path);
                        Success("\nComplete");
                        break;
                    case "4":
                        SortByCity.Sort(_path);
                        Success("\nComplete");
                        break;
                    case "5":
                        ChangeFolder();
                        Success("Folder change!");
                        break;
                    case "6":
                        return;
                    default:
                        Error("Uncorrect data");
                        break;
                }

                Thread.Sleep(3000);
            } while (true);
        }

        private static void Error(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        private static void Success(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        private static void ChangeFolder()
        {
            Console.WriteLine("Enter _path to the photo:");
            do
            {
                _path = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(_path))
                {
                    Error("Uncorrect folder");
                    continue;
                }

                var dirInfo = new DirectoryInfo(_path);
                if (!dirInfo.Exists)
                {
                    Error("Folder not found");
                }
                else if (dirInfo.Root.ToString() == _path)
                {
                    _subpath = "Kolyago";
                    dirInfo.CreateSubdirectory(_subpath);
                    return;
                }
                else
                {
                    _subpath = new DirectoryInfo(_path).Name + "-Kolyago";
                    dirInfo.CreateSubdirectory(_subpath);
                    return;
                }
            } while (true);
        }
    }
}