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

namespace Incoming.API.Fed.SIN.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SinFilesController : ControllerBase
    {
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

            var apiHelper = new APIBrokerHelper(apiConfig.Value.ApplicationRootAPI, "", "");

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
