using FOAEA3.Business.Areas.Application;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Repository;
using System.Threading.Tasks;

namespace FOAEA3.Business.Areas.BackendProcesses
{
    public class CreateESDC_NETP_eventsProcess
    {
        private readonly IFoaeaConfigurationHelper Config;
        private readonly IRepositories DB;

        public CreateESDC_NETP_eventsProcess(IRepositories repositories, IFoaeaConfigurationHelper config)
        {
            Config = config;
            DB = repositories;
        }

        public async Task RunAsync()
        {
            var prodAudit = DB.ProductionAuditTable;

            await prodAudit.InsertAsync("Create ESDC(NETP) Events Process", $"Create ESDC(NETP) Events Process Started", "O");

            var tracingManager = new TracingManager(DB, Config);
            await tracingManager.CreateNETPeventsAsync();

            await prodAudit.InsertAsync("Create ESDC(NETP) Events Process", $"Create ESDC(NETP) Events Process Completed", "O");
        }

    }
}
