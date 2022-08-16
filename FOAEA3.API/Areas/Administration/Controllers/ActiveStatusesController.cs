using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Areas.Administration.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ActiveStatusesController : ControllerBase
{
    [HttpGet("Version")]
    public ActionResult<string> GetVersion() => Ok("ActiveStatuses API Version 1.0");

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

}
