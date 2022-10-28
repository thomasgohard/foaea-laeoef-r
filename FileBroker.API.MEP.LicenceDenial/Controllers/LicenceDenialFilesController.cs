using FileBroker.Business.Helpers;
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
using System.Text;

namespace FileBroker.API.MEP.LicenceDenial.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "MEPlicenceDenial,System")]
public class LicenceDenialFilesController : ControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("LicenceDenialFiles API Version 1.0");

    [HttpGet("DB")]
    public ActionResult<string> GetDatabase([FromServices] IFileTableRepository fileTable) => Ok(fileTable.MainDB.ConnectionString);

    [HttpGet("")]
    public async Task<IActionResult> GetLatestProvincialFile([FromQuery] string partnerId, [FromServices] IFileTableRepository fileTable)
    {
        string fileContent;
        string lastFileName;
        (fileContent, lastFileName) = await LoadLatestProvincialLicenceDenialFileAsync(partnerId, fileTable);

        if (fileContent == null)
            return NotFound();

        byte[] result = Encoding.UTF8.GetBytes(fileContent);

        return File(result, "text/xml", lastFileName);
    }

    private static async Task<(string, string)> LoadLatestProvincialLicenceDenialFileAsync(string partnerId, IFileTableRepository fileTable)
    {
        var fileTableData = (await fileTable.GetFileTableDataForCategoryAsync("LICAPPOUT"))
                                     .FirstOrDefault(m => m.Name.StartsWith(partnerId) &&
                                                          m.Active.HasValue && m.Active.Value);

        string lastFileName;

        if (fileTableData is null)
        {
            lastFileName = "";
            return ($"Error: fileTableData is empty for category LICAPPOUT.", lastFileName);
        }

        var fileLocation = fileTableData.Path;
        int lastFileCycle = fileTableData.Cycle;

        int fileCycleLength = 6; // TODO: should come from FileTable

        var lifeCyclePattern = new string('0', fileCycleLength);
        string lastFileCycleString = lastFileCycle.ToString(lifeCyclePattern);
        lastFileName = $"{fileTableData.Name}.{lastFileCycleString}.XML";

        string fullFilePath = $"{fileLocation}{lastFileName}";
        if (System.IO.File.Exists(fullFilePath))
            return (System.IO.File.ReadAllText(fullFilePath), lastFileName);
        else
            return (null, null);

    }

    [HttpPost]
    public async Task<ActionResult> ProcessIncomingLicenceDenialFileAsync([FromQuery] string fileName,
                                                   [FromServices] IFileAuditRepository fileAuditDB,
                                                   [FromServices] IFileTableRepository fileTableDB,
                                                   [FromServices] IMailServiceRepository mailService,
                                                   [FromServices] IOptions<ProvincialAuditFileConfig> auditConfig,
                                                   [FromServices] IOptions<ApiConfig> apiConfig,
                                                   [FromServices] IConfiguration config,
                                                   [FromHeader] string currentSubmitter,
                                                   [FromHeader] string currentSubject)
    {
        string sourceLicenceDenialJsonData;
        using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
        {
            sourceLicenceDenialJsonData = await reader.ReadToEndAsync();
        }

        var errors = JsonHelper.Validate<MEPLicenceDenialFileData>(sourceLicenceDenialJsonData, out List<UnknownTag> unknownTags);
        if (errors.Any())
        {
            return UnprocessableEntity(errors);
        }

        //foreach (var error in errors)
        //    Console.WriteLine(error.Path + ": " + error.Kind);

        if (string.IsNullOrEmpty(fileName))
            return UnprocessableEntity("Missing fileName");

        if (fileName.ToUpper().EndsWith(".XML"))
            fileName = fileName[0..^4]; // remove .XML extension

        string token = "";
        var apiApplHelper = new APIBrokerHelper(apiConfig.Value.FoaeaApplicationRootAPI, currentSubmitter, currentSubject);
        var applicationApplicationAPIs = new ApplicationAPIBroker(apiApplHelper, token);
        var productionAuditAPIs = new ProductionAuditAPIBroker(apiApplHelper, token);
        var loginAPIs = new LoginsAPIBroker(apiApplHelper, token);

        var apiLicenceDenialApplHelper = new APIBrokerHelper(apiConfig.Value.FoaeaLicenceDenialRootAPI, currentSubmitter, currentSubject);
        var licenceDenialApplicationAPIs = new LicenceDenialApplicationAPIBroker(apiLicenceDenialApplHelper, token);
        var licenceDenialTerminationApplicationAPIs = new LicenceDenialTerminationApplicationAPIBroker(apiLicenceDenialApplHelper, token);

        var apis = new APIBrokerList
        {
            Applications = applicationApplicationAPIs,
            LicenceDenialApplications = licenceDenialApplicationAPIs,
            LicenceDenialTerminationApplications = licenceDenialTerminationApplicationAPIs,
            ProductionAudits = productionAuditAPIs,
            Accounts = loginAPIs
        };

        var repositories = new RepositoryList
        {
            FileAudit = fileAuditDB,
            FileTable = fileTableDB,
            MailService = mailService
        };

        var licenceDenialManager = new IncomingProvincialLicenceDenialManager(fileName, apis, repositories, auditConfig.Value, config);

        var fileNameNoCycle = Path.GetFileNameWithoutExtension(fileName);
        var fileTableData = await fileTableDB.GetFileTableDataForFileNameAsync(fileNameNoCycle);
        if (!fileTableData.IsLoading)
        {
            await licenceDenialManager.ExtractAndProcessRequestsInFileAsync(sourceLicenceDenialJsonData, unknownTags);
            return Ok("File processed.");
        }
        else
            return UnprocessableEntity("File was already loading?");

    }
}
