using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface IApplicationEventAPIBroker
    {
        Task<List<ApplicationEventData>> GetRequestedSINEventDataForFileAsync(string fileName);
        Task<List<ApplicationEventDetailData>> GetRequestedSINEventDetailDataForFileAsync(string fileName);
        Task<List<SinInboundToApplData>> GetLatestSinEventDataSummaryAsync();
        Task SaveEventAsync(ApplicationEventData eventData);
        Task SaveEventDetailAsync(ApplicationEventDetailData activeTraceEventDetail);
        Task UpdateOutboundEventDetailAsync(string actvSt_Cd, int appLiSt_Cd, string enfSrv_Cd, string newFilePath, List<int> eventIds);
    }
}
