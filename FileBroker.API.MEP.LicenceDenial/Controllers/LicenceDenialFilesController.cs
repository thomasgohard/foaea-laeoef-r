using FileBroker.Model.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
}
