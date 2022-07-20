using DBHelper;
using FileBroker.Common;
using FileBroker.Data.DB;
using FileBroker.Model.Interfaces;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Incoming.Common
{
    public class IncomingProvincialFile
    {
        private IFileTableRepository FileTableDB { get; }
        private string InterceptionBaseName { get; }
        private string TracingBaseName { get; }
        private string LicencingBaseName { get; }
        private ApiConfig ApiFilesConfig { get; }
        private IAPIBrokerHelper APIHelper { get; }

        public IncomingProvincialFile(IFileTableRepository fileTable,
                                      ApiConfig apiFilesConfig,
                                      IAPIBrokerHelper apiHelper,
                                      string interceptionBaseName,
                                      string tracingBaseName,
                                      string licencingBaseName)
        {
            ApiFilesConfig = apiFilesConfig;
            APIHelper = apiHelper;
            FileTableDB = fileTable;
            InterceptionBaseName = interceptionBaseName;
            TracingBaseName = tracingBaseName;
            LicencingBaseName = licencingBaseName;
        }

        public void AddNewFiles(string rootPath, ref List<string> newFiles)
        {
            var directory = new DirectoryInfo(rootPath);
            var allFiles = directory.GetFiles("*.xml");
            var last31days = DateTime.Now.AddDays(-31);
            var files = allFiles.Where(f => f.LastWriteTime > last31days).OrderByDescending(f => f.LastWriteTime);

            foreach (var fileInfo in files)
            {
                var fileNameNoFileType = Path.GetFileNameWithoutExtension(fileInfo.Name); // remove .XML
                int cycle = FileHelper.GetCycleFromFilename(fileNameNoFileType);
                var fileNameNoCycle = Path.GetFileNameWithoutExtension(fileNameNoFileType); // remove cycle
                var fileTableData = FileTableDB.GetFileTableDataForFileName(fileNameNoCycle);

                if ((cycle == fileTableData.Cycle) && (fileTableData.Active.HasValue) && (fileTableData.Active.Value))
                    newFiles.Add(fileInfo.FullName);
            }
        }

        public bool ProcessNewFile(string fullPath, ref List<string> errors)
        {
            bool fileProcessedSuccessfully = false;

            string fileNameNoExtension = Path.GetFileNameWithoutExtension(fullPath);
            string fileNameNoCycle = FileHelper.RemoveCycleFromFilename(fileNameNoExtension).ToUpper();

            if (fileNameNoExtension?.ToUpper()[6] == 'I') // incoming file have a I in 7th position (e.g. ON3D01IT.123456)
            {                                             //                                                    ↑

                string xmlData = File.ReadAllText(fullPath);
                string jsonText = FileHelper.ConvertXmlToJson(xmlData, ref errors);

                if (errors.Any())
                    return false;

                if (fileNameNoCycle == InterceptionBaseName)
                {
                    fileProcessedSuccessfully = ProcessMEPincomingInterceptionFile(errors, fileNameNoExtension, jsonText);
                }
                else if (fileNameNoCycle == LicencingBaseName)
                {
                    fileProcessedSuccessfully = ProcessMEPincomingLicencingFile(errors, fileNameNoExtension, jsonText);
                }
                else if (fileNameNoCycle == TracingBaseName)
                {
                    fileProcessedSuccessfully = ProcessMEPincomingTracingFile(errors, fileNameNoExtension, jsonText);

                }
                else
                {
                    errors.Add($"Error: Unrecognized file name '{fileNameNoCycle}'");
                }

            }
            else
            {
                errors.Add($"Error: expected 'I' in 7th position, but instead found '{fileNameNoExtension?.ToUpper()[6]}'. Is this an incoming file?");
            }

            return fileProcessedSuccessfully;
        }

        public bool ProcessMEPincomingTracingFile(List<string> errors, string fileNameNoExtension, string jsonText)
        {
            bool fileProcessedSuccessfully;
            var response = APIHelper.PostJsonFile($"api/v1/TracingFiles?fileName={fileNameNoExtension}", jsonText, ApiFilesConfig.FileBrokerMEPTracingRootAPI);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                ColourConsole.WriteEmbeddedColorLine($"[red]Error: {response.Content?.ReadAsStringAsync().Result}[/red]");
                errors.Add($"TracingFiles API failed with return code: {response.Content?.ReadAsStringAsync().Result}");
                fileProcessedSuccessfully = false;
            }
            else
            {
                fileProcessedSuccessfully = true;
            }

            return fileProcessedSuccessfully;
        }

        public bool ProcessMEPincomingLicencingFile(List<string> errors, string fileNameNoExtension, string jsonText)
        {
            bool fileProcessedSuccessfully;
            var response = APIHelper.PostJsonFile($"api/v1/LicenceDenialFiles?fileName={fileNameNoExtension}", jsonText, ApiFilesConfig.FileBrokerMEPLicenceDenialRootAPI);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                ColourConsole.WriteEmbeddedColorLine($"[red]Error: {response.Content?.ReadAsStringAsync().Result}[/red]");
                errors.Add($"LicenceDenialFiles API failed with return code: {response.Content?.ReadAsStringAsync().Result}");
                fileProcessedSuccessfully = false;
            }
            else
            {
                fileProcessedSuccessfully = true;
            }

            return fileProcessedSuccessfully;
        }

        public bool ProcessMEPincomingInterceptionFile(List<string> errors, string fileNameWithCycle, string jsonText)
        {
            bool fileProcessedSuccessfully;
            var response = APIHelper.PostJsonFile($"api/v1/InterceptionFiles?fileName={fileNameWithCycle}", jsonText, ApiFilesConfig.FileBrokerMEPInterceptionRootAPI);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                ColourConsole.WriteEmbeddedColorLine($"[red]Error: {response.Content?.ReadAsStringAsync().Result}[/red]");
                errors.Add($"InterceptionFiles API failed with return code: {response.Content?.ReadAsStringAsync().Result}");
                fileProcessedSuccessfully = false;
            }
            else
            {
                fileProcessedSuccessfully = true;
            }

            return fileProcessedSuccessfully;
        }
    }
}
