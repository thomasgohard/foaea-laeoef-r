using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Business.Areas.Financials
{
    public class FinancialEventManager
    {
        private IRepositories DB { get; }
        private IRepositories_Finance DBfinance { get; }
        private IFoaeaConfigurationHelper Config { get; }

        public FinancialEventManager(IRepositories repositories, IRepositories_Finance repositoriesFinance,
                                         IFoaeaConfigurationHelper config)
        {
            DB = repositories;
            DBfinance = repositoriesFinance;
            Config = config;
        }

        public async Task<List<CR_PADReventData>> GetActiveCR_PADReventsAsync(string enfSrv)
        {
            return await DBfinance.PADRrepository.GetActiveCR_PADReventsAsync(enfSrv);
        }

        public async Task<List<IFMSdata>> GetIFMSdataAsync(string batchId)
        {
            return await DBfinance.PADRrepository.GetIFMSdataAsync(batchId);
        }
    }
}
