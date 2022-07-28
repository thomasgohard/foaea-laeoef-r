using FOAEA3.Business.Areas.Application;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Areas.Application.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ApplicationSearchesController : ControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("ApplicationSearches API Version 1.0");

    [HttpGet("DB")]
    public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

    [HttpPost]
    public ActionResult<List<ApplicationSearchResultData>> CreateApplicationSearchResultFromSearchCriteria([FromBody] QuickSearchData quickSearchCriteria,
                                                                                                           [FromServices] IRepositories repositories,
                                                                                                           [FromQuery] int page = 1,
                                                                                                           [FromQuery] int perPage = 20)
    {
        var searchManager = new ApplicationSearchManager(repositories);
        var result = searchManager.Search(quickSearchCriteria, out int totalCount, page, perPage);

        Response.Headers.Add("Page", page.ToString());
        Response.Headers.Add("PerPage", perPage.ToString());
        Response.Headers.Add("TotalCount", totalCount.ToString());

        return Ok(result);
    }

}
