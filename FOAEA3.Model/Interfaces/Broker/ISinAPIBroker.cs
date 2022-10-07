using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface ISinAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task InsertBulkDataAsync(List<SINResultData> resultData);
        Task<List<SINOutgoingFederalData>> GetOutgoingFederalSinsAsync(int maxRecords, string activeState, int lifeState, string enfServiceCode);
    }
}
