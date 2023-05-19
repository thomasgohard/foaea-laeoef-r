using FileBroker.Common;
using FileBroker.Model.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    [HttpPost]
    public async Task<IActionResult> ReceiveFile([FromQuery] string fileName, [FromServices] IFileTableRepository fileTable)
    {
        return await FileHelper.ExtractAndSaveRequestBodyToFile(fileName, fileTable, Request);
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

}
