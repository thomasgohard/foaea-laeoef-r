using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FOAEA3.API.Areas.Administration.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EnfOfficesController : ControllerBase
    {
        [HttpGet("Version")]
        public ActionResult<string> GetVersion() => Ok("EnfOffices API Version 1.0");

        [HttpGet]
        public ActionResult<List<EnfOffData>> GetEnfOffices([FromServices] IRepositories repositories,
                                                            [FromQuery] string enfOffName = null, [FromQuery] string enfOffCode = null,
                                                            [FromQuery] string province = null, [FromQuery] string enfServCode = null)
        {
            return Ok(repositories.EnfOffRepository.GetEnfOff(enfOffName, enfOffCode, province, enfServCode));
        }

    }
}
