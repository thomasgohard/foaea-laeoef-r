using FileBroker.Common.Helpers;
using FOAEA3.Model.Structs;

namespace FileBroker.Business.Helpers
{
    public class IncomingProvincialFile
    {
        private RepositoryList DB { get; }
        private FileBaseName FileBaseName { get; }
        private APIBrokerList FoaeaApis { get; }
        private IFileBrokerConfigurationHelper Config { get; }

        public IncomingProvincialFile(RepositoryList db,
                                      APIBrokerList foaeaApis,
                                      FileBaseName fileBaseName,
                                      IFileBrokerConfigurationHelper config)
        {
            DB = db;
            FileBaseName = fileBaseName;
            FoaeaApis = foaeaApis;
            Config = config;
        }

        public async Task<bool> ProcessWaitingFile(string fullPath, List<string> errors)
        {
            if (Path.GetExtension(fullPath)?.ToUpper() == ".XML")
                await ProcessIncomingXmlFile(fullPath, errors);

            else if (Path.GetExtension(fullPath)?.ToUpper() == ".ZIP")
                await ProcessIncomingESDfile(fullPath, errors);

            else
                errors.Add($"Unknown file type: {Path.GetExtension(fullPath)?.ToUpper()} for file {fullPath}");

            return errors.Any();
        }

        public async Task ProcessIncomingXmlFile(string fullPath, List<string> errors)
        {
            string fileNameNoExtension = Path.GetFileNameWithoutExtension(fullPath);
            string fileNameNoCycle = FileHelper.RemoveCycleFromFilename(fileNameNoExtension).ToUpper();

            if (fileNameNoCycle.Length != 8)
            {
                errors.Add($"Invalid MEP incoming file: {fileNameNoCycle}");
                return;
            }

            if (fileNameNoExtension?.ToUpper()[6] == 'I') // incoming file have a I in 7th position (e.g. ON3D01IT.123456)
            {
                string xmlData = File.ReadAllText(fullPath);
                string jsonText = FileHelper.ConvertXmlToJson(xmlData, errors);

                if (errors.Any())
                    return;

                char fileType = fileNameNoCycle[7];
                switch (fileType)
                {
                    case 'T':
                        await ProcessIncomingTracing(jsonText, fileNameNoExtension, errors);
                        break;

                    case 'I':
                        await ProcessIncomingInterception(jsonText, fileNameNoExtension, errors);
                        break;

                    case 'L':
                        await ProcessIncomingLicencing(jsonText, fileNameNoExtension, errors);
                        break;

                    case 'W':
                        // TODO: process swearing files? (this is disappearing and likely won't be needed anymore)
                        break;

                    default:
                        break;
                }
            }
            else
                errors.Add($"Error: expected 'I' in 7th position, but instead found '{fileNameNoExtension?.ToUpper()[6]}'. Is this an incoming file?");

        }

        private async Task ProcessIncomingTracing(string sourceTracingJsonData, string fileName, List<string> errors)
        {
            errors = JsonHelper.Validate<MEPTracingFileData>(sourceTracingJsonData, out List<UnknownTag> unknownTags);

            if (errors.Any())
            {
                errors = JsonHelper.Validate<MEPTracingFileDataSingle>(sourceTracingJsonData, out unknownTags);

                if (errors.Any())
                    return;
            }

            var tracingManager = new IncomingProvincialTracingManager(DB, FoaeaApis, fileName, Config);

            var fileNameNoCycle = Path.GetFileNameWithoutExtension(fileName);
            var fileTableData = await DB.FileTable.GetFileTableDataForFileNameAsync(fileNameNoCycle);
            if (!fileTableData.IsLoading)
            {
                var info = await tracingManager.ExtractAndProcessRequestsInFileAsync(sourceTracingJsonData, unknownTags);

                if ((info is not null) && (info.ContainsMessagesOfType(MessageType.Error)))
                    foreach (var error in info.GetMessagesForType(MessageType.Error))
                        errors.Add(error.Description);
            }
            else
                errors.Add("File was already loading?");
        }

