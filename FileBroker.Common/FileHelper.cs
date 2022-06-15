using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace FileBroker.Common
{
    public static class FileHelper
    {
        public static string RemoveCycleFromFilename(string fileName)
        {
            int lastPeriod = fileName.LastIndexOf('.');
            if (lastPeriod > 0)
                return fileName.Substring(0, lastPeriod);
            else
                return fileName;
        }

        public static int GetCycleFromFilename(string fileName)
        {
            if (fileName.ToUpper().EndsWith(".XML"))
                fileName = Path.GetFileNameWithoutExtension(fileName);

            int lastPeriod = fileName.LastIndexOf('.');
            if (lastPeriod > 0)
                return int.TryParse(fileName[(lastPeriod + 1)..], out int cycle) ? cycle : -1;
            else
                return -1;
        }

        public static string ConvertXmlToJson(XmlDocument doc, ref List<string> errors)
        {
            try
            {
                string result = JsonConvert.SerializeXmlNode(doc);

                // clean up json to remove xml-specific artifacts
                result = result.Replace("\"?xml\":{\"@version\":\"1.0\",\"@encoding\":\"UTF-8\",\"@standalone\":\"yes\"},", "");
                result = result.Replace("\"?xml\":{\"@version\":\"1.0\",\"@encoding\":\"UTF-8\"},", "");
                result = result.Replace("\"?xml\":{\"@version\":\"1.0\",\"@encoding\":\"utf-8\"},", "");
                result = result.Replace("\"@xsi:type\":\"NewDataSet\",\"@xmlns:xsi\":\"http://www.w3.org/2001/XMLSchema-instance\",", "");
                result = result.Replace("\"@xmlns:xsd\":\"http://www.w3.org/2001/XMLSchema\",\"@xmlns:xsi\":\"http://www.w3.org/2001/XMLSchema-instance\",", "");

                return result;
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                return "";
            }
        }
    }
}
