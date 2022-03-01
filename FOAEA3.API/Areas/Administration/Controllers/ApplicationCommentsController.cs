using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FOAEA3.Data.DB;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Areas.Administration.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ApplicationCommentsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<DataList<ApplicationCommentsData>> GetApplicationComments([FromServices] IApplicationCommentsRepository applicationCommentsRepository)
        {
            if (Request.Headers.ContainsKey("CurrentSubmitter"))
                applicationCommentsRepository.CurrentSubmitter = Request.Headers["CurrentSubmitter"];

            if (Request.Headers.ContainsKey("CurrentSubject"))
                applicationCommentsRepository.UserId = Request.Headers["CurrentSubject"];

            return Ok(applicationCommentsRepository.GetApplicationComments());
        }
    }
}
