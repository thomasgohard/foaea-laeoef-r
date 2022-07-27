using FOAEA3.Business.Areas.Application;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FOAEA3.API.Areas.Application.Controllers
{
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
                                                                                                               [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var searchManager = new ApplicationSearchManager(repositories);
            var result = searchManager.Search(quickSearchCriteria, out int totalCount);

            Response.Headers.Add("TotalCount", totalCount.ToString());

            return Ok(result);

        }

    }
}
