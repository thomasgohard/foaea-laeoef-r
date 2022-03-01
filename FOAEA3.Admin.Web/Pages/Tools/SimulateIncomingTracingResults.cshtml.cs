using FOAEA3.Admin.Web.Models;
using FOAEA3.Common.Brokers;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Text;
using FOAEA3.Business.Areas.Application;
using Microsoft.Extensions.Options;
using FOAEA3.Model.Enums;

namespace FOAEA3.Admin.Web.Pages.Tools
{
    public class SimulateIncomingTracingResultsModel : PageModel
    {
        [BindProperty]
        public SimulateIncomingTracingResultsData SimulateIncomingTracingResults { get; set; }

        private ApiConfig ApiFilesConfig { get; }
        private IRepositories Repositories { get; }
        private readonly CustomConfig config;

        public SimulateIncomingTracingResultsModel(IOptions<ApiConfig> apiConfig, IRepositories repositories, IOptions<CustomConfig> config)
        {
            ApiFilesConfig = apiConfig.Value;
            Repositories = repositories;
            this.config = config.Value;
        }

        public void OnGet()
        {

        }

        public void OnPost()
        {
            if (ModelState.IsValid)
            {
                var data = SimulateIncomingTracingResults;

                // TODO: check if application exists and is in proper state
                var appManager = new TracingManager(Repositories, config);
                if (appManager.LoadApplication(data.EnfService, data.ControlCode))
                {
                    if (appManager.TracingApplication.AppCtgy_Cd != "T01")
                    {
                        ViewData["Error"] = $"Error: Invalid application category ({appManager.TracingApplication.AppCtgy_Cd}). Must be T01.";
                        return;
                    }

                    if (appManager.TracingApplication.AppLiSt_Cd.NotIn(ApplicationState.APPLICATION_ACCEPTED_10, ApplicationState.PARTIALLY_SERVICED_12, ApplicationState.APPLICATION_REINSTATED_11))
                    {
                        ViewData["Error"] = $"Error: Invalid application state ({appManager.TracingApplication.AppLiSt_Cd}). Must be either 10, 11, or 12.";
                        return;
                    }
                }
                else
                {
                    ViewData["Error"] = $"Error: Application {data.EnfService}-{data.ControlCode} not found!";
                    return;
                }

                if (SimulateIncomingTracingResults.IncomingTraceSource == "EI3STS") // NETP
                {
                }
                else if (SimulateIncomingTracingResults.IncomingTraceSource == "HR3STS") // EI incoming tracing
                {
                }
                else if (SimulateIncomingTracingResults.IncomingTraceSource == "RC3STS") // CRA incoming tracing
                {
                    // check and fix cycle
                    data.Cycle = data.Cycle.Trim();
                    if (data.Cycle.Length < 3)
                        data.Cycle = data.Cycle.PadLeft(3, '0');
                    else if (data.Cycle.Length > 3)
                    {
                        ViewData["Error"] = $"Error: Cycle length must be 3 or less!";
                        return;
                    }

                    // create fake incoming file body

                    string julianDate = DateTime.Now.AsJulianString();

                    var flatFile = new StringBuilder();
                    flatFile.AppendLine($"01            {data.Cycle}{julianDate}"); // header
                    flatFile.AppendLine($"02{data.EnfService,6}{data.ControlCode,6}           000010000100001"); // detail trace record
                    flatFile.AppendLine($"03{data.EnfService,6}{data.ControlCode,6}123 - 1234 TEST TEST TEST                                   MISSISSAUGA                ONL5N3C9    TEST.TEST.TEST.TEST.TEST.TEST.                                 2021054"); // detail employer address
                    flatFile.AppendLine($"04{data.EnfService,6}{data.ControlCode,6}123 - 1234 TEST TEST TEST                                   MISSISSAUGA                ONL5N3C9    TEST.TEST.TEST.TEST.TEST.TEST.                                 2021054"); // detail employer address
                    flatFile.AppendLine($"05{data.EnfService,6}{data.ControlCode,6}1234 TEST DRIVE                                             KAMLOOPS                   BCV2C4C3    CAN2021054"); // detail residential address (T4)
                    flatFile.AppendLine($"99999999000001"); // footer

                    string flatFileNameNoPath = $"RC3STSIT.{data.Cycle}";

                    // call Incoming Fed Tracing FileBroker API

                    var apiHelper = new APIBrokerHelper();
                    var broker = new IncomingFedTracingAPIbroker(apiHelper, ApiFilesConfig);
                    var result = broker.ProcessFlatFile(flatFileNameNoPath, flatFile.ToString());
                    if (result.IsSuccessStatusCode)
                        ViewData["Message"] = $"Successfully processed simulated trace results for {data.EnfService}-{data.ControlCode}";
                    else
                        ViewData["Error"] = $"Error {result.StatusCode}[{result.ReasonPhrase}]: Failed to process simulated trace results for {data.EnfService}-{data.ControlCode}";
                }
                else
                    ViewData["Error"] = "Error: Invalid Tracing Incoming Source";

            }

        }
    }
}
