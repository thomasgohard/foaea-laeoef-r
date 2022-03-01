using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces
{
    public interface ITracingEventAPIBroker
    {
        void CloseNETPTraceEvents();
        List<ApplicationEventData> GetRequestedTRCINEvents(string enfSrvCd, string fileCycle);
        List<ApplicationEventDetailData> GetActiveTracingEventDetails(string enfSrvCd, string fileCycle);
    }
}
