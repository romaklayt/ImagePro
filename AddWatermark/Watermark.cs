using System.Drawing;
using System.IO;
using ExtractMetadataAndParse;

namespace AddWatermark
{
    public static class Watermark
    {
        private static string _path;
        private static string _subpath;
        private static string[] _files;
        private static string _dateOfShot;

        public static void Add(string userPath)
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
                var extension = Path.GetExtension(file);
                var fileNameOnly = Path.GetFileNameWithoutExtension(file);
                var imagePath = file;
                var resultPath = _path + "\\" + _subpath + "\\" + fileNameOnly + extension;

                var bitmap = (Bitmap) Image.FromFile(imagePath);
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

                bitmap.Save(resultPath);
            }

            ProgressBar.DrawTextProgressBar(_files.Length, _files.Length);
        }
    }
}