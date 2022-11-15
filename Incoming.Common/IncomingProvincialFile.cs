using FileBroker.Common;
using FileBroker.Common.Helpers;
using FileBroker.Model.Interfaces;
using FOAEA3.Common.Brokers;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Structs;
using FOAEA3.Resources.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Incoming.Common
{
    public class IncomingProvincialFile
    {
        private IFileTableRepository FileTableDB { get; }
        private string InterceptionBaseName { get; }
        private string ESDbaseName { get; }
        private string TracingBaseName { get; }
        private string LicencingBaseName { get; }
        private ApiConfig ApiFilesConfig { get; }
        private IAPIBrokerHelper APIs { get; }
        private APIBrokerList FoaeaApis { get; }

        public IncomingProvincialFile(IFileTableRepository fileTable,
                                      ApiConfig apiFilesConfig,
                                      IAPIBrokerHelper apiHelper,
                                      FileBaseName fileBaseName,
                                      APIBrokerList foaeaApis)
        {
            ApiFilesConfig = apiFilesConfig;
            APIs = apiHelper;
            FileTableDB = fileTable;
            InterceptionBaseName = fileBaseName.Interception;
            TracingBaseName = fileBaseName.Tracing;
            LicencingBaseName = fileBaseName.Licencing;
            ESDbaseName = fileBaseName.ESD;
            FoaeaApis = foaeaApis;
        }

        public async Task AddNewXmlFilesAsync(string rootPath, List<string> newFiles)
        {
            var directory = new DirectoryInfo(rootPath);
            var allFiles = await Task.Run(() => { return directory.GetFiles("*.xml"); });
            var last31days = DateTime.Now.AddDays(-31);
            var files = allFiles.Where(f => f.LastWriteTime > last31days).OrderByDescending(f => f.LastWriteTime);

            foreach (var fileInfo in files)
            {
                var fileNameNoFileType = Path.GetFileNameWithoutExtension(fileInfo.Name); // remove .XML
                int cycle = FileHelper.GetCycleFromFilename(fileNameNoFileType);
                var fileNameNoCycle = Path.GetFileNameWithoutExtension(fileNameNoFileType); // remove cycle
                var fileTableData = await FileTableDB.GetFileTableDataForFileNameAsync(fileNameNoCycle);

                if ((cycle == fileTableData.Cycle) && (fileTableData.Active.HasValue) && (fileTableData.Active.Value))
                    newFiles.Add(fileInfo.FullName);
            }
        }

        public async Task AddNewESDfilesAsync(string rootPath, List<string> newFiles, FoaeaLoginData foaeaLoginData)
        {
            var directory = new DirectoryInfo(rootPath);

            if (directory.Exists)
            {
                var allFiles = await Task.Run(() => { return directory.GetFiles("*.zip"); });
                var last31days = DateTime.Now.AddDays(-31);
                var files = allFiles.Where(f => f.LastWriteTime > last31days).OrderByDescending(f => f.LastWriteTime);

                if (files.Any())
                {
                    var foaeaAccess = new FoaeaSystemAccess(FoaeaApis, foaeaLoginData);

                    await foaeaAccess.SystemLoginAsync();
                    try
                    {
                        foreach (var fileInfo in files)
                        {
                            var fileName = Path.GetFileName(fileInfo.FullName);

                            bool wasAlreadyLoaded = await FoaeaApis.InterceptionApplications.ESD_CheckIfAlreadyLoaded(fileName);

                            if (!wasAlreadyLoaded)
                                newFiles.Add(fileInfo.FullName);
                        }
                    }
                    finally
                    {
                        await foaeaAccess.SystemLogoutAsync();
                    }
                }
            }
        }

        public async Task<List<string>> GetWaitingFiles(List<string> searchPaths, FoaeaLoginData foaeaLoginData)
        {
            var allNewFiles = new List<string>();

            foreach (string searchPath in searchPaths)
                if (!searchPath.Contains("ESD"))
                    await AddNewXmlFilesAsync(searchPath, allNewFiles);
                else
                    await AddNewESDfilesAsync(searchPath, allNewFiles, foaeaLoginData);

            return allNewFiles;
        }

        public async Task<bool> ProcessWaitingFile(string fullPath, List<string> errors,
                                                    string userName, string userPassword)
        {
            bool fileProcessedSuccessfully = false;

            if (!fullPath.Contains("ESD"))
            {
                fileProcessedSuccessfully = await ProcessIncomingXmlFile(fullPath, errors, userName, userPassword);
            }
            else
            {
                // TODO: process ESD
                // fileProcessedSuccessfully = await ProcessIncomingESDfile(fullPath, errors, userName, userPassword);
            }

            return fileProcessedSuccessfully;
        }

        private async Task<bool> ProcessIncomingXmlFile(string fullPath, List<string> errors,
                                                        string userName, string userPassword)
        {
            bool fileProcessedSuccessfully = false;

            string fileNameNoExtension = Path.GetFileNameWithoutExtension(fullPath);
            string fileNameNoCycle = FileHelper.RemoveCycleFromFilename(fileNameNoExtension).ToUpper();

            if (fileNameNoExtension?.ToUpper()[6] == 'I') // incoming file have a I in 7th position (e.g. ON3D01IT.123456)
            {
                string xmlData = File.ReadAllText(fullPath);
                string jsonText = FileHelper.ConvertXmlToJson(xmlData, errors);

                if (errors.Any())
                    return false;

                var fileBrokerAccess = new FileBrokerSystemAccess(APIs, ApiFilesConfig, userName, userPassword);

                await fileBrokerAccess.SystemLoginAsync();

                try
                {
                    if (fileNameNoCycle == InterceptionBaseName)
                        fileProcessedSuccessfully = await ProcessMEPincomingInterceptionFileAsync(errors, fileNameNoExtension,
                                                                                                  jsonText,
                                                                                                  fileBrokerAccess.CurrentToken);
                    else if (fileNameNoCycle == LicencingBaseName)
                        fileProcessedSuccessfully = await ProcessMEPincomingLicencingFileAsync(errors, fileNameNoExtension,
                                                                                               jsonText,
                                                                                               fileBrokerAccess.CurrentToken);
                    else if (fileNameNoCycle == TracingBaseName)
                        fileProcessedSuccessfully = await ProcessMEPincomingTracingFileAsync(errors, fileNameNoExtension, jsonText,
                                                                                             fileBrokerAccess.CurrentToken);

                    else
                        errors.Add($"Error: Unrecognized file name '{fileNameNoCycle}'");
                }
                finally
                {
                    await fileBrokerAccess.SystemLogoutAsync();
                }

            }
            else
                errors.Add($"Error: expected 'I' in 7th position, but instead found '{fileNameNoExtension?.ToUpper()[6]}'. Is this an incoming file?");

            return fileProcessedSuccessfully;
        }

        //private async Task<bool> ProcessIncomingESDfile(string fullPath, List<string> errors,
        //                                                string userName, string userPassword)
        //{
        //    bool fileProcessedSuccessfully = false;

        //    // TODO: process ESD

        //    return fileProcessedSuccessfully;
        //}

        public async Task<bool> ProcessMEPincomingTracingFileAsync(List<string> errors, string fileNameNoExtension,
                                                               string jsonText, string token)
        {
            bool fileProcessedSuccessfully;
            var response = await APIs.PostJsonFileAsync($"api/v1/TracingFiles?fileName={fileNameNoExtension}",
                                            jsonText, rootAPI: ApiFilesConfig.FileBrokerMEPTracingRootAPI, token: token);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                if (response.Content is not null)
                    ColourConsole.WriteEmbeddedColorLine($"[red]Error: {await response.Content.ReadAsStringAsync()}[/red]");
                else
                    ColourConsole.WriteEmbeddedColorLine($"[red]Error[/red]");
                errors.Add($"TracingFiles API failed with return code: {response.StatusCode}");
                fileProcessedSuccessfully = false;
            }
            else
                fileProcessedSuccessfully = true;

            return fileProcessedSuccessfully;
        }

        public async Task<bool> ProcessMEPincomingLicencingFileAsync(List<string> errors, string fileNameNoExtension, string jsonText, string token)
        {
            bool fileProcessedSuccessfully;
            var response = await APIs.PostJsonFileAsync($"api/v1/LicenceDenialFiles?fileName={fileNameNoExtension}",
                                            jsonText, rootAPI: ApiFilesConfig.FileBrokerMEPLicenceDenialRootAPI, token: token);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                if (response.Content is not null)
                    ColourConsole.WriteEmbeddedColorLine($"[red]Error: {await response.Content.ReadAsStringAsync()}[/red]");
                else
                    ColourConsole.WriteEmbeddedColorLine($"[red]Error[/red]");
                errors.Add($"LicenceDenialFiles API failed with return code: {response.StatusCode}");
                fileProcessedSuccessfully = false;
            }
            else
                fileProcessedSuccessfully = true;

            return fileProcessedSuccessfully;
        }

        public async Task<bool> ProcessMEPincomingInterceptionFileAsync(List<string> errors,
                                                                        string fileNameWithCycle,
                                                                        string jsonText,
                                                                        string token)
        {
            bool fileProcessedSuccessfully;

            string apiCall = $"api/v1/InterceptionFiles?fileName={fileNameWithCycle}";
            string rootPath = ApiFilesConfig.FileBrokerMEPInterceptionRootAPI;

            var response = await APIs.PostJsonFileAsync(apiCall, jsonText, rootAPI: rootPath, token: token);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                string jsonData = String.Empty;
                if (response.Content is not null)
                    jsonData = await response.Content.ReadAsStringAsync();

                try
                {
                    if (!string.IsNullOrEmpty(jsonData))
                    {
                        var errorMessages = JsonConvert.DeserializeObject<MessageDataList>(jsonData);

                        if (errorMessages is not null)
                            foreach (var error in errorMessages.GetSystemMessagesForType(MessageType.Error))
                            {
                                ColourConsole.WriteEmbeddedColorLine($"[red]Error: {error.Description}[/red]");
                                errors.Add(error.Description);
                            }

                        else
                        {
                            ColourConsole.WriteEmbeddedColorLine($"[red]Error: {jsonData}[/red]");
                            errors.Add($"InterceptionFiles API failed with errors: {jsonData}");
                        }
                    }
                }
                catch
                {
                    ColourConsole.WriteEmbeddedColorLine($"[red]Error: {jsonData}[/red]");
                    errors.Add($"InterceptionFiles API failed with errors: {jsonData}");
                }

                fileProcessedSuccessfully = false;
            }
            else
                fileProcessedSuccessfully = true;

            return fileProcessedSuccessfully;
        }
    }
}
