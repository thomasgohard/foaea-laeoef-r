using FileBroker.Business;
using FileBroker.Data;
using FileBroker.Model;
using FileBroker.Model.Interfaces;
using FOAEA3.Common.Brokers;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NJsonSchema;
using System.IO;
using System.Linq;
using System.Text;

namespace FileBroker.API.MEP.Tracing.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class InterceptionFilesController : ControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("InterceptionFiles API Version 1.0");

    //GET api/v1/TraceResults?partnerId=ON
    [HttpGet("")]
    public IActionResult GetLatestProvincialFile([FromQuery] string partnerId, [FromServices] IFileTableRepository fileTable)
    {
        string fileContent = LoadLatestProvincialTracingFile(partnerId, fileTable, out string lastFileName);

        if (fileContent == null)
            return NotFound();

        byte[] result = Encoding.UTF8.GetBytes(fileContent);

        return File(result, "text/xml", lastFileName);
    }

    private static string LoadLatestProvincialTracingFile(string partnerId, IFileTableRepository fileTable,
                                                          out string lastFileName)
    {
        var fileTableData = fileTable.GetFileTableDataForCategory("INTAPPOUT")
                                     .FirstOrDefault(m => m.Name.StartsWith(partnerId) &&
                                                          m.Active.HasValue && m.Active.Value);

        if (fileTableData is null)
        {
            lastFileName = "";
            return $"Error: fileTableData is empty for category INTAPPOUT.";
        }

        var fileLocation = fileTableData.Path;
        int lastFileCycle = fileTableData.Cycle;

        int fileCycleLength = 6; // TODO: should come from FileTable

        var lifeCyclePattern = new string('0', fileCycleLength);
        string lastFileCycleString = lastFileCycle.ToString(lifeCyclePattern);
        lastFileName = $"{fileTableData.Name}.{lastFileCycleString}.XML";

        string fullFilePath = $"{fileLocation}{lastFileName}";
        if (System.IO.File.Exists(fullFilePath))
            return System.IO.File.ReadAllText(fullFilePath);
        else
            return null;

    }

    [HttpPost]
    public ActionResult ProcessIncomingInterceptionFile([FromQuery] string fileName,
                                                        [FromServices] IFileAuditRepository fileAuditDB,
                                                        [FromServices] IFileTableRepository fileTableDB,
                                                        [FromServices] ITranslationRepository translationDB,
                                                        [FromServices] IMailServiceRepository mailService,
                                                        [FromServices] IOptions<ProvincialAuditFileConfig> auditConfig,
                                                        [FromServices] IOptions<ApiConfig> apiConfig,
                                                        [FromHeader] string currentSubmitter,
                                                        [FromHeader] string currentSubject)
    {
        string sourceInterceptionJsonData;
        using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
        {
            sourceInterceptionJsonData = reader.ReadToEndAsync().Result;
        }

        var schema = JsonSchema.FromType<MEPInterceptionFileData>();
        var errors = schema.Validate(sourceInterceptionJsonData);
        if (errors.Any())
        {
            return UnprocessableEntity(errors);
        }

        if (string.IsNullOrEmpty(fileName))
            return UnprocessableEntity("Missing fileName");

        if (fileName.ToUpper().EndsWith(".XML"))
            fileName = fileName[0..^4]; // remove .XML extension

        var apiHelper = new APIBrokerHelper(apiConfig.Value.FoaeaInterceptionRootAPI, currentSubmitter, currentSubject);
        var interceptionApplicationAPIs = new InterceptionApplicationAPIBroker(apiHelper);

        var apis = new APIBrokerList
        {
            InterceptionApplications = interceptionApplicationAPIs
        };

        var repositories = new RepositoryList
        {
            FileAudit = fileAuditDB,
            FileTable = fileTableDB,
            MailServiceDB = mailService,
            TranslationDB = translationDB
        };

        var interceptionManager = new IncomingProvincialInterceptionManager(fileName, apis, repositories, auditConfig.Value);

        var fileNameNoCycle = Path.GetFileNameWithoutExtension(fileName);
        var fileTableData = fileTableDB.GetFileTableDataForFileName(fileNameNoCycle);
        if (!fileTableData.IsLoading)
        {
            interceptionManager.ExtractAndProcessRequestsInFile(sourceInterceptionJsonData);
            return Ok("File processed.");
        }
        else
            return UnprocessableEntity("File was already loading?");

    }

}
