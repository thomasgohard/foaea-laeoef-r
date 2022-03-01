using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces
{
    public interface IApplicationEventDetailRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        bool SaveEventDetail(ApplicationEventDetailData eventDetailData);
        bool SaveEventDetails(List<ApplicationEventDetailData> eventDetailsData);
        void UpdateOutboundEventDetail(string activeState, string applicationState, string enfSrvCode, string writtenFile, List<int> eventIds);

        List<ApplicationEventDetailData> GetActiveTracingEventDetails(string enfSrv_Cd, string cycle);

        DataList<ApplicationEventDetailData> GetRequestedSINEventDetailDataForFile(string enfSrv_Cd, string fileName);
        public ApplicationEventDetailData GetEventSINDetailDataForEventID(int eventID);
        List<ApplicationEventDetailData> GetApplicationEventDetails(string appl_EnfSrv_Cd, string appl_CtrlCd, EventQueue queue, string activeState = null);
    }
}
