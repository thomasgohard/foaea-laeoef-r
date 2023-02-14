using FOAEA3.Common;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Areas.Administration.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ApplicationReasonsController : FoaeaControllerBase
    {
        [HttpGet("Version")]
        public ActionResult<string> GetVersion() => Ok("ApplicationReasons API Version 1.0");

        [HttpGet("DB")]
        [Authorize(Roles = Roles.Admin)]
        public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

        [HttpGet]
        public ActionResult<DataList<ApplicationReasonData>> GetApplicationReasons()
        {
            List<ApplicationReasonData> items = ReferenceData.Instance().ApplicationReasons.Values.ToList();
            var data = new DataList<ApplicationReasonData>(items, string.Empty);

            return Ok(data);
        }
    }
}
