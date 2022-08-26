using FileBroker.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FileBroker.API.Fed.Interception.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class FederalInterceptionFilesController : ControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("InterceptionFiles API Version 1.0");

    [HttpGet("DB")]
    public ActionResult<string> GetDatabase([FromServices] IFileTableRepository fileTable) => Ok(fileTable.MainDB.ConnectionString);

    [HttpGet("Test")]
    public async Task<ActionResult<string>> TestDBFailure([FromServices] IFileTableRepository fileTable)
    {
        await fileTable.MainDB.ExecProcAsync("ThisDoesNotExists");

        if (string.IsNullOrEmpty(fileTable.MainDB.LastError))
            return Ok();
        else
            return UnprocessableEntity(fileTable.MainDB.LastError);
    }
}
