using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces
{
    public interface IApplicationEventAPIBroker
    {
        List<ApplicationEventData> GetRequestedSINEventDataForFile(string fileName);
        List<ApplicationEventDetailData> GetRequestedSINEventDetailDataForFile(string fileName);
        List<SinInboundToApplData> GetLatestSinEventDataSummary();
        void SaveEvent(ApplicationEventData eventData);
        void SaveEventDetail(ApplicationEventDetailData activeTraceEventDetail);
        void UpdateOutboundEventDetail(string actvSt_Cd, int appLiSt_Cd, string enfSrv_Cd, string newFilePath, List<int> eventIds);
    }
}
