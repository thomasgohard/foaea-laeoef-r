using FileBroker.Model.Interfaces;
using FileBroker.Model;
using FOAEA3.Model;
using FOAEA3.Model.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.IO;
using System.Text;

namespace FileBroker.API.MEP.Interception.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize(Roles = "MEPinterception,System")]
    public class ESDfilesController : ControllerBase
    {
        //[HttpPost]
        //public async Task<ActionResult> ProcessIncomingESDfileAsync([FromQuery] string fileName,
        //                                               [FromServices] IFileAuditRepository fileAuditDB,
        //                                               [FromServices] IFileTableRepository fileTableDB,
        //                                               [FromServices] ITranslationRepository translationDB,
        //                                               [FromServices] IRequestLogRepository requestLogDB,
        //                                               [FromServices] IMailServiceRepository mailService,
        //                                               [FromServices] ILoadInboundAuditRepository loadInboundAuditData,
        //                                               [FromServices] IOptions<ProvincialAuditFileConfig> auditConfig,
        //                                               [FromServices] IOptions<ApiConfig> apiConfig,
        //                                               [FromServices] IConfiguration config,
        //                                               [FromHeader] string currentSubmitter,
        //                                               [FromHeader] string currentSubject)
        //{
        //    using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
        //    {
        //        sourceInterceptionJsonData = await reader.ReadToEndAsync();
        //    }
        //}
    }
}
