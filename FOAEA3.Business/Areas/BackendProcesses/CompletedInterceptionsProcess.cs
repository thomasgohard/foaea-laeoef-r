using FOAEA3.Business.Areas.Application;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Repository;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FOAEA3.Business.BackendProcesses
{
    public class CompletedInterceptionsProcess
    {
        private readonly IRepositories DB;
        private readonly IRepositories_Finance DBfinance;
        private readonly IFoaeaConfigurationHelper Config;
        private readonly ClaimsPrincipal User;

        public CompletedInterceptionsProcess(IRepositories repositories, IRepositories_Finance repositoriesFinance,
                                             IFoaeaConfigurationHelper config, ClaimsPrincipal user)
        {
            DB = repositories;
            DBfinance = repositoriesFinance;
            Config = config;
            User = user;
        }

        public async Task Run()
        {
            var prodAudit = DB.ProductionAuditTable;

            await prodAudit.Insert("Completed I01 Process", "Completed I01 Process Started", "O");

            var applTerminated = await DB.InterceptionTable.GetTerminatedI01();

            foreach (var appl in applTerminated)
            {
                var manager = new InterceptionManager((InterceptionApplicationData)appl, DB, DBfinance, Config, User);
                await manager.CompleteApplication();
            }

            await prodAudit.Insert("Completed I01 Process", "Completed I01 Process Completed", "O");
        }
    }
}
