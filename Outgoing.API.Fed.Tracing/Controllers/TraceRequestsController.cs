using FileBroker.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;

namespace Outgoing.API.Fed.Tracing.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TraceRequestsController : ControllerBase
    {
        //GET api/v1/TraceRequests?partnerId=RC
        [HttpGet("")]
        public IActionResult GetFile([FromQuery] string partnerId, [FromServices] IFileTableRepository fileTable)
        {
            string fileName = partnerId + "3STSOT"; // e.g. RC3STSOT

            int fileCycleLength = 6; // TODO: should come from FileTable
            if (partnerId == "RC")
                fileCycleLength = 3;

            string fileContent = LoadLatestFederalTracingFile(fileName, fileTable, fileCycleLength, out string lastFileCycleString);

            if (fileContent == null)
                return NotFound();

            byte[] result = Encoding.UTF8.GetBytes(fileContent);

            return File(result, "text/plain", fileName + "." + lastFileCycleString);
        }

        private static string LoadLatestFederalTracingFile(string fileName, IFileTableRepository fileTable,
                                                           int fileCycleLength, out string lastFileCycleString)
        {
            var fileTableData = fileTable.GetFileTableDataForFileName(fileName);
            var fileLocation = fileTableData.Path;
            int lastFileCycle = fileTableData.Cycle; // - 1;
            //if (lastFileCycle < 1)
            //{
            //    // e.g. 10³ - 1 = 999
            //    // e.g. 10⁶ - 1 = 999999
            //    lastFileCycle = (int)Math.Pow(10, fileCycleLength) - 1;
            //}

            var lifeCyclePattern = new string('0', fileCycleLength);
            lastFileCycleString = lastFileCycle.ToString(lifeCyclePattern);

            string fullFilePath = $"{fileLocation}{fileName}.{lastFileCycleString}";
            if (System.IO.File.Exists(fullFilePath))
                return System.IO.File.ReadAllText(fullFilePath);
            else
                return null;

        }
    }
}
