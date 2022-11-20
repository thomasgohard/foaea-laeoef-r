using System.Text;

namespace FileBroker.Business.Helpers
{
    public class IncomingFederalTracingFile
    {
        private RepositoryList DB { get; }
        private APIBrokerList FoaeaApis { get; }
        private FileBrokerConfigurationHelper Config { get; }

        public List<string> Errors { get; }

        public IncomingFederalTracingFile(RepositoryList db,
                                          APIBrokerList foaeaApis,
                                          FileBrokerConfigurationHelper config)
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
                int cycle = FileHelper.GetCycleFromFilename(fileInfo.Name);
                var fileNameNoCycle = Path.GetFileNameWithoutExtension(fileInfo.Name); // remove cycle
                var fileTableData = await DB.FileTable.GetFileTableDataForFileNameAsync(fileNameNoCycle);

                if ((cycle == fileTableData.Cycle) && (fileTableData.Active.HasValue) && (fileTableData.Active.Value))
                    newFiles.Add(fileInfo.FullName);
            }
        }

        public async Task<bool> ProcessNewFileAsync(string fullPath)
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

                var fileNameNoCycle = Path.GetFileNameWithoutExtension(fullPath);
                var fileTableData = await DB.FileTable.GetFileTableDataForFileNameAsync(fileNameNoCycle);
                if (!fileTableData.IsLoading)
                {
                    await tracingManager.ProcessFlatFileAsync(flatFile, fullPath);
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
