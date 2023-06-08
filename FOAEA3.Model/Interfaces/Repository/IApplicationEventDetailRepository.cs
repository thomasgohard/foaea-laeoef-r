using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IApplicationEventDetailRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<bool> SaveEventDetail(ApplicationEventDetailData eventDetailData);
        Task<bool> SaveEventDetails(List<ApplicationEventDetailData> eventDetailsData);
        Task UpdateOutboundEventDetail(string activeState, string applicationState, string enfSrvCode, string writtenFile, List<int> eventIds);

        Task<List<ApplicationEventDetailData>> GetActiveTracingEventDetails(string enfSrv_Cd, string cycle);

        Task<List<ApplicationEventDetailData>> GetRequestedLICINLicenceDenialEventDetails(string enfSrv_Cd, string appl_EnfSrv_Cd,
                                                                string appl_CtrlCd);

        Task<DataList<ApplicationEventDetailData>> GetRequestedSINEventDetailDataForFile(string enfSrv_Cd, string fileName);
        Task<ApplicationEventDetailData> GetEventSINDetailDataForEventID(int eventID);
        Task<List<ApplicationEventDetailData>> GetApplicationEventDetails(string appl_EnfSrv_Cd, string appl_CtrlCd, EventQueue queue, string activeState = null);
    }
}
