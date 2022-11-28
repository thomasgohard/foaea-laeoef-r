using FOAEA3.Business.Areas.Application;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Repository;
using System.Threading.Tasks;

namespace FOAEA3.Business.Areas.BackendProcesses
{
    internal class AutoAcceptProcess
    {
        private readonly IFoaeaConfigurationHelper Config;
        private readonly IRepositories DB;
        private readonly IRepositories_Finance DBfinance;

        public AutoAcceptProcess(IRepositories repositories, IRepositories_Finance repositories_finance,
                                        IFoaeaConfigurationHelper config)
        {
            Config = config;
            DB = repositories;
            DBfinance = repositories_finance;
        }

        public async Task RunAsync()
        {
            var prodAudit = DB.ProductionAuditTable;

            await prodAudit.InsertAsync("Auto Accept Process", $"Auto Accept Process Started", "O");

            var interceptionManager = new InterceptionManager(DB, DBfinance, Config);

            foreach (string enfService in Config.AutoAccept)
                await interceptionManager.AutoAcceptVariationsAsync(enfService);

            await prodAudit.InsertAsync("Auto Accept Process", $"Auto Accept Process Completed", "O");
        }

        public async Task RunAsync(string enfService)
        {
            var prodAudit = DB.ProductionAuditTable;

            await prodAudit.InsertAsync("Auto Accept Process", $"Auto Accept Process for {enfService} Started", "O");

            if (Config.AutoAccept.Contains(enfService.ToUpper()))
            {
                var interceptionManager = new InterceptionManager(DB, DBfinance, Config);
                await interceptionManager.AutoAcceptVariationsAsync(enfService);
            }
            else
                await prodAudit.InsertAsync("Auto Accept Process", $"Error: {enfService} not in valid auto accept list", "O");

            await prodAudit.InsertAsync("Auto Accept Process", $"Auto Accept Process for {enfService} Completed", "O");
        }
    }
}
