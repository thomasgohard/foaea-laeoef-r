using FileBroker.Business;
using FileBroker.Business.Helpers;
using FileBroker.Data;
using FileBroker.Model;
using FileBroker.Model.Interfaces;
using FOAEA3.Common.Brokers;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
[Authorize(Roles = "MEPinterception,System")]
public class InterceptionFilesController : ControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("InterceptionFiles API Version 1.0");

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

        FileContentResult file = File(result, "text/xml", lastFileName);
        return file;
    }

    private static async Task<(string, string)> LoadLatestProvincialTracingFileAsync(string partnerId, IFileTableRepository fileTable)
    {
        var fileTableData = (await fileTable.GetFileTableDataForCategoryAsync("INTAPPOUT"))
                                     .FirstOrDefault(m => m.Name.StartsWith(partnerId) &&
                                                          m.Active.HasValue && m.Active.Value);

        string lastFileName;
        
        if(fileTableData is null)
        {
            lastFileName = "";
            return ($"Error: fileTableData is empty for category INTAPPOUT.", lastFileName);
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
    public async Task<ActionResult> ProcessIncomingInterceptionFileAsync([FromQuery] string fileName,
                                                        [FromServices] IFileAuditRepository fileAuditDB,
                                                        [FromServices] IFileTableRepository fileTableDB,
                                                        [FromServices] ITranslationRepository translationDB,
                                                        [FromServices] IRequestLogRepository requestLogDB,
                                                        [FromServices] IMailServiceRepository mailService,
                                                        [FromServices] ILoadInboundAuditRepository loadInboundAuditData,
                                                        [FromServices] IOptions<ProvincialAuditFileConfig> auditConfig,
                                                        [FromServices] IOptions<ApiConfig> apiConfig,
                                                        [FromServices] IConfiguration config,
                                                        [FromHeader] string currentSubmitter,
                                                        [FromHeader] string currentSubject)
    {
        string sourceInterceptionJsonData;
        using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
        {
            sourceInterceptionJsonData = await reader.ReadToEndAsync();
        }


        var errors = JsonHelper.Validate<MEPInterceptionFileData>(sourceInterceptionJsonData, out List<UnknownTag> unknownTags);

        if (errors.Any())
            return UnprocessableEntity(errors);

        if (string.IsNullOrEmpty(fileName))
            return UnprocessableEntity("Missing fileName");

        if (fileName.ToUpper().EndsWith(".XML"))
            fileName = fileName[0..^4]; // remove .XML extension

        var apiApplHelper = new APIBrokerHelper(apiConfig.Value.FoaeaApplicationRootAPI, currentSubmitter, currentSubject);
        var applicationApplicationAPIs = new ApplicationAPIBroker(apiApplHelper);
        var productionAuditAPIs = new ProductionAuditAPIBroker(apiApplHelper);
        var loginAPIs = new LoginsAPIBroker(apiApplHelper);

        var apiInterceptionApplHelper = new APIBrokerHelper(apiConfig.Value.FoaeaInterceptionRootAPI, currentSubmitter, currentSubject);
        var interceptionApplicationAPIs = new InterceptionApplicationAPIBroker(apiInterceptionApplHelper);

        var apis = new APIBrokerList
        {
            Applications = applicationApplicationAPIs,
            InterceptionApplications = interceptionApplicationAPIs,
            ProductionAudits = productionAuditAPIs,
            Accounts = loginAPIs
        };

        var repositories = new RepositoryList
        {
            FileAudit = fileAuditDB,
            FileTable = fileTableDB,
            MailService = mailService,
            TranslationTable = translationDB,
            RequestLogTable = requestLogDB,
            LoadInboundAuditTable = loadInboundAuditData
        };

        var interceptionManager = new IncomingProvincialInterceptionManager(fileName, apis, repositories, auditConfig.Value, config);

        var fileNameNoCycle = Path.GetFileNameWithoutExtension(fileName);
        var fileTableData = await fileTableDB.GetFileTableDataForFileNameAsync(fileNameNoCycle);
        if (!fileTableData.IsLoading)
        {
            var info = await interceptionManager.ExtractAndProcessRequestsInFileAsync(sourceInterceptionJsonData, unknownTags, includeInfoInMessages: true);

            if ((info is not null) && (info.ContainsMessagesOfType(MessageType.Error)))
                if (info.ContainsSystemMessagesOfType(MessageType.Error))
                    return UnprocessableEntity(info);
                else
                    return Ok(info);

            return Ok("File processed.");
        }
        else
            return UnprocessableEntity("File was already loading?");

    }

}
