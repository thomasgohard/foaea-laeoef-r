namespace FileBroker.Business.Helpers
{
    public class IncomingFederalTracingFile
    {
        private RepositoryList DB { get; }
        private APIBrokerList FoaeaApis { get; }
        private IFileBrokerConfigurationHelper Config { get; }
        public List<string> Errors { get; }

        public IncomingFederalTracingFile(RepositoryList db,
                                          APIBrokerList foaeaApis,
                                          IFileBrokerConfigurationHelper config)
        {
            DB = db;
            FoaeaApis = foaeaApis;
            Config = config;
            Errors = new List<string>();
        }

        public async Task GetNextExpectedIncomingFilesFoundInFolder(string rootPath, List<string> newFiles)
        {
            var directory = new DirectoryInfo(rootPath);
            var allFiles = directory.GetFiles("*IT.*");
            var last31days = DateTime.Now.AddDays(-31);
            var files = allFiles.Where(f => f.LastWriteTime > last31days).OrderByDescending(f => f.LastWriteTime);

            foreach (var fileInfo in files)
            {
                string fileName = fileInfo.Name;
                if (fileName.EndsWith(".XML", StringComparison.InvariantCultureIgnoreCase))
                    fileName = fileName[..^4];

                int cycle = FileHelper.ExtractCycleFromFilename(fileName);
                if (cycle != FileHelper.INVALID_CYCLE)
                {
                    var fileNameNoCycle = Path.GetFileNameWithoutExtension(fileName); // remove cycle
                    var fileTableData = await DB.FileTable.GetFileTableDataForFileName(fileNameNoCycle);

                    if ((cycle == fileTableData.Cycle) && (fileTableData.Type.ToLower() == "in") &&
                        (fileTableData.Active.HasValue) && (fileTableData.Active.Value))
                        newFiles.Add(fileInfo.FullName);
                }
            }
        }

        public async Task<bool> ProcessWaitingFile(string fullPath)
        {
            if (await FileHelper.CheckForDuplicateFile(fullPath, DB.MailService, Config))
            {
                Errors.Add("Duplicate file found!");
                return false;
            }

            string fileContent = File.ReadAllText(fullPath);

            var tracingManager = new IncomingFederalTracingManager(FoaeaApis, DB, Config);

            var fileFullName = fullPath;
            if (fileFullName.EndsWith(".XML", StringComparison.InvariantCultureIgnoreCase))
                fileFullName = Path.GetFileNameWithoutExtension(fileFullName);
            var fileNameNoCycle = Path.GetFileNameWithoutExtension(fileFullName);

            var fileTableData = await DB.FileTable.GetFileTableDataForFileName(fileNameNoCycle);
            if (!fileTableData.IsLoading)
            {
                await DB.FileTable.SetIsFileLoadingValue(fileTableData.PrcId, true);

                if (!fileTableData.IsXML)
                    Errors.AddRange(await tracingManager.ProcessFlatFileData(fileContent, fullPath));
                else
                    Errors.AddRange(await tracingManager.ProcessXmlData(fileContent, fileFullName));

                if (Errors.Count == 0)
                    await DB.FileTable.SetNextCycleForFileType(fileTableData);
                await DB.FileTable.SetIsFileLoadingValue(fileTableData.PrcId, false);

                if (!Errors.Any())
                {
                    string errorDoingBackup = await FileHelper.BackupFile(fullPath, DB, Config);

                    if (!string.IsNullOrEmpty(errorDoingBackup))
                        await DB.ErrorTrackingTable.MessageBrokerError($"File Error: {fullPath}",
                                                                        "Error creating backup of outbound file: " + errorDoingBackup);
                }

                return true;
            }
            else
            {
                Errors.Add("File was already loading?");
                return false;
            }
        }
    }
}
