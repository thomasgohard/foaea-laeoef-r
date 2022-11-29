using FileBroker.Model;
using FileBroker.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

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

                //var doc = XDocument.Parse(xmlData);
                //var json = JsonConvert.SerializeXNode(doc, Newtonsoft.Json.Formatting.Indented, omitRootObject: true);
                //return json;
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

        public static async Task<IActionResult> ProcessIncomingFileAsync(string fileName, IFileTableRepository fileTable, HttpRequest Request)
        {
            string bodyContent;
            string fileNameNoExtension;
            string fileNameNoCycle;
            FileTableData fileData;

            string outputPath;
            string outputFile;

            switch (Request.ContentType?.ToLower())
            {
                case "text/plain":
                    using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
                    {
                        bodyContent = await reader.ReadToEndAsync();
                    }

                    fileNameNoCycle = Path.GetFileNameWithoutExtension(fileName);
                    fileData = await fileTable.GetFileTableDataForFileNameAsync(fileNameNoCycle);

                    if ((fileData == null) || (string.IsNullOrEmpty(fileData.Path)))
                        return new BadRequestResult();

                    outputPath = fileData.Path;
                    outputFile = outputPath.AppendToPath(fileName, isFileName: true);

                    await System.IO.File.WriteAllTextAsync(outputFile, bodyContent);

                    return new OkResult();

                case "application/xml":
                case "text/xml":
                    using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
                    {
                        bodyContent = await reader.ReadToEndAsync();
                    }

                    fileNameNoExtension = Path.GetFileNameWithoutExtension(fileName);
                    fileNameNoCycle = Path.GetFileNameWithoutExtension(fileNameNoExtension);
                    fileData = await fileTable.GetFileTableDataForFileNameAsync(fileNameNoCycle);

                    if ((fileData == null) || (string.IsNullOrEmpty(fileData.Path)))
                        return new BadRequestResult();

                    outputPath = fileData.Path;
                    outputFile = outputPath.AppendToPath(fileName, isFileName: true);

                    await System.IO.File.WriteAllTextAsync(outputFile, bodyContent);

                    return new OkResult();

                case "application/json":
                    using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
                    {
                        bodyContent = await reader.ReadToEndAsync();
                    }

                    fileNameNoExtension = Path.GetFileNameWithoutExtension(fileName);
                    fileNameNoCycle = Path.GetFileNameWithoutExtension(fileNameNoExtension);
                    fileData = await fileTable.GetFileTableDataForFileNameAsync(fileNameNoCycle);

                    if ((fileData == null) || (string.IsNullOrEmpty(fileData.Path)))
                        return new BadRequestResult();

                    outputPath = fileData.Path;
                    outputFile = outputPath.AppendToPath(fileName, isFileName: true);
                    outputFile = Path.ChangeExtension(outputFile, ".XML");

                    var doc = JsonConvert.DeserializeXNode(bodyContent)!;

                    XDeclaration _defaultDeclaration = new("1.0", null, null);

                    var declaration = doc.Declaration ?? _defaultDeclaration;

                    await System.IO.File.WriteAllTextAsync(outputFile, $"{declaration}{Environment.NewLine}{doc}");

                    return new OkResult();

                default:
                    return new BadRequestResult();
            }
        }

    }
}
