using FOAEA3.Business.Areas.Application;
using FOAEA3.Common;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Tracing.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CraFieldsController : FoaeaControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<CraFieldData>>> Get([FromServices] IRepositories repositories)
        {
            var manager = new TracingManager(repositories, config, User);
            return Ok(await manager.GetCraFields());
        }
    }
}
