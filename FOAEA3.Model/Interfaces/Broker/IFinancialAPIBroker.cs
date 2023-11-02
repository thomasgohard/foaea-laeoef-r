using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface IFinancialAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task<List<CR_PADReventData>> GetActiveCR_PADRevents(string enfSrv);
        Task CloseCR_PADRevents(string batchId, string enfSrv);
        Task<List<BlockFundData>> GetBlockFunds(string enfSrv);
        Task<List<DivertFundData>> GetDivertFunds(string enfSrv, string batchId);
        Task<List<IFMSdata>> GetIFMSDataFromFoaea(string batchId);
    }
}
