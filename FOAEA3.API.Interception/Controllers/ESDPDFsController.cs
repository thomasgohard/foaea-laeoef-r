using FOAEA3.Business.Areas.Application;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Interception.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ESDPDFsController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<ElectronicSummonsDocumentPdfData>> CreateESDPDF([FromServices] IRepositories repositories)
        {
            var pdfData = await APIBrokerHelper.GetDataFromRequestBodyAsync<ElectronicSummonsDocumentPdfData>(Request);

            var manager = new ElectronicSummonsDocumentManager(repositories);
            await manager.SetCurrentUserAsync(User);

            var pdf = await manager.CreateESDPDF(pdfData);
            if (pdf is not null)
                return Ok(pdf);
            else
                return NotFound();
        }
    }
}
