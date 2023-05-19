using FOAEA3.Common;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Areas.Administration.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class GendersController : FoaeaControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("Genders API Version 1.0");

    [HttpGet("DB")]
    [Authorize(Roles = Roles.Admin)]
    public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

    [HttpGet]
    public ActionResult<DataList<GenderData>> GetGenders()
    {
        List<GenderData> items = ReferenceData.Instance().Genders.Values.ToList();
        items.Sort();
        var data = new DataList<GenderData>(items, string.Empty);

        return Ok(data);
    }
}
