using FOAEA3.Business.Areas.Application;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Repository;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FOAEA3.Business.Areas.BackendProcesses
{
    public class CreateESDC_NETP_eventsProcess
    {
        private readonly IFoaeaConfigurationHelper Config;
        private readonly IRepositories DB;
        private readonly ClaimsPrincipal User;

        public CreateESDC_NETP_eventsProcess(IRepositories repositories, IFoaeaConfigurationHelper config, ClaimsPrincipal user)
        {
            Config = config;
            DB = repositories;
            User = user;
        }

        public async Task Run()
        {
            var prodAudit = DB.ProductionAuditTable;

            await prodAudit.Insert("Create ESDC(NETP) Events Process", $"Create ESDC(NETP) Events Process Started", "O");

            var tracingManager = new TracingManager(DB, Config, User);
            await tracingManager.CreateNETPevents();

            await prodAudit.Insert("Create ESDC(NETP) Events Process", $"Create ESDC(NETP) Events Process Completed", "O");
        }

    }
}
