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
        private string InterceptionPrefix { get; }
        private string TracingPrefix { get; }
        private string LicencingPrefix { get; }
        private string ElectronicSummonsPrefix { get; }
        private ApiConfig ApiFilesConfig { get; }
        private IAPIBrokerHelper APIHelper { get; }

        public IncomingProvincialFile(IDBTools mainDB,
                                      ApiConfig apiFilesConfig,
                                      IAPIBrokerHelper apiHelper,
                                      string defaultProvincePrefix,
                                      string interceptionOverridePrefix = null,
                                      string tracingOverridePrefix = null,
                                      string licencingOverridePrefix = null,
                                      string electronicSummonsOverridePrefix = null)
        {
            ApiFilesConfig = apiFilesConfig;
            APIHelper = apiHelper;
            FileTableDB = new DBFileTable(mainDB);
            InterceptionPrefix = interceptionOverridePrefix ?? defaultProvincePrefix;
            TracingPrefix = tracingOverridePrefix ?? defaultProvincePrefix;
            LicencingPrefix = licencingOverridePrefix ?? defaultProvincePrefix;
            ElectronicSummonsPrefix = electronicSummonsOverridePrefix ?? defaultProvincePrefix;
        }

        public Dictionary<string, FileTableData> GetNewFiles(string rootPath)
        {
            var newFiles = new Dictionary<string, FileTableData>();

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

        public bool ProcessNewFile(string fullPath)
        {
            bool fileProcessedSuccessfully = false;

            string fileNameNoExtension = Path.GetFileNameWithoutExtension(fullPath);
            string fileNameNoCycle = FileHelper.RemoveCycleFromFilename(fileNameNoExtension).ToUpper();

            if (fileNameNoExtension.ToUpper()[6] == 'I') // incoming file have a I in 7th position (e.g. ON3D01IT.123456)
            {                                            //                                                    ↑

                var doc = new XmlDocument(); // load xml file
                doc.Load(fullPath);

                string jsonText = JsonConvert.SerializeXmlNode(doc); // convert xml to json

                // send json to processor api

                if (fileNameNoCycle == InterceptionPrefix + "II")
                {
                    APIHelper.PostJsonFile($"api/v1/InterceptionFiles?fileName={fileNameNoExtension}", jsonText, ApiFilesConfig.InterceptionRootAPI);
                    fileProcessedSuccessfully = true;
                }
                else if (fileNameNoCycle == LicencingPrefix + "IL")
                {
                    APIHelper.PostJsonFile($"api/v1/LicenceDenialFiles?fileName={fileNameNoExtension}", jsonText, ApiFilesConfig.IncomingFederalLicenceDenialRootAPI);
                    fileProcessedSuccessfully = true;
                }
                else if (fileNameNoCycle == TracingPrefix + "IT")
                {
                    var response = APIHelper.PostJsonFile($"api/v1/TracingFiles?fileName={fileNameNoExtension}", jsonText, ApiFilesConfig.TracingRootAPI);
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        ColourConsole.WriteEmbeddedColorLine($"[red]Error: {response.Content?.ReadAsStringAsync().Result}[/red]");
                    else
                        ColourConsole.WriteEmbeddedColorLine($"[green]{response.Content?.ReadAsStringAsync().Result}[/green]");

                    fileProcessedSuccessfully = true;
                }
                else if (fileNameNoCycle == LicencingPrefix + "IW")
                {
                    //                    APIHelper.PostJsonFile($"api/v1/AffidavitSwearingFiles?fileName={fileNameNoExtension}", jsonText, ApiFilesConfig.SwearingRootAPI);
                    fileProcessedSuccessfully = true;
                }

            }
            else if (fileNameNoExtension.ToUpper().StartsWith(ElectronicSummonsPrefix.Substring(0, 2) + "ESD"))
            {
                // TODO: call Incoming.API.MEP.ESD 
            }
            else
            {
                // TODO: generate Unknown file name exception or ignore?
            }

            return fileProcessedSuccessfully;
        }

    }
}
