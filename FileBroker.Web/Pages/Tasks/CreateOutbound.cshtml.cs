using DBHelper;
using FileBroker.Business;
using FileBroker.Common;
using FileBroker.Data;
using FileBroker.Model;
using FileBroker.Model.Interfaces;
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
        private IFileBrokerConfigurationHelper Config { get; }

        public string InfoMessage { get; set; }
        public string ErrorMessage { get; set; }

        public List<FileTableData> ActiveOutgoingProcesses { get; set; }

        public CreateOutboundModel(IFileTableRepository fileTable,
                                   IFlatFileSpecificationRepository flatFileSpecs,
                                   IOutboundAuditRepository outboundAuditDB,
                                   IErrorTrackingRepository errorTrackingDB,
                                   IProcessParameterRepository processParameterTable,
                                   IMailServiceRepository mailServiceDB,
                                   IFileBrokerConfigurationHelper config)
        {
            FileTable = fileTable;
            FlatFileSpecs = flatFileSpecs;
            OutboundAuditDB = outboundAuditDB;
            ErrorTrackingDB = errorTrackingDB;
            ProcessParameterTable = processParameterTable;
            MailServiceDB = mailServiceDB;
            Config = config;
        }

        public async Task OnGet()
        {
            ActiveOutgoingProcesses = (await FileTable.GetAllActiveAsync()).Where(m => m.Type.ToLower() == "out").ToList();
        }

        public async Task OnPostCreateFiles(int[] selectedProcesses)
        {
            var foaeaApis = FoaeaApiHelper.SetupFoaeaAPIs(Config.ApiRootData);

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
                            outgoingFileManager = new OutgoingProvincialLicenceDenialManager(foaeaApis, repositories, Config);
                            break;
                        case "TRCAPPOUT":
                            outgoingFileManager = new OutgoingProvincialTracingManager(foaeaApis, repositories, Config);
                            break;
                        case "STATAPPOUT":
                            outgoingFileManager = new OutgoingProvincialStatusManager(foaeaApis, repositories, Config);
                            break;
                        case "TRCOUT":
                            outgoingFileManager = new OutgoingFederalTracingManager(foaeaApis, repositories, Config);
                            break;
                        case "SINOUT":
                            outgoingFileManager = new OutgoingFederalSinManager(foaeaApis, repositories, Config);
                            break;
                        case "LICOUT":
                            outgoingFileManager = new OutgoingFederalLicenceDenialManager(foaeaApis, repositories, Config);
                            break;
                    }

                    (filePath, errors) = await outgoingFileManager.CreateOutputFileAsync(thisProcess.Name);
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
