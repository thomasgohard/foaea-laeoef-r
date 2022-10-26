using FileBroker.Business;
using FileBroker.Data;
using FileBroker.Model;
using FileBroker.Model.Interfaces;
using FOAEA3.Common.Brokers;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBroker.API.Fed.SIN.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "SinRegistry,System")]
public class SinFilesController : ControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("SinFiles API Version 1.0");

    [HttpGet("Identity")]
    public ActionResult GetIdentityInfo()
    {
        var user = User.Identity;
        return Ok(user?.Name);
    }

    [HttpGet("DB")]
    public ActionResult<string> GetDatabase([FromServices] IFileTableRepository fileTable) => Ok(fileTable.MainDB.ConnectionString);

    [HttpGet]
    public async Task<IActionResult> GetFileAsync([FromServices] IFileTableRepository fileTable)
    {
        string fileContent;
        string lastFileName;

        (fileContent, lastFileName) = await LoadLatestFederalSinFileAsync(fileTable);

        if (fileContent == null)
            return NotFound();

        byte[] result = Encoding.UTF8.GetBytes(fileContent);

        return File(result, "text/plain", lastFileName);
    }

    private static async Task<(string, string)> LoadLatestFederalSinFileAsync(IFileTableRepository fileTable)
    {
        string lastFileName;
        var fileTableData = (await fileTable.GetFileTableDataForCategoryAsync("SINOUT"))
                                 .FirstOrDefault(m => m.Active.HasValue && m.Active.Value);

        if (fileTableData is null)
        {
            lastFileName = "";
            return ($"Error: fileTableData is empty for category SINOUT.", lastFileName);
        }

        var fileLocation = fileTableData.Path;
        int lastFileCycle = fileTableData.Cycle;

        int fileCycleLength = 3; // TODO: should come from FileTable

        var lifeCyclePattern = new string('0', fileCycleLength);
        string lastFileCycleString = lastFileCycle.ToString(lifeCyclePattern);
        lastFileName = $"{fileTableData.Name}.{lastFileCycleString}";

        string fullFilePath = $"{fileLocation}{lastFileName}";
        if (System.IO.File.Exists(fullFilePath))
            return (System.IO.File.ReadAllText(fullFilePath), lastFileName);
        else
            return (null, null);

    }

    [HttpPost]
    public async Task<ActionResult> ProcessSINFileAsync([FromQuery] string fileName,
                                       [FromServices] IFileAuditRepository fileAuditDB,
                                       [FromServices] IFileTableRepository fileTableDB,
                                       [FromServices] IMailServiceRepository mailService,
                                       [FromServices] IProcessParameterRepository processParameterDB,
                                       [FromServices] IFlatFileSpecificationRepository flatFileSpecs,
                                       [FromServices] IOptions<ProvincialAuditFileConfig> auditConfig,
                                       [FromServices] IOptions<ApiConfig> apiConfig,
                                       [FromServices] IConfiguration config,
                                       [FromHeader] string currentSubmitter,
                                       [FromHeader] string currentSubject)
    {
        string flatFileContent;
        using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
        {
            flatFileContent = await reader.ReadToEndAsync();
        }

        if (string.IsNullOrEmpty(fileName))
            return UnprocessableEntity("Missing fileName");

        if (fileName.ToUpper().EndsWith(".XML"))
            fileName = fileName[0..^4]; // remove .XML extension

        var apiHelper = new APIBrokerHelper(apiConfig.Value.FoaeaApplicationRootAPI, currentSubmitter, currentSubject);

        string token = "";
        var apis = new APIBrokerList
        {
            // TODO: fix token
            Sins = new SinAPIBroker(apiHelper, token),
            ApplicationEvents = new ApplicationEventAPIBroker(apiHelper, token),
            Applications = new ApplicationAPIBroker(apiHelper, token)
        };

        var repositories = new RepositoryList
        {
            FlatFileSpecs = flatFileSpecs,
            FileAudit = fileAuditDB,
            FileTable = fileTableDB,
            MailService = mailService,
            ProcessParameterTable = processParameterDB
        };

        var sinManager = new IncomingFederalSinManager(apis, repositories, config);

        var fileNameNoCycle = Path.GetFileNameWithoutExtension(fileName);
        var fileTableData = await fileTableDB.GetFileTableDataForFileNameAsync(fileNameNoCycle);
        if (!fileTableData.IsLoading)
        {
            var errors = await sinManager.ProcessFlatFileAsync(flatFileContent, fileName);
            if (errors.Any())
                return UnprocessableEntity(errors);
            else
                return Ok("File processed.");
        }
        else
            return UnprocessableEntity("File was already loading?");
    }
}
