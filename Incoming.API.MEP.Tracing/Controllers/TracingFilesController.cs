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
using System.Text;

namespace Incoming.API.MEP.Tracing.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TracingFilesController : ControllerBase
    {
        [HttpPost]
        public ActionResult ProcessTracingFile([FromQuery] string fileName,
                                               [FromServices] IFileAuditRepository fileAuditDB,
                                               [FromServices] IFileTableRepository fileTableDB,
                                               [FromServices] IMailServiceRepository mailService,
                                               [FromServices] IOptions<ProvincialAuditFileConfig> auditConfig,
                                               [FromServices] IOptions<ApiConfig> apiConfig)
        {
            string sourceTracingData;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                sourceTracingData = reader.ReadToEndAsync().Result;
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
                FileAudit = fileAuditDB,
                FileTable = fileTableDB,
                MailServiceDB = mailService
            };

            var tracingManager = new IncomingProvincialTracingManager(fileName, apis, repositories, auditConfig.Value);

            var fileNameNoCycle = Path.GetFileNameWithoutExtension(fileName);
            var fileTableData = fileTableDB.GetFileTableDataForFileName(fileNameNoCycle);
            if (!fileTableData.IsLoading)
            {
                tracingManager.ExtractAndProcessRequestsInFile(sourceTracingData);
                return Ok("File processed.");
            }
            else
                return UnprocessableEntity("File was already loading?");

        }

    }
}
