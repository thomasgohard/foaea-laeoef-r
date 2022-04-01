using FileBroker.Business;
using FileBroker.Data;
using FileBroker.Model;
using FileBroker.Model.Interfaces;
using FOAEA3.Common.Brokers;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IO;
using System.Linq;
using System.Text;

namespace Incoming.API.Fed.SIN.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SinFilesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetFile([FromServices] IFileTableRepository fileTable)
        {
            string fileContent = LoadLatestFederalSinFile(fileTable, out string lastFileName);

            if (fileContent == null)
                return NotFound();

            byte[] result = Encoding.UTF8.GetBytes(fileContent);

            return File(result, "text/plain", lastFileName);
        }

        private static string LoadLatestFederalSinFile(IFileTableRepository fileTable, out string lastFileName)
        {
            var fileTableData = fileTable.GetFileTableDataForCategory("SINOUT")
                                         .FirstOrDefault(m => m.Active.HasValue && m.Active.Value);

            var fileLocation = fileTableData.Path;
            int lastFileCycle = fileTableData.Cycle;

            int fileCycleLength = 3; // TODO: should come from FileTable

            var lifeCyclePattern = new string('0', fileCycleLength);
            string lastFileCycleString = lastFileCycle.ToString(lifeCyclePattern);
            lastFileName = $"{fileTableData.Name}.{lastFileCycleString}";

            string fullFilePath = $"{fileLocation}{lastFileName}";
            if (System.IO.File.Exists(fullFilePath))
                return System.IO.File.ReadAllText(fullFilePath);
            else
                return null;

        }

        [HttpPost]
        public ActionResult ProcessSINFile([FromQuery] string fileName,
                                           [FromServices] IFileAuditRepository fileAuditDB,
                                           [FromServices] IFileTableRepository fileTableDB,
                                           [FromServices] IMailServiceRepository mailService,
                                           [FromServices] IFlatFileSpecificationRepository flatFileSpecs,
                                           [FromServices] IOptions<ProvincialAuditFileConfig> auditConfig,
                                           [FromServices] IOptions<ApiConfig> apiConfig)
        {
            string flatFileContent;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                flatFileContent = reader.ReadToEndAsync().Result;
            }

            if (string.IsNullOrEmpty(fileName))
                return UnprocessableEntity("Missing fileName");

            if (fileName.ToUpper().EndsWith(".XML"))
                fileName = fileName[0..^4]; // remove .XML extension

            var apiHelper = new APIBrokerHelper(apiConfig.Value.FoaeaApplicationRootAPI, "", "");

            var apis = new APIBrokerList
            {
                SinAPIBroker = new SinAPIBroker(apiHelper),
                ApplicationEventAPIBroker = new ApplicationEventAPIBroker(apiHelper),
                ApplicationAPIBroker = new ApplicationAPIBroker(apiHelper)
            };

            var repositories = new RepositoryList
            {
                FlatFileSpecs = flatFileSpecs,
                FileAudit = fileAuditDB,
                FileTable = fileTableDB,
                MailServiceDB = mailService
            };

            var sinManager = new IncomingFederalSinManager(apis, repositories);

            var fileNameNoCycle = Path.GetFileNameWithoutExtension(fileName);
            var fileTableData = fileTableDB.GetFileTableDataForFileName(fileNameNoCycle);
            if (!fileTableData.IsLoading)
            {
                sinManager.ProcessFlatFile(flatFileContent, fileName);
                return Ok("File processed.");
            }
            else
                return UnprocessableEntity("File was already loading?");
        }
    }
}
