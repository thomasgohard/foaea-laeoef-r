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
        (fileContent, lastFileName) = await LoadLatestProvincialInterceptionFileAsync(partnerId, fileTable);

        if (fileContent == null)
            return NotFound();

        byte[] result = Encoding.UTF8.GetBytes(fileContent);

        FileContentResult file = File(result, "text/xml", lastFileName);
        return file;
    }

    [HttpPost]
    public async Task<IActionResult> ReceiveFile([FromQuery] string fileName, [FromServices] IFileTableRepository fileTable)
    {
        return await FileHelper.ProcessIncomingFileAsync(fileName, fileTable, Request);
    }

    private static async Task<(string, string)> LoadLatestProvincialInterceptionFileAsync(string partnerId, IFileTableRepository fileTable)
    {
        var fileTableData = (await fileTable.GetFileTableDataForCategoryAsync("INTAPPOUT"))
                                     .FirstOrDefault(m => m.Name.StartsWith(partnerId) &&
                                                          m.Active.HasValue && m.Active.Value);

        string lastFileName;

        if (fileTableData is null)
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

}
