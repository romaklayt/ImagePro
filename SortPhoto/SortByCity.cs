using System.IO;
using ExtractMetadataAndParse;
using GpsCore;

namespace SortPhoto
{
    public static class SortByCity
    {
        private static string _path;
        private static string _subpath;
        private static string[] _files;


        public static void Sort(string userPath)
        {
            _path = userPath;
            _files = Directory.GetFiles(_path);
            _subpath = new DirectoryInfo(_path).Name + "-Kolyago";
            var counter = 0;
            foreach (var file in _files)
            {
                ProgressBar.DrawTextProgressBar(counter++, _files.Length);
                var (gpsW, gpsH, gpsCoordinateNotFound) = ParseGps.Parse(file);

                if (gpsCoordinateNotFound) continue;
                var city = GpsApi.FindCity(gpsW, gpsH);
                if (city == null) continue;
                var destFileName = new DirectoryInfo(_path + "\\" + _subpath + "\\" + city);
                if (!destFileName.Exists) destFileName.Create();
                var extension = Path.GetExtension(file);
                var fileNameOnly = Path.GetFileNameWithoutExtension(file);
                File.Copy(file, destFileName + "\\" + fileNameOnly + extension, true);
            }

            ProgressBar.DrawTextProgressBar(_files.Length, _files.Length);
        }
    }
}