using DBHelper;
using FileBroker.Business.Helpers;
using FileBroker.Common;
using FileBroker.Common.Helpers;
using FileBroker.Data.DB;
using FileBroker.Data;
using FileBroker.Model.Interfaces;
using FOAEA3.Common.Brokers;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Structs;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Text;

namespace FileBroker.Web.Pages.Tasks
{
    public class ImportFileModel : PageModel
    {
        private IFileTableRepository FileTable { get; }
        private ApiConfig ApiConfig { get; }
        private IAPIBrokerHelper APIHelper { get; }
        private IConfiguration Config { get; }

        public IFormFile FormFile { get; set; }
        public string InfoMessage { get; set; }
        public string ErrorMessage { get; set; }

        public ImportFileModel(IFileTableRepository fileTable, IOptions<ApiConfig> apiConfig, IConfiguration config)
        {
            FileTable = fileTable;
            ApiConfig = apiConfig.Value;
            APIHelper = new APIBrokerHelper(currentSubmitter: LoginsAPIBroker.SYSTEM_SUBMITTER, currentUser: LoginsAPIBroker.SYSTEM_SUBJECT);
            Config = config;
        }

        public async Task OnPostUpload(IConfiguration configuration)
        {
            var file = FormFile;
            if (file is not null)
            {
                var fileSize = file.Length / 1024;
                var fileName = file.FileName;

                if (fileName.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase))
                    fileName = Path.GetFileNameWithoutExtension(fileName); // remove XML extension first
                var fileNameNoExt = Path.GetFileNameWithoutExtension(fileName); // remove cycle, if any

                var incomingFileInfo = await FileTable.GetFileTableDataForFileNameAsync(fileNameNoExt);
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

                var fileBrokerDB = new DBToolsAsync(configuration.GetConnectionString("FileBroker").ReplaceVariablesWithEnvironmentValues());
                var db = DataHelper.SetupFileBrokerRepositories(fileBrokerDB);

                var apiRootData = configuration.GetSection("APIroot").Get<ApiConfig>();
                var foaeaApis = FoaeaApiHelper.SetupFoaeaAPIs(apiRootData);
                var provincialFileManager = new IncomingProvincialFile(db, foaeaApis, fileBaseName, configuration);

                var errors = new List<string>();

                if (file.ContentType.ToLower().In("text/xml", "application/json"))
                {
                    var result = new StringBuilder();
                    using (var reader = new StreamReader(file.OpenReadStream()))
                    {
                        while (reader.Peek() >= 0)
                            result.AppendLine(reader.ReadLine());
                    }

                    string fileContentAsJson;
                    if (file.ContentType.ToLower() == "text/xml")
                        fileContentAsJson = FileHelper.ConvertXmlToJson(result.ToString(), errors);
                    else // else already json
                        fileContentAsJson = result.ToString();

                    switch (incomingFileInfo.Category)
                    {
                        case "INTAPPIN":

                            string userName = Config["FILE_BROKER:userName"].ReplaceVariablesWithEnvironmentValues();
                            string userPassword = Config["FILE_BROKER:userPassword"].ReplaceVariablesWithEnvironmentValues();

                            var fileBrokerAccess = new FileBrokerSystemAccess(APIHelper, ApiConfig, userName, userPassword);

                            await fileBrokerAccess.SystemLoginAsync();
                            try
                            {
                                // TODO: fix this
                                //                                await provincialFileManager.ProcessMEPincomingInterceptionFileAsync(errors, fileName, fileContentAsJson, fileBrokerAccess.CurrentToken);
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
                            finally
                            {
                                await fileBrokerAccess.SystemLogoutAsync();
                            }
                            break;
                        default:
                            ErrorMessage = "Not able to process files of category: " + incomingFileInfo.Category;
                            return;
                    }
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
