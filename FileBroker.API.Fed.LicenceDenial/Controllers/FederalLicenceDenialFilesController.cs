using FileBroker.Business;
using FileBroker.Data;
using FileBroker.Model;
using FileBroker.Model.Interfaces;
using FOAEA3.Common.Brokers;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NJsonSchema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBroker.API.Fed.LicenceDenial.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "FederalLicenceDenial,System")]
public class FederalLicenceDenialFilesController : ControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("FederalLicenceDenialFiles API Version 1.0");

    [HttpGet("DB")]
    public ActionResult<string> GetDatabase([FromServices] IFileTableRepository fileTable) => Ok(fileTable.MainDB.ConnectionString);

    [HttpGet("")]
    public async Task<IActionResult> GetFile([FromQuery] string partnerId, [FromServices] IFileTableRepository fileTable)
    {
        string fileName = partnerId + "3SLSOL"; // e.g. PA3SLSOL

        int fileCycleLength = 6; // TODO: should come from FileTable

        string fileContent;
        string lastFileCycleString;

        (fileContent, lastFileCycleString) = await LoadLatestFederalLicenceDenialFileAsync(fileName, fileTable, fileCycleLength);

        if (fileContent == null)
            return NotFound();

        byte[] result = Encoding.UTF8.GetBytes(fileContent);

        return File(result, "text/xml", fileName + "." + lastFileCycleString + ".XML");
    }

    private static async Task<(string, string)> LoadLatestFederalLicenceDenialFileAsync(string fileName, IFileTableRepository fileTable,
                                                             int fileCycleLength)
    {
        var fileTableData = await fileTable.GetFileTableDataForFileNameAsync(fileName);
        var fileLocation = fileTableData.Path;
        int lastFileCycle = fileTableData.Cycle; // - 1;
                                                 //if (lastFileCycle < 1)
                                                 //{
                                                 //    // e.g. 10³ - 1 = 999
                                                 //    // e.g. 10⁶ - 1 = 999999
                                                 //    lastFileCycle = (int)Math.Pow(10, fileCycleLength) - 1;
                                                 //}

        var lifeCyclePattern = new string('0', fileCycleLength);
        string lastFileCycleString = lastFileCycle.ToString(lifeCyclePattern);

        string fullFilePath = $"{fileLocation}{fileName}.{lastFileCycleString}.XML";
        if (System.IO.File.Exists(fullFilePath))
            return (System.IO.File.ReadAllText(fullFilePath), lastFileCycleString);
        else
            return (null, null);

    }

    [HttpPost]
    public async Task<ActionResult> ProcessLicenceDenialFileAsync([FromQuery] string fileName,
                                                 [FromServices] IFileAuditRepository fileAuditDB,
                                                 [FromServices] IFileTableRepository fileTableDB,
                                                 [FromServices] IMailServiceRepository mailService,
                                                 [FromServices] IProcessParameterRepository processParameterDB,
                                                 [FromServices] IFlatFileSpecificationRepository flatFileSpecs,
                                                 [FromServices] IOptions<ProvincialAuditFileConfig> auditConfig,
                                                 [FromServices] IOptions<ApiConfig> apiConfig,
                                                 [FromHeader] string currentSubmitter,
                                                 [FromHeader] string currentSubject)
    {
        string sourceLicenceDenialJsonData;
        using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
        {
            sourceLicenceDenialJsonData = await reader.ReadToEndAsync();
        }

        var schema = JsonSchema.FromType<FedLicenceDenialFileData>();
        var errors = schema.Validate(sourceLicenceDenialJsonData);
        if (errors.Any())
            return UnprocessableEntity(errors);

        if (string.IsNullOrEmpty(fileName))
            return UnprocessableEntity("Missing fileName");

        if (fileName.ToUpper().EndsWith(".XML"))
            fileName = fileName[0..^4]; // remove .XML extension

        // TODO: fix token
        var token = "";
        var apiLicenceDenialHelper = new APIBrokerHelper(apiConfig.Value.FoaeaLicenceDenialRootAPI, currentSubmitter, currentSubject);
        var licenceDenialApplicationAPIs = new LicenceDenialApplicationAPIBroker(apiLicenceDenialHelper, token);
        var licenceDenialTerminationApplicationAPIs = new LicenceDenialTerminationApplicationAPIBroker(apiLicenceDenialHelper, token);
        var licenceDenialEventAPIs = new LicenceDenialEventAPIBroker(apiLicenceDenialHelper, token);
        var licenceDenialResponsesAPIs = new LicenceDenialResponseAPIBroker(apiLicenceDenialHelper, token);

        var apiApplicationHelper = new APIBrokerHelper(apiConfig.Value.FoaeaApplicationRootAPI, currentSubmitter, currentSubject);
        var applicationEventsAPIs = new ApplicationEventAPIBroker(apiApplicationHelper, token);

        var apis = new APIBrokerList
        {
            LicenceDenialApplications = licenceDenialApplicationAPIs,
            LicenceDenialTerminationApplications = licenceDenialTerminationApplicationAPIs,
            LicenceDenialEvents = licenceDenialEventAPIs,
            LicenceDenialResponses = licenceDenialResponsesAPIs,
            ApplicationEvents = applicationEventsAPIs
        };

        var repositories = new RepositoryList
        {
            FlatFileSpecs = flatFileSpecs,
            FileAudit = fileAuditDB,
            FileTable = fileTableDB,
            MailService = mailService,
            ProcessParameterTable = processParameterDB
        };

        var licenceDenialManager = new IncomingFederalLicenceDenialManager(apis, repositories);

        var fileNameNoCycle = Path.GetFileNameWithoutExtension(fileName);
        var fileTableData = await fileTableDB.GetFileTableDataForFileNameAsync(fileNameNoCycle);
        if (!fileTableData.IsLoading)
        {
            await licenceDenialManager.ProcessJsonFileAsync(sourceLicenceDenialJsonData, fileName);
            return Ok("File processed.");
        }
        else
            return UnprocessableEntity("File was already loading?");
    }
}
