using FOAEA3.Data.DB;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FOAEA3.API.Areas.Administration.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ActiveStatusesController : ControllerBase
    {
        [HttpGet]
        public ActionResult<DataList<ActiveStatusData>> GetActiveStatuses([FromServices] IActiveStatusRepository activeStatusRepository)
        {
            if (Request.Headers.ContainsKey("CurrentSubmitter"))
                activeStatusRepository.CurrentSubmitter = Request.Headers["CurrentSubmitter"];

            if (Request.Headers.ContainsKey("CurrentSubject"))
                activeStatusRepository.UserId = Request.Headers["CurrentSubject"];

            var data = activeStatusRepository.GetActiveStatus();

            return Ok(data);
        }
        [HttpGet("Version")]
        public ActionResult<string> Version()
        {
            return Ok("FOAEA3.API API Version 1.4");
        }
    }
}
