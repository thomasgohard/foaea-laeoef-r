using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Areas.Administration.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProvincesController : ControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("Provinces API Version 1.0");

    [HttpGet]
    public ActionResult<List<ProvinceData>> GetProvinces([FromServices] IRepositories repositories)
    {
        return Ok(repositories.ProvinceRepository.GetProvinces());
    }

}
