using FOAEA3.Business.Areas.Application;
using FOAEA3.Business.BackendProcesses;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Repository;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FOAEA3.Business.Areas.BackendProcesses
{
    public class UpdateFixedAmountRecalcDate
    {
        private readonly IRepositories DB;
        private readonly IRepositories_Finance DBfinance;
        private readonly IFoaeaConfigurationHelper Config;
        private readonly ClaimsPrincipal User;

        public UpdateFixedAmountRecalcDate(IRepositories repositories, IRepositories_Finance repositoriesFinance,
                              IFoaeaConfigurationHelper config, ClaimsPrincipal user)
        {
            DB = repositories;
            DBfinance = repositoriesFinance;
            Config = config;
            User = user;
        }

        public async Task RunAsync()
        {
            var prodAudit = DB.ProductionAuditTable;

            await prodAudit.InsertAsync("Update Fixed Amount Date", "Update Fixed Amount Recalc Date Started", "O");

            var interceptionManager = new InterceptionManager(DB, DBfinance, Config, User);
            var summSmryApplications = await interceptionManager.GetFixedAmountRecalcDateRecords();

            var amountOwedProcess = new AmountOwedProcess(DB, DBfinance);
            await amountOwedProcess.RunAsync(summSmryApplications);

            await prodAudit.InsertAsync("Update Fixed Amount Date", "Update Fixed Amount Recalc Date Completed", "O");
        }
    }
}
