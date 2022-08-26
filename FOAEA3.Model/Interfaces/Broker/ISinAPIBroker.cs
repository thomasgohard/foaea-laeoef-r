using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface ISinAPIBroker
    {
        Task InsertBulkDataAsync(List<SINResultData> resultData);
        Task<List<SINOutgoingFederalData>> GetOutgoingFederalSinsAsync(int maxRecords, string activeState, int lifeState, string enfServiceCode);
    }
}
