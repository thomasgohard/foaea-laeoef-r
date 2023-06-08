using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ITracingEventAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task CloseNETPTraceEvents();
        Task<List<ApplicationEventData>> GetRequestedTRCINEvents(string enfSrvCd, string fileCycle);
        Task<List<ApplicationEventDetailData>> GetActiveTracingEventDetails(string enfSrvCd, string fileCycle);
    }
}
