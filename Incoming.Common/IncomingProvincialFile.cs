using DBHelper;
using FileBroker.Common;
using FileBroker.Data.DB;
using FileBroker.Model;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Incoming.Common
{
    public class IncomingProvincialFile
    {
        private DBFileTable FileTableDB { get; }
        private string InterceptionBaseName { get; }
        private string TracingBaseName { get; }
        private string LicencingBaseName { get; }
        private ApiConfig ApiFilesConfig { get; }
        private IAPIBrokerHelper APIHelper { get; }

        public IncomingProvincialFile(IDBTools mainDB,
                                      ApiConfig apiFilesConfig,
                                      IAPIBrokerHelper apiHelper,
                                      string interceptionBaseName,
                                      string tracingBaseName,
                                      string licencingBaseName)
        {
            ApiFilesConfig = apiFilesConfig;
            APIHelper = apiHelper;
            FileTableDB = new DBFileTable(mainDB);
            InterceptionBaseName = interceptionBaseName;
            TracingBaseName = tracingBaseName;
            LicencingBaseName = licencingBaseName;
        }

        public Dictionary<string, FileTableData> GetNewFiles(string rootPath, ref Dictionary<string, FileTableData> newFiles)
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
                    newFiles.Add(fileInfo.FullName, fileTableData);
            }

            return newFiles;
        }

        public bool ProcessNewFile(string fullPath, ref List<string> errors)
        {
            bool fileProcessedSuccessfully = false;

            string fileNameNoExtension = Path.GetFileNameWithoutExtension(fullPath);
            string fileNameNoCycle = FileHelper.RemoveCycleFromFilename(fileNameNoExtension).ToUpper();

            if (fileNameNoExtension?.ToUpper()[6] == 'I') // incoming file have a I in 7th position (e.g. ON3D01IT.123456)
            {                                            //                                                    ↑

                var doc = new XmlDocument(); // load xml file
                doc.Load(fullPath);

                string jsonText = JsonConvert.SerializeXmlNode(doc); // convert xml to json

                // send json to processor api

                if (fileNameNoCycle == InterceptionBaseName)
                {
                    var response = APIHelper.PostJsonFile($"api/v1/InterceptionFiles?fileName={fileNameNoExtension}", jsonText, ApiFilesConfig.FoaeaInterceptionRootAPI);
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        ColourConsole.WriteEmbeddedColorLine($"[red]Error: {response.Content?.ReadAsStringAsync().Result}[/red]");
                        errors.Add($"InterceptionFiles API failed with return code: {response.Content?.ReadAsStringAsync().Result}");
                        fileProcessedSuccessfully = false;
                    }
                    else
                    {
                        ColourConsole.WriteEmbeddedColorLine($"[green]InterceptionFiles API succeeded.[/green]");
                        fileProcessedSuccessfully = true;
                    }
                }
                else if (fileNameNoCycle == LicencingBaseName)
                {
                    var response = APIHelper.PostJsonFile($"api/v1/LicenceDenialFiles?fileName={fileNameNoExtension}", jsonText, ApiFilesConfig.FileBrokerFederalLicenceDenialRootAPI);
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        ColourConsole.WriteEmbeddedColorLine($"[red]Error: {response.Content?.ReadAsStringAsync().Result}[/red]");
                        errors.Add($"LicenceDenialFiles API failed with return code: {response.Content?.ReadAsStringAsync().Result}");
                        fileProcessedSuccessfully = false;
                    }
                    else
                    {
                        ColourConsole.WriteEmbeddedColorLine($"[green]LicenceDenialFiles API succeeded.[/green]");
                        fileProcessedSuccessfully = true;
                    }
                }
                else if (fileNameNoCycle == TracingBaseName)
                {
                    var response = APIHelper.PostJsonFile($"api/v1/TracingFiles?fileName={fileNameNoExtension}", jsonText, ApiFilesConfig.FoaeaTracingRootAPI);
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        ColourConsole.WriteEmbeddedColorLine($"[red]Error: {response.Content?.ReadAsStringAsync().Result}[/red]");
                        errors.Add($"TracingFiles API failed with return code: {response.Content?.ReadAsStringAsync().Result}");
                        fileProcessedSuccessfully = false;
                    }
                    else
                    {
                        ColourConsole.WriteEmbeddedColorLine($"[green]TracingFiles API succeeded.[/green]");
                        fileProcessedSuccessfully = true;
                    }

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

    }
}
