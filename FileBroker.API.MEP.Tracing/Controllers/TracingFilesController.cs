using FileBroker.Business;
using FileBroker.Business.Helpers;
using FileBroker.Data;
using FileBroker.Model;
using FileBroker.Model.Interfaces;
using FOAEA3.Common.Brokers;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NJsonSchema;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBroker.API.MEP.Tracing.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TracingFilesController : ControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("TracingFiles API Version 1.0");

    [HttpGet("DB")]
    public ActionResult<string> GetDatabase([FromServices] IFileTableRepository fileTable) => Ok(fileTable.MainDB.ConnectionString);

    //GET api/v1/TraceResults?partnerId=ON
    [HttpGet("")]
    public async Task<IActionResult> GetLatestProvincialFile([FromQuery] string partnerId, [FromServices] IFileTableRepository fileTable)
    {
        string fileContent;
        string lastFileName;
        (fileContent, lastFileName) = await LoadLatestProvincialTracingFileAsync(partnerId, fileTable);

        if (fileContent == null)
            return NotFound();

        byte[] result = Encoding.UTF8.GetBytes(fileContent);

        return File(result, "text/xml", lastFileName);
    }

    private static async Task<(string, string)> LoadLatestProvincialTracingFileAsync(string partnerId, IFileTableRepository fileTable)
    {
        var fileTableData = (await fileTable.GetFileTableDataForCategoryAsync("TRCAPPOUT"))
                                     .FirstOrDefault(m => m.Name.StartsWith(partnerId) &&
                                                          m.Active.HasValue && m.Active.Value);

        string lastFileName;

        if (fileTableData is null)
        {
            lastFileName = "";
            return ($"Error: fileTableData is empty for category TRCAPPOUT.", lastFileName);
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
    public async Task<ActionResult> ProcessIncomingTracingFileAsync([FromQuery] string fileName,
                                                   [FromServices] IFileAuditRepository fileAuditDB,
                                                   [FromServices] IFileTableRepository fileTableDB,
                                                   [FromServices] IMailServiceRepository mailService,
                                                   [FromServices] IOptions<ProvincialAuditFileConfig> auditConfig,
                                                   [FromServices] IOptions<ApiConfig> apiConfig,
                                                   [FromHeader] string currentSubmitter,
                                                   [FromHeader] string currentSubject)
    {
        string sourceTracingJsonData;
        using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
        {
            sourceTracingJsonData = await reader.ReadToEndAsync();
        }

        var errors = JsonHelper.Validate<MEPTracingFileData>(sourceTracingJsonData, out List<UnknownTag> unknownTags);
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

        var apiHelper = new APIBrokerHelper(apiConfig.Value.FoaeaTracingRootAPI, currentSubmitter, currentSubject);
        var tracingApplicationAPIs = new TracingApplicationAPIBroker(apiHelper);

        var apis = new APIBrokerList
        {
            TracingApplications = tracingApplicationAPIs
        };

        var repositories = new RepositoryList
        {
            FileAudit = fileAuditDB,
            FileTable = fileTableDB,
            MailService = mailService
        };

        var tracingManager = new IncomingProvincialTracingManager(fileName, apis, repositories, auditConfig.Value);

        var fileNameNoCycle = Path.GetFileNameWithoutExtension(fileName);
        var fileTableData = await fileTableDB.GetFileTableDataForFileNameAsync(fileNameNoCycle);
        if (!fileTableData.IsLoading)
        {
            await tracingManager.ExtractAndProcessRequestsInFileAsync(sourceTracingJsonData, unknownTags);
            return Ok("File processed.");
        }
        else
            return UnprocessableEntity("File was already loading?");

    }

}
