using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IPADRRepository
    {
        Task<List<CR_PADReventData>> GetActiveCR_PADReventsAsync(string enfSrv);
        Task<List<IFMSdata>> GetIFMSdataAsync(string batchId);
    }
}
