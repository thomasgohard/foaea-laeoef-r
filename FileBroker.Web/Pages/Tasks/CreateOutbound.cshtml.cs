using DBHelper;
using FileBroker.Business;
using FileBroker.Data;
using FileBroker.Model;
using FileBroker.Model.Interfaces;
using FOAEA3.Common.Brokers;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
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
        private IConfiguration Config { get; }

        public string InfoMessage { get; set; }
        public string ErrorMessage { get; set; }

        public List<FileTableData> ActiveOutgoingProcesses { get; set; }

        public CreateOutboundModel(IFileTableRepository fileTable,
                                   IFlatFileSpecificationRepository flatFileSpecs,
                                   IOutboundAuditRepository outboundAuditDB,
                                   IErrorTrackingRepository errorTrackingDB,
                                   IProcessParameterRepository processParameterTable,
                                   IMailServiceRepository mailServiceDB,
                                   IOptions<ApiConfig> apiConfig,
                                   IConfiguration config)
        {
            FileTable = fileTable;
            FlatFileSpecs = flatFileSpecs;
            OutboundAuditDB = outboundAuditDB;
            ErrorTrackingDB = errorTrackingDB;
            ProcessParameterTable = processParameterTable;
            MailServiceDB = mailServiceDB;
            ApiConfig = apiConfig.Value;
            Config = config;
        }

        public async Task OnGet()
        {
            ActiveOutgoingProcesses = (await FileTable.GetAllActiveAsync()).Where(m => m.Type.ToLower() == "out").ToList();
        }

        public async Task OnPostCreateFiles(int[] selectedProcesses)
        {
            var applicationApiHelper = new APIBrokerHelper(ApiConfig.FoaeaApplicationRootAPI, currentSubmitter: LoginsAPIBroker.SYSTEM_SUBMITTER, currentUser: LoginsAPIBroker.SYSTEM_SUBJECT);
            var tracingApiHelper = new APIBrokerHelper(ApiConfig.FoaeaTracingRootAPI, currentSubmitter: LoginsAPIBroker.SYSTEM_SUBMITTER, currentUser: LoginsAPIBroker.SYSTEM_SUBJECT);
            var licenceDenialApiHelper = new APIBrokerHelper(ApiConfig.FoaeaLicenceDenialRootAPI, currentSubmitter: LoginsAPIBroker.SYSTEM_SUBMITTER, currentUser: LoginsAPIBroker.SYSTEM_SUBJECT);

            // TODO: fix token
            string token = "";
            var apiBrokers = new APIBrokerList
            {
                Applications = new ApplicationAPIBroker(applicationApiHelper, token),
                ApplicationEvents = new ApplicationEventAPIBroker(applicationApiHelper, token),
                TracingApplications = new TracingApplicationAPIBroker(tracingApiHelper, token),
                TracingResponses = new TraceResponseAPIBroker(tracingApiHelper, token),
                TracingEvents = new TracingEventAPIBroker(tracingApiHelper, token),
                LicenceDenialApplications = new LicenceDenialApplicationAPIBroker(licenceDenialApiHelper, token),
                LicenceDenialTerminationApplications = new LicenceDenialTerminationApplicationAPIBroker(licenceDenialApiHelper, token),
                LicenceDenialResponses = new LicenceDenialResponseAPIBroker(licenceDenialApiHelper, token),
                LicenceDenialEvents = new LicenceDenialEventAPIBroker(licenceDenialApiHelper, token),
            };

            var repositories = new RepositoryList
            {
                FileTable = FileTable,
                FlatFileSpecs = FlatFileSpecs,
                OutboundAuditTable = OutboundAuditDB,
                ErrorTrackingTable = ErrorTrackingDB,
                ProcessParameterTable = ProcessParameterTable,
                MailService = MailServiceDB
            };

            var processData = (await FileTable.GetAllActiveAsync()).Where(m => m.Type.ToLower() == "out").ToList();

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

                List<string> errors = null;
                string filePath = string.Empty;
                if (thisProcess.Category.In("LICAPPOUT", "TRCAPPOUT", "STATAPPOUT", "TRCOUT", "SINOUT", "LICOUT"))
                {
                    IOutgoingFileManager outgoingFileManager = null;
                    switch (thisProcess.Category)
                    {
                        case "LICAPPOUT":
                            outgoingFileManager = new OutgoingProvincialLicenceDenialManager(apiBrokers, repositories, Config);
                            break;
                        case "TRCAPPOUT":
                            outgoingFileManager = new OutgoingProvincialTracingManager(apiBrokers, repositories, Config);
                            break;
                        case "STATAPPOUT":
                            outgoingFileManager = new OutgoingProvincialStatusManager(apiBrokers, repositories, Config);
                            break;
                        case "TRCOUT":
                            outgoingFileManager = new OutgoingFederalTracingManager(apiBrokers, repositories, Config);
                            break;
                        case "SINOUT":
                            outgoingFileManager = new OutgoingFederalSinManager(apiBrokers, repositories, Config);
                            break;
                        case "LICOUT":
                            outgoingFileManager = new OutgoingFederalLicenceDenialManager(apiBrokers, repositories, Config);
                            break;
                    }

                    filePath = await outgoingFileManager.CreateOutputFileAsync(thisProcess.Name, errors);
                }
                else
                    errors.Add($"Unsupported category [{thisProcess.Category}] for file {fileName}");

                if (errors is not null)
                {
                    if (errors.Count == 0)
                        InfoMessage = $"Successfully created {filePath}";
                    else
                        foreach (var error in errors)
                            ErrorMessage = $"Error creating {fileName}: {error}";
                }
            }
        }
    }
}
