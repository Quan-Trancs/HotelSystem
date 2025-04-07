using System;
using System.Net.Http;
using System.Xml;
using System.Xml.Schema;
using Newtonsoft.Json;
using System.Threading.Tasks;


namespace ConsoleApp1
{
    public class Program
    {
        // URLs to the XML files and schema
        public static string xmlURL = "https://raw.githubusercontent.com/Quan-Trancs/HotelSystem/refs/heads/main/Hotels.xml";
        public static string xmlErrorURL = "https://raw.githubusercontent.com/Quan-Trancs/HotelSystem/refs/heads/main/HotelsErrors.xml";
        public static string xsdURL = "https://raw.githubusercontent.com/Quan-Trancs/HotelSystem/refs/heads/main/Hotels.xsd";

        static bool hasErrors = false;
        static string errorMsg = "";

        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting XML Validation and Conversion...\n");

                // Validate correct XML
                string result = Verification(xmlURL, xsdURL);
                Console.WriteLine(result);

                // Validate XML with Errors
                result = Verification(xmlErrorURL, xsdURL);
                Console.WriteLine(result);

                // Convert valid XML to JSON
                result = Xml2Json(xmlURL); // URL of the valid XML
                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        // Q2.1 Verification - Updated to fetch files from URL
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

                string xmlContent = ReadFileFromUrl(xmlUrl).GetAwaiter().GetResult(); // Synchronously wait for the result
                if (string.IsNullOrEmpty(xmlContent))
                {
                    return "Failed to retrieve XML data.";
                }

                using (XmlReader reader = XmlReader.Create(new System.IO.StringReader(xmlContent), settings))
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

        // Q2.2 XML to JSON - Updated to fetch XML from URL
        public static string Xml2Json(string xmlUrl)
        {
            try
            {
                string xmlContent = ReadFileFromUrl(xmlUrl).GetAwaiter().GetResult(); // Synchronously wait for the result
                if (string.IsNullOrEmpty(xmlContent))
                {
                    return "Failed to retrieve XML data.";
                }

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlContent); // Load XML content from string

                // Convert XML to JSON (using Newtonsoft.Json to serialize)
                string jsonText = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented, true);

                // Optionally: Deserialize the JSON back to XML to ensure it's deserializable (can be used for validation or debugging)
                var deserializedXml = JsonConvert.DeserializeXmlNode(jsonText);

                // Return the JSON string
                return jsonText;
            }
            catch (Exception ex)
            {
                return "Exception: " + ex.Message;
            }
        }

        // Method to read file content from URL
        public static async Task<string> ReadFileFromUrl(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        return content;
                    }
                    else
                    {
                        return $"Error: {response.StatusCode}";
                    }
                }
                catch (Exception ex)
                {
                    return $"Exception: {ex.Message}";
                }
            }
        }
    }
}