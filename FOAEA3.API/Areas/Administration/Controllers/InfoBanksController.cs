using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Areas.Administration.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class InfoBanksController : ControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("InfoBanks API Version 1.0");

    [HttpGet]
    public async Task<ActionResult<List<InfoBankData>>> GetInfoBanks([FromServices] IRepositories repositories)
    {
        return Ok(await repositories.InfoBankTable.GetInfoBanksAsync());
    }
}
