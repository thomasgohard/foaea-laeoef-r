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
    public class IncomingFederalLicenceDenialFile
    {
        private DBFileTable FileTable { get; }
        private ApiConfig ApiFilesConfig { get; }
        private IAPIBrokerHelper APIHelper { get; }

        public IncomingFederalLicenceDenialFile(IDBTools fileBrokerDB,
                                                ApiConfig apiFilesConfig,
                                                IAPIBrokerHelper apiHelper)
        {
            ApiFilesConfig = apiFilesConfig;
            APIHelper = apiHelper;
            FileTable = new DBFileTable(fileBrokerDB);
        }

        public Dictionary<string, FileTableData> GetNewFiles(string rootPath)
        {
            var newFiles = new Dictionary<string, FileTableData>();

            var directory = new DirectoryInfo(rootPath);
            var allFiles = directory.GetFiles("*IL.*");
            var last31days = DateTime.Now.AddDays(-31);
            var files = allFiles.Where(f => f.LastWriteTime > last31days).OrderByDescending(f => f.LastWriteTime);

            foreach (var fileInfo in files)
            {
                int cycle = FileHelper.GetCycleFromFilename(fileInfo.Name);
                var fileNameNoXmlExt = Path.GetFileNameWithoutExtension(fileInfo.Name); // remove xml extension 
                var fileNameNoCycle = Path.GetFileNameWithoutExtension(fileNameNoXmlExt); // remove cycle extension
                var fileTableData = FileTable.GetFileTableDataForFileName(fileNameNoCycle);

                if ((cycle == fileTableData.Cycle) && (fileTableData.Active.HasValue) && (fileTableData.Active.Value))
                    newFiles.Add(fileInfo.FullName, fileTableData);
            }

            return newFiles;
        }

        public bool ProcessNewFile(string fullPath)
        {
            bool fileProcessedSuccessfully = false;

            string fileNameNoPath = Path.GetFileName(fullPath);

            if (fullPath.ToUpper()[6] == 'I') // incoming file have a I in 7th position (e.g. PA3SLSIL.001368.XML)
            {                                 //                                                    ↑ 

                var doc = new XmlDocument(); // load xml file
                doc.Load(fullPath);

                string jsonText = JsonConvert.SerializeXmlNode(doc); // convert xml to json

                // send XML??? to processor api

                var response = APIHelper.PostFlatFile($"api/v1/FederalLicenceDenialFiles?fileName={fileNameNoPath}",
                                                      jsonText, ApiFilesConfig.FileBrokerFederalLicenceDenialRootAPI);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    ColourConsole.WriteEmbeddedColorLine($"[red]Error: {response.Content?.ReadAsStringAsync().Result}[/red]");
                else
                    ColourConsole.WriteEmbeddedColorLine($"[green]{response.Content?.ReadAsStringAsync().Result}[/green]");

                fileProcessedSuccessfully = true;

            }
            else
            {
                // TODO: generate Unknown file name exception or ignore?
            }

            return fileProcessedSuccessfully;
        }
    }
}
