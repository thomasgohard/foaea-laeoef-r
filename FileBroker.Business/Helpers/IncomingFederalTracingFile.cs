using FOAEA3.Model.Structs;
using System.Text;

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

        public async Task AddNewFilesAsync(string rootPath, List<string> newFiles)
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
                var fileNameNoCycle = Path.GetFileNameWithoutExtension(fileName); // remove cycle
                var fileTableData = await DB.FileTable.GetFileTableDataForFileNameAsync(fileNameNoCycle);

                if ((cycle == fileTableData.Cycle) && (fileTableData.Active.HasValue) && (fileTableData.Active.Value))
                    newFiles.Add(fileInfo.FullName);
            }
        }

        public async Task<bool> ProcessNewFileAsync(string fullPath, List<string> errors)
        {
            string fileNameNoPath = Path.GetFileName(fullPath);

            if (fileNameNoPath?.ToUpper()[6] == 'I') // incoming file have a I in 7th position (e.g. EI3STSIT.000022)
            {                                        //                                                    ↑ 
                string flatFile;
                using (var streamReader = new StreamReader(fullPath, Encoding.UTF8))
                {
                    flatFile = streamReader.ReadToEnd();
                }

                var tracingManager = new IncomingFederalTracingManager(FoaeaApis, DB, Config);

                var fileFullName = fullPath;
                if (fileFullName.EndsWith(".XML", StringComparison.InvariantCultureIgnoreCase))
                    fileFullName = fileFullName[..^4];
                var fileNameNoCycle = Path.GetFileNameWithoutExtension(fileFullName);

                var fileTableData = await DB.FileTable.GetFileTableDataForFileNameAsync(fileNameNoCycle);
                if (!fileTableData.IsLoading)
                {
                    var fInfo = new FileInfo(fullPath);
                    if (await FileHelper.CheckForDuplicateFile(fInfo, DB.MailService, Config))
                    {
                        errors.Add("Duplicate file found!");
                        return false;
                    }

                    if (!fileTableData.IsXML)
                        await tracingManager.ProcessFlatFileAsync(flatFile, fullPath);
                    else
                    {
                        string jsonText = FileHelper.ConvertXmlToJson(flatFile, errors);
                        await tracingManager.ProcessXmlFileAsync(jsonText, fileFullName);
                    }

                    if (!errors.Any())
                    {
                        string errorDoingBackup = await FileHelper.BackupFile(fullPath, DB, Config);

                        if (!string.IsNullOrEmpty(errorDoingBackup))
                            await DB.ErrorTrackingTable.MessageBrokerErrorAsync($"File Error: {fullPath}",
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
            else
                Errors.Add($"Error: expected 'I' in 7th position, but instead found '{fileNameNoPath?.ToUpper()[6]}'. Is this an incoming file?");

            return false;
        }

    }
}
