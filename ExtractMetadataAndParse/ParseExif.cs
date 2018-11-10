using System.IO;
using System.Text.RegularExpressions;

namespace ExtractMetadataAndParse
{
    public static class ParseExif
    {
        private static string _dateOfShot;
        private static readonly Regex Regex = new Regex(@"\d{4}:\d{2}:\d{2} \d{2}:\d{2}:\d{2}");

        public static string Parse(string file)
        {
            var fileMetadata = ExtractMetadata.Extract(file);
            if (fileMetadata == null) return null;
            _dateOfShot = fileMetadata[2].Tags[2].Description;
            if (!Regex.IsMatch(_dateOfShot, Regex.ToString(), RegexOptions.IgnoreCase))
                _dateOfShot = File.GetCreationTime(file).ToString();

            return _dateOfShot.Replace(":", "-");
        }

        public static string Parse(FileStream file)
        {
            var fileMetadata = ExtractMetadata.Extract(file);
            if (fileMetadata == null) return null;
            _dateOfShot = fileMetadata[2].Tags[2].Description;
            if (!Regex.IsMatch(_dateOfShot, Regex.ToString(), RegexOptions.IgnoreCase))
                _dateOfShot = "Not_inform";

            return _dateOfShot.Replace(":", "-");
        }
    }
}