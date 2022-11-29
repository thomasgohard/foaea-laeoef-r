using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Repository;
using System.Threading.Tasks;

namespace FOAEA3.Business.BackendProcesses
{
    public class NightlyProcess
    {
        private readonly IRepositories DB;
        private readonly IRepositories_Finance DBfinance;
        private readonly IFoaeaConfigurationHelper Config;

        public NightlyProcess(IRepositories repositories, IRepositories_Finance repositoriesFinance,
                              IFoaeaConfigurationHelper config)
        {
            DB = repositories;
            DBfinance = repositoriesFinance;
            Config = config;
        }

        public async Task RunAsync()
        {
            var prodAudit = DB.ProductionAuditTable;

            await prodAudit.InsertAsync("Nightly Process", "Nightly Process Started", "O");

            var completeI01process = new CompletedInterceptionsProcess(DB, DBfinance, Config);
            await completeI01process.RunAsync();

            var amountOwedProcess = new AmountOwedProcess(DB, DBfinance);
            await amountOwedProcess.RunAsync();

            var divertFundProcess = new DivertFundsProcess(DB, DBfinance);
            await divertFundProcess.RunAsync();

            ChequeReqProcess.Run();

            await prodAudit.InsertAsync("Nightly Process", "Nightly Process Completed", "O");
        }
    }
}
