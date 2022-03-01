using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FOAEA3.API.Areas.Administration.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class FoaEventsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<FoaEventDataDictionary> GetFoaeEvents()
        {
            return Ok(ReferenceData.Instance().FoaEvents);
        }
    }
}
