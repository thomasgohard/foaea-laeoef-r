using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ISinAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task InsertBulkData(List<SINResultData> resultData);
        Task<List<SINOutgoingFederalData>> GetOutgoingFederalSins(int maxRecords, string activeState, int lifeState, string enfServiceCode);
    }
}
