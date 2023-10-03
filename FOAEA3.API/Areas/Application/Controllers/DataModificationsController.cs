using FOAEA3.Business.Areas.Application;
using FOAEA3.Common;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Areas.Application.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DataModificationsController : FoaeaControllerBase
    {
        [HttpPut("update")]
        public async Task<ActionResult<StringData>> Update([FromServices] IRepositories repositories,
                                                       [FromServices] IRepositories_Finance repositoriesFinance)
        {
            var dataModicationsData = await APIBrokerHelper.GetDataFromRequestBody<DataModificationData>(Request);

            var dataModManager = new DataModificationManager(repositories, repositoriesFinance, config, User);

            string message = await dataModManager.Update(dataModicationsData);
            return Ok(new StringData { Data = message });
        }

        [HttpPost("SINpending/insert")]
        public async Task<ActionResult<StringData>> InsertCRAPendingUpdateSIN()
        {
            var sinModificationData = await APIBrokerHelper.GetDataFromRequestBody<SinModificationData>(Request);

            var sinManager = new ApplicationSINManager(null, null);
            string message = await sinManager.ProcessCRAPendingUpdateSIN("insert", sinModificationData.OldSIN, sinModificationData.NewSIN);

            return Ok(new StringData { Data = message });
        }

        [HttpPost("SINpending/delete")]
        public async Task<ActionResult<StringData>> DeleteCRAPendingUpdateSIN()
        {
            var sinModificationData = await APIBrokerHelper.GetDataFromRequestBody<SinModificationData>(Request);

            var sinManager = new ApplicationSINManager(null, null);
            string message = await sinManager.ProcessCRAPendingUpdateSIN("delete", newSIN: sinModificationData.NewSIN);

            return Ok(new StringData { Data = message });
        }
    }
}
