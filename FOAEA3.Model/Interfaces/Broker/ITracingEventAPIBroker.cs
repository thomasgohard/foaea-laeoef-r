using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface ITracingEventAPIBroker
    {
        Task CloseNETPTraceEventsAsync();
        Task<List<ApplicationEventData>> GetRequestedTRCINEventsAsync(string enfSrvCd, string fileCycle);
        Task<List<ApplicationEventDetailData>> GetActiveTracingEventDetailsAsync(string enfSrvCd, string fileCycle);
    }
}
