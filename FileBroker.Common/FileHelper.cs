using FileBroker.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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
            if (string.IsNullOrEmpty(fileName))
                return -1;

            if (fileName.ToUpper().EndsWith(".XML"))
                fileName = Path.GetFileNameWithoutExtension(fileName);

            int lastPeriod = fileName.LastIndexOf('.');
            if (lastPeriod > 0)
                return int.TryParse(fileName[(lastPeriod + 1)..], out int cycle) ? cycle : -1;
            else
                return -1;
        }

        public static bool IsExpectedCycle(FileTableData fileTableData, string fileName, out int expectedCycle, out int actualCycle)
        {
            expectedCycle = fileTableData.Cycle;
            actualCycle = GetCycleFromFilename(fileName);

            return (actualCycle == expectedCycle);
        }

        public static string ConvertXmlToJson(string xmlData, List<string> errors)
        {
            try
            {
                xmlData = FileHelper.RemoveXMLartifacts(xmlData);
                var doc = new XmlDocument();
                doc.LoadXml(xmlData);

                return JsonConvert.SerializeXmlNode(doc);
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                return "";
            }
        }

        private static string RemoveXMLartifacts(string xmlData)
        {
            string result = xmlData;

            string replacement = string.Empty;
            string pattern = @"<\?xml.*\?>";
            result = Regex.Replace(result, pattern, replacement, RegexOptions.IgnoreCase);

            replacement = "";
            pattern = @"[\t\s]+xmlns[^>]+";
            result = Regex.Replace(result, pattern, replacement, RegexOptions.IgnoreCase);

            replacement = "";
            pattern = @"[\t\s]+xsi[^>]+";
            result = Regex.Replace(result, pattern, replacement, RegexOptions.IgnoreCase);

            return result;
        }

    }
}
