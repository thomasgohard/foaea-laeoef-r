using DBHelper;
using FileBroker.Common;
using FileBroker.Data.DB;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Incoming.Common
{
    public class IncomingFederalLicenceDenialFile
    {
        private DBFileTable FileTable { get; }
        private ApiConfig ApiFilesConfig { get; }
        private IAPIBrokerHelper APIHelper { get; }
        public List<string> Errors { get; }

        public IncomingFederalLicenceDenialFile(IDBToolsAsync fileBrokerDB,
                                                ApiConfig apiFilesConfig,
                                                IAPIBrokerHelper apiHelper)
        {
            ApiFilesConfig = apiFilesConfig;
            APIHelper = apiHelper;
            FileTable = new DBFileTable(fileBrokerDB);
            Errors = new List<string>();
        }

        public async Task AddNewFilesAsync(string rootPath, List<string> newFiles)
        {
            var directory = new DirectoryInfo(rootPath);
            var allFiles = directory.GetFiles("*IL.*");
            var last31days = DateTime.Now.AddDays(-31);
            var files = allFiles.Where(f => f.LastWriteTime > last31days).OrderByDescending(f => f.LastWriteTime);

            foreach (var fileInfo in files)
            {
                int cycle = FileHelper.GetCycleFromFilename(fileInfo.Name);
                var fileNameNoXmlExt = Path.GetFileNameWithoutExtension(fileInfo.Name); // remove xml extension 
                var fileNameNoCycle = Path.GetFileNameWithoutExtension(fileNameNoXmlExt); // remove cycle extension
                var fileTableData = await FileTable.GetFileTableDataForFileNameAsync(fileNameNoCycle);

                if ((cycle == fileTableData.Cycle) && (fileTableData.Active.HasValue) && (fileTableData.Active.Value))
                    newFiles.Add(fileInfo.FullName);
            }
        }

        public async Task<bool> ProcessNewFileAsync(string fullPath)
        {
            bool fileProcessedSuccessfully = false;

            string fileNameNoPath = Path.GetFileName(fullPath);

            if (fileNameNoPath?.ToUpper()[6] == 'I') // incoming file have a I in 7th position (e.g. PA3SLSIL.001368.XML)
            {

                string xmlData = File.ReadAllText(fullPath);
                string jsonText = FileHelper.ConvertXmlToJson(xmlData, Errors); // convert xml to json

                if (Errors.Any())
                    return false;

                var response = await APIHelper.PostFlatFileAsync($"api/v1/FederalLicenceDenialFiles?fileName={fileNameNoPath}",
                                                      jsonText, ApiFilesConfig.FileBrokerFederalLicenceDenialRootAPI);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    if (response.Content is not null)
                        ColourConsole.WriteEmbeddedColorLine($"[red]Error: {await response.Content.ReadAsStringAsync()}[/red]");
                    else
                        ColourConsole.WriteEmbeddedColorLine($"[red]Error[/red]");
                    Errors.Add($"FederalLicenceDenialFiles API failed with return code: {response.StatusCode}");
                }
                else
                {
                    if (response.Content is not null)
                        ColourConsole.WriteEmbeddedColorLine($"[green]{await response.Content.ReadAsStringAsync()}[/green]");
                }

                fileProcessedSuccessfully = true;

            }
            else
                Errors.Add($"Error: expected 'I' in 7th position, but instead found '{fileNameNoPath?.ToUpper()[6]}'. Is this an incoming file?");

            return fileProcessedSuccessfully;
        }

    }
}
