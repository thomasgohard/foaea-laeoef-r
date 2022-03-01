using FileBroker.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text;

namespace Outgoing.API.MEP.Tracing.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TraceResultsController : ControllerBase
    {
        //GET api/v1/TraceResults?partnerId=ON
        [HttpGet("")]
        public IActionResult GetFile([FromQuery] string partnerId, [FromServices] IFileTableRepository fileTable)
        {
            string fileContent = LoadLatestFederalTracingFile(partnerId, fileTable, out string lastFileName);

            if (fileContent == null)
                return NotFound();

            byte[] result = Encoding.UTF8.GetBytes(fileContent);

            return File(result, "text/xml", lastFileName);
        }

        private static string LoadLatestFederalTracingFile(string partnerId, IFileTableRepository fileTable,
                                                           out string lastFileName)
        {
            var fileTableData = fileTable.GetFileTableDataForCategory("TRCAPPOUT")
                                         .FirstOrDefault(m => m.Name.StartsWith(partnerId) &&
                                                              m.Active.HasValue && m.Active.Value);

            var fileLocation = fileTableData.Path;
            int lastFileCycle = fileTableData.Cycle;

            int fileCycleLength = 6; // TODO: should come from FileTable

            var lifeCyclePattern = new string('0', fileCycleLength);
            string lastFileCycleString = lastFileCycle.ToString(lifeCyclePattern);
            lastFileName = $"{fileTableData.Name}.{lastFileCycleString}.XML";

            string fullFilePath = $"{fileLocation}{lastFileName}";
            if (System.IO.File.Exists(fullFilePath))
                return System.IO.File.ReadAllText(fullFilePath);
            else
                return null;

        }
    }
}
