using FOAEA3.Data.Base;
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
    public class GendersController : ControllerBase
    {
        [HttpGet]
        public ActionResult<DataList<GenderData>> GetGenders([FromServices] IGenderRepository genderRepository)
        {
            if (Request.Headers.ContainsKey("CurrentSubmitter"))
                genderRepository.CurrentSubmitter = Request.Headers["CurrentSubmitter"];

            if (Request.Headers.ContainsKey("CurrentSubject"))
                genderRepository.UserId = Request.Headers["CurrentSubject"];

            var data = genderRepository.GetGenders();

            return Ok(data);
        }
    }
}
