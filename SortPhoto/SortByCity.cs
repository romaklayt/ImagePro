using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using ExtractMetadataAndParse;

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
            var urlApi = "https://geocode-maps.yandex.ru/1.x/?geocode=";
            var counter = 0;
            foreach (var file in _files)
            {
                ProgressBar.DrawTextProgressBar(counter++, _files.Length);
                var (gpsW, gpsH, gpsCoordinateNotFound) = ParseGps.Parse(file);

                if (gpsCoordinateNotFound) continue;

                var xd = new XmlDocument();
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers.Add("Accept-Language", " en-US");
                    client.Headers.Add("Accept", "application/xml");
                    client.Headers.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)");
                    xd.LoadXml(client.DownloadString(urlApi + gpsH + "," + gpsW));
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

            ProgressBar.DrawTextProgressBar(_files.Length, _files.Length);
        }
    }
}