        public async Task ProcessIncomingInterception(string sourceInterceptionJsonData, string fileName, List<string> errors)
        {
            errors = JsonHelper.Validate<MEPInterceptionFileData>(sourceInterceptionJsonData, out List<UnknownTag> unknownTags);

            if (errors.Any())
            {
                errors = JsonHelper.Validate<MEPInterceptionFileDataSingleSource>(sourceInterceptionJsonData, out unknownTags);

                if (errors.Any())
                    return;
            }

            var interceptionManager = new IncomingProvincialInterceptionManager(DB, FoaeaApis, fileName, Config);

            var fileNameNoCycle = Path.GetFileNameWithoutExtension(fileName);
            var fileTableData = await DB.FileTable.GetFileTableDataForFileNameAsync(fileNameNoCycle);
            if (!fileTableData.IsLoading)
            {
                var info = await interceptionManager.ExtractAndProcessRequestsInFileAsync(sourceInterceptionJsonData, unknownTags,
                                                                                          includeInfoInMessages: true);

                if ((info is not null) && (info.ContainsMessagesOfType(MessageType.Error)))
                    foreach (var error in info.GetMessagesForType(MessageType.Error))
                        errors.Add(error.Description);
            }
            else
                errors.Add("File was already loading?");
        }

        private async Task ProcessIncomingLicencing(string sourceLicenceDenialJsonData, string fileName, List<string> errors)
        {
            errors = JsonHelper.Validate<MEPLicenceDenialFileData>(sourceLicenceDenialJsonData, out List<UnknownTag> unknownTags);
            if (errors.Any())
            {
                errors = JsonHelper.Validate<MEPLicenceDenialFileDataSingle>(sourceLicenceDenialJsonData, out unknownTags);

                if (errors.Any())
                    return;
            }

            var licenceDenialManager = new IncomingProvincialLicenceDenialManager(DB, FoaeaApis, fileName, Config);

            var fileNameNoCycle = Path.GetFileNameWithoutExtension(fileName);
            var fileTableData = await DB.FileTable.GetFileTableDataForFileNameAsync(fileNameNoCycle);
            if (!fileTableData.IsLoading)
            {
                var info = await licenceDenialManager.ExtractAndProcessRequestsInFileAsync(sourceLicenceDenialJsonData, unknownTags);

                if ((info is not null) && (info.ContainsMessagesOfType(MessageType.Error)))
                    foreach (var error in info.GetMessagesForType(MessageType.Error))
                        errors.Add(error.Description);
            }
            else
                errors.Add("File was already loading?");
        }

        private async Task ProcessIncomingESDfile(string fullPath, List<string> errors)
        {
            var electronicSummonsManager = new IncomingProvincialElectronicSummonsManager(DB, FoaeaApis, fullPath, Config);

            var info = await electronicSummonsManager.ExtractAndProcessRequestsInFileAsync(fullPath);

            if ((info is not null) && (info.ContainsMessagesOfType(MessageType.Error)))
                foreach (var error in info.GetMessagesForType(MessageType.Error))
                    errors.Add(error.Description);
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
                var fileTableData = await DB.FileTable.GetFileTableDataForFileNameAsync(fileNameNoCycle);

                if ((cycle == fileTableData.Cycle) && (fileTableData.Active.HasValue) && (fileTableData.Active.Value))
                    newFiles.Add(fileInfo.FullName);
            }
        }

        public async Task AddNewESDfilesAsync(string rootPath, List<string> newFiles)
        {
            var directory = new DirectoryInfo(rootPath);

            if (directory.Exists)
            {
                var allFiles = await Task.Run(() => { return directory.GetFiles("*.zip"); });
                var last31days = DateTime.Now.AddDays(-31);
                var files = allFiles.Where(f => f.LastWriteTime > last31days).OrderByDescending(f => f.LastWriteTime);

                if (files.Any())
                {
                    var foaeaAccess = new FoaeaSystemAccess(FoaeaApis, Config.FoaeaLogin);

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

        public async Task<List<string>> GetWaitingFiles(List<string> searchPaths)
        {
            var allNewFiles = new List<string>();

            foreach (string searchPath in searchPaths)
                if (!searchPath.Contains("ESD"))
                    await AddNewXmlFilesAsync(searchPath, allNewFiles);
                else
                    await AddNewESDfilesAsync(searchPath, allNewFiles);

            return allNewFiles;
        }

    }
}
