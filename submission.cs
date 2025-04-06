using System;
using System.Xml;
using System.Xml.Schema;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    public class Program
    {
        // Update these URLs with your ASU public files later
        public static string xmlURL = "https://www.public.asu.edu/~YourASURITEID/Hotels.xml";         
        public static string xmlErrorURL = "https://www.public.asu.edu/~YourASURITEID/HotelsErrors.xml"; 
        public static string xsdURL = "https://www.public.asu.edu/~YourASURITEID/Hotels.xsd";  

        static bool hasErrors = false;
        static string errorMsg = "";

        public static void Main(string[] args)
        {
            // Validate correct XML
            string result = Verification(xmlURL, xsdURL);
            Console.WriteLine(result);

            // Validate XML with Errors
            result = Verification(xmlErrorURL, xsdURL);
            Console.WriteLine(result);

            // Convert valid XML to JSON
            result = Xml2Json("Hotels.xml"); // Local file name for test
            Console.WriteLine(result);
        }

        // Q2.1 Verification
        public static string Verification(string xmlUrl, string xsdUrl)
        {
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas.Add(null, xsdUrl);
                settings.ValidationType = ValidationType.Schema;
                settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);

                hasErrors = false;
                errorMsg = "";

                using (XmlReader reader = XmlReader.Create(xmlUrl, settings))
                {
                    while (reader.Read()) { }
                }

                if (hasErrors)
                {
                    return errorMsg;
                }
                else
                {
                    return "No Error";
                }
            }
            catch (Exception ex)
            {
                return "Exception: " + ex.Message;
            }
        }

        private static void ValidationCallBack(object sender, ValidationEventArgs e)
        {
            hasErrors = true;
            errorMsg += e.Message + Environment.NewLine;
        }

        // Q2.2 XML to JSON
        public static string Xml2Json(string xmlUrl)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlUrl);

                string jsonText = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented, true);
                return jsonText;
            }
            catch (Exception ex)
            {
                return "Exception: " + ex.Message;
            }
        }
    }
}
