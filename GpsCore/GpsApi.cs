using System.Net;
using System.Text;
using System.Xml;

namespace GpsCore
{
    public static class GpsApi
    {
        public static string FindCity(string gpsW, string gpsH)
        {
            if (!Check(gpsW, gpsH)) return null;
            const string urlApi = "https://geocode-maps.yandex.ru/1.x/?geocode=";
            var xd = new XmlDocument();
            string txtXmlAddress = null;
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
                    txtXmlAddress = childCountry.FirstChild.InnerText;
                    txtXmlAddress = txtXmlAddress.Remove(txtXmlAddress.IndexOf(','),
                        txtXmlAddress.Length - txtXmlAddress.IndexOf(','));

                    break;
                }

            return txtXmlAddress;
        }

        private static bool Check(string gpsW, string gpsH)
        {
            var res1 = double.TryParse(gpsW, out var aResult);
            var res2 = double.TryParse(gpsH, out var bResult);
            return res1 & res2;
        }
    }
}