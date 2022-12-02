using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface IFinancialEventAPIBroker
    {
        Task<List<CR_PADReventData>> GetActiveCR_PADReventsAsync(string enfSrv);
        Task<List<IFMSdata>> GetIFMSasync(string batchId);
    }
}
