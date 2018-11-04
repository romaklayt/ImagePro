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
            var dateShot = fileMetadata[2].Tags[2].Description;
            if (!Regex.IsMatch(dateShot, Regex.ToString(), RegexOptions.IgnoreCase))
                _dateOfShot = File.GetCreationTime(file).ToString();


            return _dateOfShot.Replace(":", "-");
        }
    }
}