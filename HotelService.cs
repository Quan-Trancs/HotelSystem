using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Schema;
using Newtonsoft.Json;

public class HotelService
{
    // 2.1 Verification Method
    public static string Verification(string xmlUrl, string xsdUrl)
    {
        try
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Schemas.Add(null, XmlReader.Create(xsdUrl));
            settings.ValidationType = ValidationType.Schema;

            string errorMsg = "No Error";

            settings.ValidationEventHandler += (sender, e) =>
            {
                errorMsg = $"Validation Error: {e.Message}";
            };

            using (XmlReader reader = XmlReader.Create(xmlUrl, settings))
            {
                while (reader.Read()) { }
            }

            return errorMsg;
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }

    // 2.2 Xml2Json Method
    public static string Xml2Json(string xmlUrl)
    {
        try
        {
            WebClient client = new WebClient();
            string xmlContent = client.DownloadString(xmlUrl);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlContent);

            // Remove Rating element if empty
            XmlNodeList hotels = doc.SelectNodes("//Hotel");
            foreach (XmlNode hotel in hotels)
            {
                XmlNode ratingNode = hotel.SelectSingleNode("Rating");
                if (ratingNode != null && string.IsNullOrWhiteSpace(ratingNode.InnerText))
                {
                    hotel.RemoveChild(ratingNode);
                }
            }

            string jsonText = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented, true);
            return jsonText;
        }
        catch (Exception ex)
        {
            return $"Exception: {ex.Message}";
        }
    }
}
