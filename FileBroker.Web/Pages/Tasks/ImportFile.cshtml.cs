using DBHelper;
using FileBroker.Business;
using FileBroker.Business.Helpers;
using FileBroker.Common;
using FileBroker.Common.Helpers;
using FileBroker.Model.Interfaces;
using FOAEA3.Common.Brokers;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Structs;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Text;

namespace FileBroker.Web.Pages.Tasks
{
    public class ImportFileModel : PageModel
    {
        private IFileTableRepository FileTable { get; }
        private IAPIBrokerHelper APIHelper { get; }
        private IFileBrokerConfigurationHelper Config { get; }

        public IFormFile FormFile { get; set; }
        public string InfoMessage { get; set; }
        public string ErrorMessage { get; set; }

        public ImportFileModel(IFileTableRepository fileTable, IFileBrokerConfigurationHelper config)
        {
            FileTable = fileTable;
            APIHelper = new APIBrokerHelper(currentSubmitter: LoginsAPIBroker.SYSTEM_SUBMITTER, currentUser: LoginsAPIBroker.SYSTEM_SUBJECT);
            Config = config;
        }

        public async Task OnPostUpload()
        {
            var file = FormFile;
            if (file is not null)
            {
                var fileSize = file.Length / 1024;
                var fileName = file.FileName;

                if (fileName.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase))
                    fileName = Path.GetFileNameWithoutExtension(fileName); // remove XML extension first
                var fileNameNoExt = Path.GetFileNameWithoutExtension(fileName); // remove cycle, if any

                var incomingFileInfo = await FileTable.GetFileTableDataForFileName(fileNameNoExt);
                if (incomingFileInfo is null)
                {
                    ErrorMessage = "Error: Could not identify type of file from FileTable";
                    return;
                }

                InfoMessage = $"Loading and processing {fileName} [category: {incomingFileInfo.Category}, size: {fileSize} KB]...";

                var fileBaseName = new FileBaseName
                {
                    Interception = string.Empty,
                    Tracing = string.Empty,
                    Licencing = string.Empty
                };

                var fileBrokerDB = new DBToolsAsync(Config.FileBrokerConnection);
                var db = DataHelper.SetupFileBrokerRepositories(fileBrokerDB);

                var foaeaApis = FoaeaApiHelper.SetupFoaeaAPIs(Config.ApiRootData);

                var errors = new List<string>();

                if (file.ContentType.ToLower().In("text/xml", "application/json", "application/octet-stream"))
                {
                    var result = new StringBuilder();
                    using (var reader = new StreamReader(file.OpenReadStream()))
                    {
                        while (reader.Peek() >= 0)
                            result.AppendLine(reader.ReadLine());
                    }

                    string fileContent;
                    if (file.ContentType.ToLower() == "text/xml")
                        fileContent = FileHelper.ConvertXmlToJson(result.ToString(), errors);
                    else // file content is either json or flat file
                        fileContent = result.ToString();

                    switch (incomingFileInfo.Category)
                    {
                        case "INTAPPIN": // MEP Interception
                            var provincialFileHelper = new IncomingProvincialFile(db, foaeaApis, Config);
                            await provincialFileHelper.ProcessIncomingInterception(fileContent, fileName, errors);                            
                            break;

                        case "FAFRFTTRA": // FED Training FAs
                            var trainingFileManager = new IncomingFederalTrainingManager(foaeaApis, db, Config);
                            errors = await trainingFileManager.ProcessIncomingTraining(fileContent, fileName);
                            break;

                        case "FAFRFTTRT": // FED Training FTs
                            break;

                        case "FAFRFTOAS": // FED OAS FAs
                            break;

                        default:
                            ErrorMessage = "Not able to process files of category: " + incomingFileInfo.Category;
                            return;
                    }

                    if (errors.Any())
                    {
                        ErrorMessage = String.Empty;
                        string lastError = errors.Last();
                        foreach (string error in errors)
                        {
                            ErrorMessage += error;
                            ErrorMessage += (error == lastError) ? string.Empty : "; ";
                        }
                    }
                    else
                        InfoMessage = $"File {fileName} processed successfully.";

                }
                else
                {
                    ErrorMessage = "Error: Cannot process files of content type: " + file.ContentType;
                }

            }

            return;
        }
    }
}
