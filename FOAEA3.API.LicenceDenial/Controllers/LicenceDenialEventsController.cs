using FOAEA3.Business.Areas.Application;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FOAEA3.API.LicenceDenial.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class LicenceDenialEventsController : ControllerBase
    {
        private readonly CustomConfig config;

        public LicenceDenialEventsController(IOptions<CustomConfig> config)
        {
            this.config = config.Value;
        }

        [HttpGet("Version")]
        public ActionResult<string> GetVersion() => Ok("LicenceDenialEvents API Version 1.0");

        [HttpGet("DB")]
        public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

        [HttpGet("RequestedLICIN")]
        public ActionResult<ApplicationEventData> GetRequestedLICINTracingEvents([FromQuery] string enforcementServiceCode,
                                                                                 [FromQuery] string appl_EnfSrv_Cd, 
                                                                                 [FromQuery] string appl_CtrlCd,
                                                                                 [FromServices] IRepositories repositories)
        {
            var manager = new LicenceDenialManager(repositories, config);

            if (string.IsNullOrEmpty(enforcementServiceCode))
                return BadRequest("Missing enforcementServiceCode parameter");

            if (string.IsNullOrEmpty(appl_EnfSrv_Cd))
                return BadRequest("Missing appl_EnfSrv_Cd parameter");
            
            if (string.IsNullOrEmpty(appl_CtrlCd))
                return BadRequest("Missing appl_CtrlCd parameter");

            var result = manager.GetRequestedLICINLicenceDenialEvents(enforcementServiceCode, appl_EnfSrv_Cd, appl_CtrlCd);
            return Ok(result);

        }

    }
}
