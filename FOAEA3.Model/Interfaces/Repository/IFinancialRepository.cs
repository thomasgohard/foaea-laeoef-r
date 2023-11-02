using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IFinancialRepository
    {
        Task<List<CR_PADReventData>> GetActiveCR_PADRevents(string enfSrv);
        Task CloseCR_PADRevents(string batchId, string enfSrv);
        Task<List<BlockFundData>> GetBlockFundsData(string enfSrv);
        Task<List<DivertFundData>> GetDivertFundsData(string enfSrv, string batchId);
        Task<List<IFMSdata>> GetIFMSdata(string batchId);
    }
}
