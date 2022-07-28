using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Areas.Administration.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class FamilyProvisionsController : ControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("FamilyProvisions API Version 1.0");

    [HttpGet]
    public ActionResult<List<FamilyProvisionData>> GetFamilyProvisions([FromServices] IRepositories repositories)
    {
        return Ok(repositories.FamilyProvisionRepository.GetFamilyProvisions());
    }
}
