using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FOAEA3.API.Areas.Administration.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class InfoBanksController : ControllerBase
    {
        [HttpGet("Version")]
        public ActionResult<string> GetVersion() => Ok("InfoBanks API Version 1.0");

        [HttpGet]
        public ActionResult<List<InfoBankData>> GetInfoBanks([FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            return Ok(repositories.InfoBankRepository.GetInfoBanks());
        }
    }
}
