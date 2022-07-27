using FOAEA3.Business.Utilities;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductionAuditsController : ControllerBase
    {
        [HttpGet("Version")]
        public ActionResult<string> GetVersion() => Ok("APIConfigurations API Version 1.0");

        [HttpGet("DB")]
        public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

        [HttpPost]
        public ActionResult<ProductionAuditData> InsertNotification([FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var productionAuditData = APIBrokerHelper.GetDataFromRequestBody<ProductionAuditData>(Request);

            var productionAuditManager = new ProductionAuditManager(repositories);
            productionAuditManager.Insert(productionAuditData);

            return Ok(productionAuditData);
        }
    }
}
