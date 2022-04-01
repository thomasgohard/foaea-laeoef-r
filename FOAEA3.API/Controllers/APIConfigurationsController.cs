using FOAEA3.Common.Helpers;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class APIConfigurationsController : ControllerBase
    {
        [HttpGet("Version")]
        public ActionResult<string> GetVersion() => Ok("APIConfigurations API Version 1.0");

        [HttpGet("MainDBCconnectionString")]
        public ActionResult<string> MainDBConnectionString([FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            return Ok(repositories.MainDB.ConnectionString);
        }

        [HttpGet("Messages")]
        public ActionResult<MessageDataList> GetMessages()
        {
            return Ok(ReferenceData.Instance().Messages);
        }
    }
}
