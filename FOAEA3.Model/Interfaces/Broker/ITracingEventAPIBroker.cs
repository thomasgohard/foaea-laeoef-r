using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface ITracingEventAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task CloseNETPTraceEventsAsync();
        Task<List<ApplicationEventData>> GetRequestedTRCINEventsAsync(string enfSrvCd, string fileCycle);
        Task<List<ApplicationEventDetailData>> GetActiveTracingEventDetailsAsync(string enfSrvCd, string fileCycle);
    }
}
