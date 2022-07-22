using FileBroker.Business;
using FileBroker.Data;
using FileBroker.Data.DB;
using FileBroker.Model;
using FileBroker.Model.Interfaces;
using FOAEA3.Common.Brokers;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace FileBroker.Web.Pages.Tasks
{
    public class CreateOutboundModel : PageModel
    {

        private IFileTableRepository FileTable { get; }
        private IFlatFileSpecificationRepository FlatFileSpecs { get; }
        private IOutboundAuditRepository OutboundAuditDB { get; }
        private IErrorTrackingRepository ErrorTrackingDB { get; }
        private IProcessParameterRepository ProcessParameterTable { get; }
        private IMailServiceRepository MailServiceDB { get; }
        private ApiConfig ApiConfig { get; }
        private IAPIBrokerHelper APIHelper { get; }

        public string InfoMessage { get; set; }
        public string ErrorMessage { get; set; }

        public List<FileTableData> ActiveOutgoingProcesses { get; set; }

        public CreateOutboundModel(IFileTableRepository fileTable, 
                                   IFlatFileSpecificationRepository flatFileSpecs, 
                                   IOutboundAuditRepository outboundAuditDB,
                                   IErrorTrackingRepository errorTrackingDB,
                                   IProcessParameterRepository processParameterTable,
                                   IMailServiceRepository mailServiceDB,
                                   IOptions<ApiConfig> apiConfig)
        {
            FileTable = fileTable;
            FlatFileSpecs = flatFileSpecs;
            OutboundAuditDB = outboundAuditDB;
            ErrorTrackingDB = errorTrackingDB;
            ProcessParameterTable = processParameterTable;
            MailServiceDB = mailServiceDB;
            ApiConfig = apiConfig.Value;
            APIHelper = new APIBrokerHelper(currentSubmitter: "MSGBRO", currentUser: "MSGBRO");
        }

        public void OnGet()
        {
            ActiveOutgoingProcesses = FileTable.GetAllActive().Where(m => m.Type.ToLower() == "out").ToList();
        }

        public void OnPostCreateFiles(int[] selectedProcesses)
        {
            // Category.In("TRCAPPOUT", "LICAPPOUT", "STATAPPOUT", "TRCOUT", "SINOUT", "LICOUT")
            var applicationApiHelper = new APIBrokerHelper(ApiConfig.FoaeaApplicationRootAPI, currentSubmitter: "MSGBRO", currentUser: "MSGBRO");
            var tracingApiHelper = new APIBrokerHelper(ApiConfig.FoaeaTracingRootAPI, currentSubmitter: "MSGBRO", currentUser: "MSGBRO");
            var licenceDenialApiHelper = new APIBrokerHelper(ApiConfig.FoaeaLicenceDenialRootAPI, currentSubmitter: "MSGBRO", currentUser: "MSGBRO");

            var apiBrokers = new APIBrokerList
            {
                Applications = new ApplicationAPIBroker(applicationApiHelper),
                ApplicationEvents = new ApplicationEventAPIBroker(applicationApiHelper),
                TracingApplications = new TracingApplicationAPIBroker(tracingApiHelper),
                TracingResponses = new TraceResponseAPIBroker(tracingApiHelper),
                TracingEvents = new TracingEventAPIBroker(tracingApiHelper),
                LicenceDenialApplications = new LicenceDenialApplicationAPIBroker(licenceDenialApiHelper),
                LicenceDenialTerminationApplications = new LicenceDenialTerminationApplicationAPIBroker(licenceDenialApiHelper),
                LicenceDenialResponses = new LicenceDenialResponseAPIBroker(licenceDenialApiHelper),
                LicenceDenialEvents = new LicenceDenialEventAPIBroker(licenceDenialApiHelper),
            };

            var repositories = new RepositoryList
            {
                FileTable = FileTable,
                FlatFileSpecs = FlatFileSpecs,
                OutboundAuditDB = OutboundAuditDB,
                ErrorTrackingDB = ErrorTrackingDB,
                ProcessParameterTable = ProcessParameterTable,
                MailServiceDB = MailServiceDB
            };

            var processData = FileTable.GetAllActive().Where(m => m.Type.ToLower() == "out").ToList();

            InfoMessage = string.Empty;
            int lastItem = selectedProcesses.Last();
            foreach (int processId in selectedProcesses)
            {
                var thisProcess = processData.First(m => m.PrcId == processId);

                string fileName;
                if (thisProcess.IsXML)
                    fileName = $"{thisProcess.Name.Trim()}.{thisProcess.Cycle,6:D6}.XML";
                else
                    fileName = $"{thisProcess.Name.Trim()}.{thisProcess.Cycle,3:D3}";

                InfoMessage += $"Creating  {fileName} [{processId}] in {thisProcess.Path}";
                if (processId != lastItem)
                    InfoMessage += "; ";

                switch (thisProcess.Category)
                {
                    case "STATAPPOUT":
                        var manager = new OutgoingProvincialStatusManager(apiBrokers, repositories);
                        
                        string filePath = manager.CreateOutputFile(thisProcess.Name, out List<string> errors);
                        if (errors.Count == 0)
                            InfoMessage = $"Successfully created {filePath}";
                        else
                            foreach (var error in errors)
                                ErrorMessage = $"Error creating {fileName}: {error}";
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
