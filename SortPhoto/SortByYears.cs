using System.IO;
using ExtractMetadataAndParse;

namespace SortPhoto
{
    public static class SortByYears
    {
        private static string _path;
        private static string _subpath;
        private static string[] _files;
        private static string _dateOfShot;

        public static void Sort(string userPath)
        {
            _path = userPath;
            _files = Directory.GetFiles(_path);
            _subpath = new DirectoryInfo(_path).Name + "-Kolyago";
            var counter = 0;
            foreach (var file in _files)
            {
                ProgressBar.DrawTextProgressBar(counter++, _files.Length);
                _dateOfShot = ParseExif.Parse(file);
                if (_dateOfShot == null) continue;
                var year = _dateOfShot;
                year = year.Remove(0, 6).Remove(4, 9);

                var dirInfo = new DirectoryInfo(_path + "\\" + _subpath + "\\" + year);

                if (!dirInfo.Exists) dirInfo.Create();
                var extension = Path.GetExtension(file);
                var fileNameOnly = Path.GetFileNameWithoutExtension(file);

                File.Copy(file, _path + "\\" + _subpath + "\\" + year + "\\" + fileNameOnly + extension, true);
            }

            ProgressBar.DrawTextProgressBar(_files.Length, _files.Length);
        }
    }
}