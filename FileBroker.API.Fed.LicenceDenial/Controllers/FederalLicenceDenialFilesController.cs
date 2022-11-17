using FileBroker.Model.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Threading.Tasks;

namespace FileBroker.API.Fed.LicenceDenial.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "FederalLicenceDenial,System")]
public class FederalLicenceDenialFilesController : ControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("FederalLicenceDenialFiles API Version 1.0");

    [HttpGet("DB")]
    public ActionResult<string> GetDatabase([FromServices] IFileTableRepository fileTable) => Ok(fileTable.MainDB.ConnectionString);

    [HttpGet("")]
    public async Task<IActionResult> GetFile([FromQuery] string partnerId, [FromServices] IFileTableRepository fileTable)
    {
        string fileName = partnerId + "3SLSOL"; // e.g. PA3SLSOL

        int fileCycleLength = 6; // TODO: should come from FileTable

        string fileContent;
        string lastFileCycleString;

        (fileContent, lastFileCycleString) = await LoadLatestFederalLicenceDenialFileAsync(fileName, fileTable, fileCycleLength);

        if (fileContent == null)
            return NotFound();

        byte[] result = Encoding.UTF8.GetBytes(fileContent);

        return File(result, "text/xml", fileName + "." + lastFileCycleString + ".XML");
    }

    private static async Task<(string, string)> LoadLatestFederalLicenceDenialFileAsync(string fileName, IFileTableRepository fileTable,
                                                             int fileCycleLength)
    {
        var fileTableData = await fileTable.GetFileTableDataForFileNameAsync(fileName);
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

        string fullFilePath = $"{fileLocation}{fileName}.{lastFileCycleString}.XML";
        if (System.IO.File.Exists(fullFilePath))
            return (System.IO.File.ReadAllText(fullFilePath), lastFileCycleString);
        else
            return (null, null);

    }

}
