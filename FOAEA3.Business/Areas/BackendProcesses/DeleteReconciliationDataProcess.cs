using FOAEA3.Business.Areas.Application;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Repository;
using System.Threading.Tasks;

namespace FOAEA3.Business.Areas.BackendProcesses
{
    public class DeleteReconciliationDataProcess
    {
        private readonly IFoaeaConfigurationHelper Config;
        private readonly IRepositories DB;
        private readonly IRepositories_Finance DBfinance;

        public DeleteReconciliationDataProcess(IRepositories repositories, IRepositories_Finance repositories_finance,
                                        IFoaeaConfigurationHelper config)
        {
            Config = config;
            DB = repositories;
            DBfinance = repositories_finance;
        }

        public async Task RunAsync()
        {
            var prodAudit = DB.ProductionAuditTable;

            await prodAudit.InsertAsync("Delete Reconciliation Process", $"Delete Reconciliation Process Started", "O");

            var interceptionManager = new InterceptionManager(DB, DBfinance, Config);

            await interceptionManager.MessageBrokerCRAReconciliation();

            await prodAudit.InsertAsync("Delete Reconciliation Process", $"Delete Reconciliation Process Completed", "O");
        }
    }
}
