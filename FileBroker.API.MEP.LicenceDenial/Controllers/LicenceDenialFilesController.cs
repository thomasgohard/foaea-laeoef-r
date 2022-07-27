using FileBroker.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FileBroker.API.MEP.LicenceDenial.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LicenceDenialFilesController : ControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("LicenceDenialFiles API Version 1.0");

    [HttpGet("DB")]
    public ActionResult<string> GetDatabase([FromServices] IFileTableRepository fileTable) => Ok(fileTable.MainDB.ConnectionString);
}
