using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IFinancialRepository
    {
        Task<List<CR_PADReventData>> GetActiveCR_PADReventsAsync(string enfSrv);
        Task CloseCR_PADReventsAsync(string batchId, string enfSrv);
        Task<List<BlockFundData>> GetBlockFundsData(string enfSrv);
        Task<List<IFMSdata>> GetIFMSdataAsync(string batchId);
        Task CloseControlBatchAsync(string batchId);
    }
}
