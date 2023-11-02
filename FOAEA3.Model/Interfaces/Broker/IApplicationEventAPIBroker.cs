using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface IApplicationEventAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task<ApplicationEventsList> GetEvents(string appl_EnfSrvCd, string appl_CtrlCd);
        Task<ApplicationEventsList> GetRequestedSINEventDataForFile(string fileName);
        Task<ApplicationEventDetailsList> GetRequestedSINEventDetailDataForFile(string fileName);
        Task<List<SinInboundToApplData>> GetLatestSinEventDataSummary();
        Task SaveEvent(ApplicationEventData eventData);
        Task SaveEventDetail(ApplicationEventDetailData activeTraceEventDetail);
        Task SaveEventDetails(ApplicationEventDetailsList eventDetails);
        Task UpdateOutboundEventDetail(string actvSt_Cd, int appLiSt_Cd, string enfSrv_Cd, string newFilePath, List<int> eventIds);
    }
}
