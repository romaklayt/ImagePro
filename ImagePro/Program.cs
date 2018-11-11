using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using MetadataExtractor;
using Directory = MetadataExtractor.Directory;

namespace ImagePro
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ChangeFolder();
            do
            {
                Console.WriteLine("Menu:");
                Console.WriteLine("1 - RenameFilesontheDate");
                Console.WriteLine("2 - Create Watermark");
                Console.WriteLine("3 - Sort by years");
                Console.WriteLine("4 - Sort by city");
                Console.WriteLine("5 - change workFolder");
                Console.WriteLine("6 - exit");
                switch (Console.ReadLine())
                {
                    case "1":
                        RenametotheDate();
                        Console.WriteLine("Complete");
                        break;
                    case "2":
                        AddMark();
                        Console.WriteLine("Complete");
                        break;
                    case "3":
                        SortbyYears();
                        Console.WriteLine("Complete");
                        break;
                    case "4":
                        SortbyCity();
                        Console.WriteLine("Complete");
                        break;
                    case "5":
                        ChangeFolder();
                        Console.WriteLine("Folder change!");
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("uncorrect data");
                        break;
                }
            } while (true);
        }

        private static string _path;
        private static string _subpath;
        private static string[] _files;
        private static string _dateOfShot;
        private static readonly Regex regex = new Regex(@"\d{4}:\d{2}:\d{2} \d{2}:\d{2}:\d{2}");
        private static string _gpsW;
        private static string _gpsH;
        private static IReadOnlyList<Directory> _fileMetadata;


        private static void ChangeFolder()
        {
            Console.WriteLine("Enter _path to the photo:");
            do
            {
                _path = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(_path))
                {
                    Console.WriteLine("uncorrect folder");
                    continue;
                }

                var dirInfo = new DirectoryInfo(_path);
                if (!dirInfo.Exists)
                    Console.WriteLine("Folder not found");
                else
                    return;
            } while (true);
        }

        private static void WorkFolder()
        {
            _files = System.IO.Directory.GetFiles(_path);
            var dirInfo = new DirectoryInfo(_path);
            _subpath = dirInfo.Name + "-Kolyago";
            if (dirInfo.Root.ToString() == _path)
            {
                _subpath = "Kolyago";
            }
            dirInfo.CreateSubdirectory(_subpath);
        }

        private static void GpsCoordinate(string file, out bool GpsCoordinateNot)
        {
            try
            {
                _gpsW = _fileMetadata[4].Tags[1].Description;
                _gpsH = _fileMetadata[4].Tags[3].Description;
                _gpsH = GpsParse(_gpsH).Replace(",", ".");
                _gpsW = GpsParse(_gpsW).Replace(",", ".");
            }
            catch
            {
                Console.WriteLine($"File: {file} gps coordinate not found");
                GpsCoordinateNot = true;
                return;
            }

            GpsCoordinateNot = false;
        }

        private static string GpsParse(string gps)
        {
            var gpsmin = double.Parse(gps[4].ToString() + gps[5]) / 60;
            var gpssec = double.Parse(gps[8].ToString() + gps[9]) / 3600;
            var gpsGrad = double.Parse(gps[0].ToString() + gps[1]);
            var gpsRes = (gpsGrad + gpsmin + gpssec).ToString();
            return gpsRes;
        }

        private static void ParseFilesAndExif(string file)
        {
            try
            {
                _fileMetadata = ImageMetadataReader.ReadMetadata(file);
            }
            catch
            {
                Console.WriteLine($"File: {file} not support");
                return;
            }

            var dateShot = _fileMetadata[2].Tags[2].Description;
            if (!Regex.IsMatch(dateShot, regex.ToString(), RegexOptions.IgnoreCase))
                _dateOfShot = File.GetCreationTime(file).ToString();


            _dateOfShot = _dateOfShot.Replace(":", "-");
        }

        private static void RenametotheDate()
        {
            var counter = 1;
            WorkFolder();
            foreach (var file in _files)
            {
                ParseFilesAndExif(file);
                var extension = Path.GetExtension(file);
                try
                {
                    File.Copy(file, _path + "\\" + _subpath + "\\" + _dateOfShot + extension);
                }
                catch
                {
                    var path = Path.GetDirectoryName(file);
                    var newFullPath = file;

                    while (File.Exists(newFullPath))
                    {
                        var tempFileName = $"{_dateOfShot}({counter++})";
                        newFullPath = Path.Combine(path + "\\" + _subpath, tempFileName + extension);
                    }

                    File.Copy(file, newFullPath);
                }
            }
        }

        private static void AddMark()
        {
            WorkFolder();
            foreach (var file in _files)
            {
                ParseFilesAndExif(file);
                var extension = Path.GetExtension(file);
                var fileNameOnly = Path.GetFileNameWithoutExtension(file);
                var imagePath = file;
                var resultPath = _path + "\\" + _subpath + "\\" + fileNameOnly + extension;

                var bitmap = (Bitmap) Image.FromFile(imagePath); //load the image file
                var imageWidth = bitmap.Width;
                var textSize = 10;
                while (imageWidth / 2 > textSize * 20) textSize += 5;

                var firstLocation = new PointF(imageWidth - _dateOfShot.Length * textSize / 2, 0);
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    using (var arialFont = new Font("Arial", textSize))
                    {
                        graphics.DrawString(_dateOfShot, arialFont, Brushes.White, firstLocation);
                    }
                }

                bitmap.Save(resultPath); //save the image file
            }
        }

        private static void SortbyYears()
        {
            WorkFolder();

            foreach (var file in _files)
            {
                ParseFilesAndExif(file);
                var year = _dateOfShot;
                year = year.Remove(0, 6).Remove(4, 9);

                var dirInfo = new DirectoryInfo(_path + "\\" + _subpath + "\\" + year);

                if (!dirInfo.Exists) dirInfo.Create();
                var extension = Path.GetExtension(file);
                var fileNameOnly = Path.GetFileNameWithoutExtension(file);

                File.Copy(file, _path + "\\" + _subpath + "\\" + year + "\\" + fileNameOnly + extension, true);
            }
        }

        private static void SortbyCity()
        {
            WorkFolder();
            var urlAPI = "https://geocode-maps.yandex.ru/1.x/?geocode=";
            foreach (var file in _files)
            {
                ParseFilesAndExif(file);
                GpsCoordinate(file, out var gpsCoordinateNot);
                if (gpsCoordinateNot) continue;

                var xd = new XmlDocument();
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers.Add("Accept-Language", " en-US");
                    client.Headers.Add("Accept", "application/xml");
                    client.Headers.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)");
                    xd.LoadXml(client.DownloadString(urlAPI + _gpsH + "," + _gpsW));
                }

                var geoObjectTempCountry = xd.GetElementsByTagName("Country");

                foreach (XmlNode childCountry in geoObjectTempCountry[0].ChildNodes)
                    if (childCountry.Name == "AddressLine")
                    {
                        var txtXmlAddress = childCountry.FirstChild.InnerText;
                        txtXmlAddress = txtXmlAddress.Remove(txtXmlAddress.IndexOf(','),
                            txtXmlAddress.Length - txtXmlAddress.IndexOf(','));

                        var destFileName = new DirectoryInfo(_path + "\\" + _subpath + "\\" + txtXmlAddress);
                        if (!destFileName.Exists) destFileName.Create();
                        var extension = Path.GetExtension(file);
                        var fileNameOnly = Path.GetFileNameWithoutExtension(file);
                        File.Copy(file, destFileName + "\\" + fileNameOnly + extension, true);
                        break;
                    }
            }
        }
    }
}