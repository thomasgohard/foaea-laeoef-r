using FOAEA3.Business.Areas.Application;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Areas.Application.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ApplicationSearchesController : ControllerBase
{
    [HttpGet("Version")]
    //[Authorize]
    public ActionResult<string> GetVersion() => Ok("ApplicationSearches API Version 1.0");

    [HttpGet("DB")]
    //[Authorize(Roles = "Administrator")]
    public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

    [HttpPost]
    //[Authorize]
    public async Task<ActionResult<List<ApplicationSearchResultData>>> DoSearch([FromBody] QuickSearchData quickSearchCriteria,
                                                                                [FromServices] IRepositories repositories,
                                                                                [FromQuery] int page = 1,
                                                                                [FromQuery] int perPage = 1000,
                                                                                [FromQuery] string orderBy = "EnforcementService, ControlCode")
    {
        var searchManager = new ApplicationSearchManager(repositories);
        List<ApplicationSearchResultData> result;
        int totalCount;
        (result, totalCount) = await searchManager.SearchAsync(quickSearchCriteria, page, perPage, orderBy);

        Response.Headers.Add("Page", page.ToString());
        Response.Headers.Add("PerPage", perPage.ToString());
        Response.Headers.Add("OrderBy", orderBy);
        Response.Headers.Add("TotalCount", totalCount.ToString());

        if (string.IsNullOrEmpty(searchManager.LastError))
            return Ok(result);
        else
            return UnprocessableEntity(searchManager.LastError);
    }

}
