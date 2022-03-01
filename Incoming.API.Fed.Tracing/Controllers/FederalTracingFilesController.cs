using FileBroker.Business;
using FileBroker.Data;
using FileBroker.Model;
using FileBroker.Model.Interfaces;
using FOAEA3.Common.Brokers;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IO;
using System.Text;

namespace Incoming.API.Fed.Tracing.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FederalTracingFilesController : ControllerBase
    {
        [HttpPost]
        public ActionResult ProcessTracingFile([FromQuery] string fileName,
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

            var apiHelper = new APIBrokerHelper(apiConfig.Value.TracingRootAPI, "", "");
            var tracingApplicationAPIs = new TracingApplicationAPIBroker(apiHelper);

            var apis = new APIBrokerList
            {
                TracingApplicationAPIBroker = tracingApplicationAPIs
            };

            var repositories = new RepositoryList
            {
                FlatFileSpecs = flatFileSpecs,
                FileAudit = fileAuditDB,
                FileTable = fileTableDB,
                MailServiceDB = mailService
            };

            var tracingManager = new IncomingFederalTracingManager(apis, repositories);

            var fileNameNoCycle = Path.GetFileNameWithoutExtension(fileName);
            var fileTableData = fileTableDB.GetFileTableDataForFileName(fileNameNoCycle);
            if (!fileTableData.IsLoading)
            {
                tracingManager.ProcessFlatFile(flatFileContent, fileName);
                return Ok("File processed.");
            }
            else
                return UnprocessableEntity("File was already loading?");
        }
    }
}
