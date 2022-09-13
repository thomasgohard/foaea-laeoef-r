using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Areas.Administration.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class FoaEventsController : ControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("FoaEvents API Version 1.0");

    [HttpGet("DB")]
    [Authorize(Roles = "Admin")]
    public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

    [HttpGet]
    public ActionResult<FoaEventDataDictionary> GetFoaeEvents()
    {
        return Ok(ReferenceData.Instance().FoaEvents);
    }
}
