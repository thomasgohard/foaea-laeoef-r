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
    public class FamilyProvisionsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<FamilyProvisionData>> GetFamilyProvisions([FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            return Ok(repositories.FamilyProvisionRepository.GetFamilyProvisions());
        }
    }
}
