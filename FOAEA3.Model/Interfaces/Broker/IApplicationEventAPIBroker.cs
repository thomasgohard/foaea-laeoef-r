using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface IApplicationEventAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task<List<ApplicationEventData>> GetEvents(string appl_EnfSrvCd, string appl_CtrlCd);
        Task<List<ApplicationEventData>> GetRequestedSINEventDataForFile(string fileName);
        Task<List<ApplicationEventDetailData>> GetRequestedSINEventDetailDataForFile(string fileName);
        Task<List<SinInboundToApplData>> GetLatestSinEventDataSummary();
        Task SaveEvent(ApplicationEventData eventData);
        Task SaveEventDetail(ApplicationEventDetailData activeTraceEventDetail);
        Task UpdateOutboundEventDetail(string actvSt_Cd, int appLiSt_Cd, string enfSrv_Cd, string newFilePath, List<int> eventIds);
    }
}
