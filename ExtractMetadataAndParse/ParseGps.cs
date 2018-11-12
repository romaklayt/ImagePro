using System;
using System.IO;

namespace ExtractMetadataAndParse
{
    public static class ParseGps
    {
        private static string _gpsW;
        private static string _gpsH;

        public static (string, string, bool) Parse(string file)
        {
            var fileMetadata = ExtractMetadata.Extract(file);
            if (fileMetadata == null) return (null, null, true);
            try
            {
                _gpsW = fileMetadata[5].Tags[1].Description;
                _gpsH = fileMetadata[5].Tags[3].Description;
                _gpsH = GpsParseToString(_gpsH).Replace(",", ".");
                _gpsW = GpsParseToString(_gpsW).Replace(",", ".");
            }
            catch
            {
                Console.CursorLeft = 0;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"File: {file} gps coordinate not found");
                Console.ResetColor();
                return (null, null, true);
            }

            return (_gpsW, _gpsH, false);
        }

        private static string GpsParseToString(string gps)
        {
            var gpsmin = double.Parse(gps[4].ToString() + gps[5]) / 60;
            var gpssec = double.Parse(gps[8].ToString() + gps[9]) / 3600;
            var gpsGrad = double.Parse(gps[0].ToString() + gps[1]);
            var gpsRes = (gpsGrad + gpsmin + gpssec).ToString();
            return gpsRes;
        }

        public static (string, string, bool) Parse(FileStream file)
        {
            var fileMetadata = ExtractMetadata.Extract(file);
            if (fileMetadata == null) return (null, null, true);
            try
            {
                _gpsW = fileMetadata[5].Tags[1].Description;
                _gpsH = fileMetadata[5].Tags[3].Description;
                _gpsH = GpsParseToString(_gpsH).Replace(",", ".");
                _gpsW = GpsParseToString(_gpsW).Replace(",", ".");
            }
            catch
            {
                Console.CursorLeft = 0;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"File: {file} gps coordinate not found");
                Console.ResetColor();
                return (null, null, true);
            }

            return (_gpsW, _gpsH, false);
        }
    }
}