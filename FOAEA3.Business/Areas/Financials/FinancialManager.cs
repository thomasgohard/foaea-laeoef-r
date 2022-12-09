using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Business.Areas.Financials
{
    public class FinancialManager
    {
        private IRepositories DB { get; }
        private IRepositories_Finance DBfinance { get; }

        public FinancialManager(IRepositories repositories, IRepositories_Finance repositoriesFinance)
        {
            DB = repositories;
            DBfinance = repositoriesFinance;
        }

        public async Task<List<CR_PADReventData>> GetActiveCR_PADReventsAsync(string enfSrv)
        {
            return await DBfinance.FinancialRepository.GetActiveCR_PADReventsAsync(enfSrv);
        }

        public async Task CloseCR_PADReventsAsync(string batchId, string enfSrv)
        {
            await DBfinance.FinancialRepository.CloseCR_PADReventsAsync(batchId, enfSrv);
        }

        public async Task<List<BlockFundData>> GetBlockFundsData(string enfSrv)
        {
            return await DBfinance.FinancialRepository.GetBlockFundsData(enfSrv);
        }

        public async Task<List<IFMSdata>> GetIFMSdataAsync(string batchId)
        {
            return await DBfinance.FinancialRepository.GetIFMSdataAsync(batchId);
        }

        public async Task CloseControlBatchAsync(string batchId)
        {
            await DBfinance.FinancialRepository.CloseControlBatchAsync(batchId);
        }

        public async Task<DateTime> GetDateLastUIBatchLoaded()
        {
            return await DB.InterceptionTable.GetDateLastUIBatchLoaded();
        }

    }
}
