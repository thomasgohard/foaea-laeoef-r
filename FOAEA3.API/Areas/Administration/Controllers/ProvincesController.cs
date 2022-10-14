using FOAEA3.Model;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Areas.Administration.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProvincesController : ControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("Provinces API Version 1.0");

    [HttpGet("DB")]
    [Authorize(Roles = Roles.Admin)]
    public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

    [HttpGet]
    public async Task<ActionResult<List<ProvinceData>>> GetProvinces([FromServices] IRepositories repositories)
    {
        return Ok(await repositories.ProvinceTable.GetProvincesAsync());
    }

}
