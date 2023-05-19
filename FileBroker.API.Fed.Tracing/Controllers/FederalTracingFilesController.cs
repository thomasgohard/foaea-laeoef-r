using FileBroker.Common;
using FileBroker.Model.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Threading.Tasks;

namespace FileBroker.API.Fed.Tracing.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "FederalTracing,System")]
public class FederalTracingFilesController : ControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("FederalTracingFiles API Version 1.0");

    [HttpGet("DB")]
    public ActionResult<string> GetDatabase([FromServices] IFileTableRepository fileTable) => Ok(fileTable.MainDB.ConnectionString);

    //GET api/v1/TraceRequests?partnerId=RC
    [HttpGet]
    public async Task<IActionResult> GetLastFederalTracingFile([FromQuery] string partnerId, [FromServices] IFileTableRepository fileTable)
    {
        string fileName = partnerId + "3STSOT"; // e.g. RC3STSOT

        int fileCycleLength = 6; // TODO: should come from FileTable
        if (partnerId == "RC")
            fileCycleLength = 3;

        string fileContent;
        string lastFileCycleString;
        (fileContent, lastFileCycleString) = await LoadLatestFederalTracingFileAsync(fileName, fileTable, fileCycleLength);

        if (fileContent == null)
            return NotFound();

        byte[] result = Encoding.UTF8.GetBytes(fileContent);

        return File(result, "text/plain", fileName + "." + lastFileCycleString);
    }

    [HttpPost]
    public async Task<IActionResult> ReceiveFile([FromQuery] string fileName, [FromServices] IFileTableRepository fileTable)
    {
        return await FileHelper.ExtractAndSaveRequestBodyToFile(fileName, fileTable, Request);
    }

    private static async Task<(string, string)> LoadLatestFederalTracingFileAsync(string fileName, IFileTableRepository fileTable,
                                                       int fileCycleLength)
    {
        var fileTableData = await fileTable.GetFileTableDataForFileName(fileName);
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

        string fullFilePath = $"{fileLocation}{fileName}.{lastFileCycleString}";
        if (System.IO.File.Exists(fullFilePath))
            return (System.IO.File.ReadAllText(fullFilePath), lastFileCycleString);
        else
            return (null, null);

    }
}
