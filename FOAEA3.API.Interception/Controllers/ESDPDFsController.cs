using FOAEA3.Business.Areas.Application;
using FOAEA3.Common;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Interception.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ESDPDFsController : FoaeaControllerBase
    {
        [HttpGet("{key}")]
        public async Task<List<ElectronicSummonsDocumentData>> FindDocumentsForApplication([FromRoute] string key, 
                                                                                           [FromServices] IRepositories repositories)
        {
            var applKey = new ApplKey(key);

            var manager = new ElectronicSummonsDocumentManager(repositories);
            await manager.SetCurrentUser(User);

            return await manager.FindDocumentsForApplication(applKey.EnfSrv, applKey.CtrlCd);
        }

        [HttpPost]
        public async Task<ActionResult<ElectronicSummonsDocumentPdfData>> AddESDPDF([FromServices] IRepositories repositories)
        {
            var pdfData = await APIBrokerHelper.GetDataFromRequestBody<ElectronicSummonsDocumentPdfData>(Request);

            var manager = new ElectronicSummonsDocumentManager(repositories);
            await manager.SetCurrentUser(User);

            var pdf = await manager.CreateESDPDF(pdfData);
            if (pdf is not null)
                return Ok(pdf);
            else
                return NotFound();
        }
    }
}
