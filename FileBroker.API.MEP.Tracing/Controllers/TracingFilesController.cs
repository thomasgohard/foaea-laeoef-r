using FileBroker.Common;
using FileBroker.Model.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NJsonSchema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBroker.API.MEP.Tracing.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "MEPtracing,System")]
public class TracingFilesController : ControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("TracingFiles API Version 1.0");

    [HttpGet("DB")]
    public ActionResult<string> GetDatabase([FromServices] IFileTableRepository fileTable) => Ok(fileTable.MainDB.ConnectionString);

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

    [HttpPost]
    public async Task<IActionResult> ReceiveFile([FromQuery] string fileName, [FromServices] IFileTableRepository fileTable)
    {
        return await FileHelper.ExtractAndSaveRequestBodyToFile(fileName, fileTable, Request);
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
    
}
