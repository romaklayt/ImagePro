using System.IO;
using ExtractMetadataAndParse;

namespace RenamePhoto
{
    public static class RenameByDate
    {
        private static string _path;
        private static string _subpath;
        private static string[] _files;
        private static string _dateOfShot;


        public static void Rename(string userPath)
        {
            _path = userPath;
            _files = Directory.GetFiles(_path);
            _subpath = new DirectoryInfo(_path).Name + "-Kolyago";
            var counter = 1;
            var i = 0;
            foreach (var file in _files)
            {
                ProgressBar.DrawTextProgressBar(i++, _files.Length);
                _dateOfShot = ParseExif.Parse(file);
                if (_dateOfShot == null) continue;
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

            ProgressBar.DrawTextProgressBar(_files.Length, _files.Length);
        }
    }
}