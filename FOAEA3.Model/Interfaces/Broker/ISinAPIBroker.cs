using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces
{
    public interface ISinAPIBroker
    {
        void InsertBulkData(List<SINResultData> resultData);
        List<SINOutgoingFederalData> GetOutgoingFederalSins(int maxRecords, string activeState, int lifeState, string enfServiceCode);
    }
}
