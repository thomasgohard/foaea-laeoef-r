using FileBroker.Common;
using FileBroker.Model.Interfaces;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using Incoming.Common;
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

        public IFormFile FormFile { get; set; }
        public string InfoMessage { get; set; }
        public string ErrorMessage { get; set; }

        public ImportFileModel(IFileTableRepository fileTable, IOptions<ApiConfig> apiConfig)
        {
            FileTable = fileTable;
            ApiConfig = apiConfig.Value;
            APIHelper = new APIBrokerHelper(currentSubmitter: "MSGBRO", currentUser: "MSGBRO");
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

                var incomingFileInfo = FileTable.GetFileTableDataForFileName(fileNameNoExt);
                if (incomingFileInfo is null)
                {
                    ErrorMessage = "Error: Could not identify type of file from FileTable";
                    return;
                }

                InfoMessage = $"Loading and processing {fileName} [category: {incomingFileInfo.Category}, size: {fileSize} KB]...";

                var provincialFileManager = new IncomingProvincialFile(FileTable, ApiConfig, APIHelper,
                                                                       interceptionBaseName: string.Empty,
                                                                       tracingBaseName: string.Empty,
                                                                       licencingBaseName: string.Empty);

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
                            await provincialFileManager.ProcessMEPincomingInterceptionFileAsync(errors, fileName, fileContentAsJson);
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
