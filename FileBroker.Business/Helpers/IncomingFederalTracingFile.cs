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
using System.Text;
using System.Threading.Tasks;

namespace FileBroker.Business.Helpers
{
    public class IncomingFederalTracingFile
    {
        private DBFileTable FileTable { get; }
        private ApiConfig ApiFilesConfig { get; }
        private IAPIBrokerHelper APIHelper { get; }
        public List<string> Errors { get; }

        public IncomingFederalTracingFile(IDBToolsAsync fileBrokerDB,
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
            var allFiles = directory.GetFiles("*IT.*");
            var last31days = DateTime.Now.AddDays(-31);
            var files = allFiles.Where(f => f.LastWriteTime > last31days).OrderByDescending(f => f.LastWriteTime);

            foreach (var fileInfo in files)
            {
                int cycle = FileHelper.GetCycleFromFilename(fileInfo.Name);
                var fileNameNoCycle = Path.GetFileNameWithoutExtension(fileInfo.Name); // remove cycle
                var fileTableData = await FileTable.GetFileTableDataForFileNameAsync(fileNameNoCycle);

                if ((cycle == fileTableData.Cycle) && (fileTableData.Active.HasValue) && (fileTableData.Active.Value))
                    newFiles.Add(fileInfo.FullName);
            }
        }

        public async Task<bool> ProcessNewFileAsync(string fullPath)
        {
            bool fileProcessedSuccessfully = false;

            string fileNameNoPath = Path.GetFileName(fullPath);

            if (fileNameNoPath?.ToUpper()[6] == 'I') // incoming file have a I in 7th position (e.g. EI3STSIT.000022)
            {                                 //                                                    ↑ 

                string flatFile;
                using (var streamReader = new StreamReader(fullPath, Encoding.UTF8))
                {
                    flatFile = streamReader.ReadToEnd();
                }

                // send json to processor api

                var response = await APIHelper.PostFlatFileAsync($"api/v1/FederalTracingFiles?fileName={fileNameNoPath}", flatFile, ApiFilesConfig.FileBrokerFederalTracingRootAPI);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    if (response.Content is not null)
                        ColourConsole.WriteEmbeddedColorLine($"[red]Error: {await response.Content.ReadAsStringAsync()}[/red]");
                    else
                        ColourConsole.WriteEmbeddedColorLine($"[red]Error[/red]");
                    Errors.Add($"FederalTracingFiles API failed with return code: {response.StatusCode}");
                }
                else if (response.Content is not null)
                    ColourConsole.WriteEmbeddedColorLine($"[green]{await response.Content.ReadAsStringAsync()}[/green]");

                fileProcessedSuccessfully = true;

            }
            else
                Errors.Add($"Error: expected 'I' in 7th position, but instead found '{fileNameNoPath?.ToUpper()[6]}'. Is this an incoming file?");

            return fileProcessedSuccessfully;
        }

    }
}
