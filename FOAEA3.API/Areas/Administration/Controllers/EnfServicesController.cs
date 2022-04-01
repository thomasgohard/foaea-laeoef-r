using FOAEA3.Business.Utilities;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Areas.Administration.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EnfServicesController : ControllerBase
    {
        [HttpGet("Version")]
        public ActionResult<string> GetVersion() => Ok("EnfServices API Version 1.0");

        [HttpGet("{enfServiceCode}")]
        public ActionResult<EnfSrvData> GetEnforcementService([FromRoute] string enfServiceCode, [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var enfSrvManager = new EnforcementServiceManager(repositories);
            EnfSrvData enfSrvData = enfSrvManager.GetEnforcementService(enfServiceCode);

            if (enfSrvData != null)
                return Ok(enfSrvData);
            else
                return UnprocessableEntity();
        }

        [HttpGet]
        public ActionResult<EnfSrvData> GetEnforcementServices([FromServices] IRepositories repositories,
                                                               [FromQuery] string enforcementServiceCode = null, [FromQuery] string enforcementServiceName = null,
                                                               [FromQuery] string enforcementServiceProvince = null, [FromQuery] string enforcementServiceCategory = null)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            return Ok(repositories.EnfSrvRepository.GetEnfService(enforcementServiceCode, enforcementServiceName, enforcementServiceProvince, enforcementServiceCategory));
        }
    }
}
