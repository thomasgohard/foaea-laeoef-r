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

        Task<bool> SaveEventDetailAsync(ApplicationEventDetailData eventDetailData);
        Task<bool> SaveEventDetailsAsync(List<ApplicationEventDetailData> eventDetailsData);
        Task UpdateOutboundEventDetailAsync(string activeState, string applicationState, string enfSrvCode, string writtenFile, List<int> eventIds);

        Task<List<ApplicationEventDetailData>> GetActiveTracingEventDetailsAsync(string enfSrv_Cd, string cycle);

        Task<List<ApplicationEventDetailData>> GetRequestedLICINLicenceDenialEventDetailsAsync(string enfSrv_Cd, string appl_EnfSrv_Cd,
                                                                string appl_CtrlCd);

        Task<DataList<ApplicationEventDetailData>> GetRequestedSINEventDetailDataForFileAsync(string enfSrv_Cd, string fileName);
        Task<ApplicationEventDetailData> GetEventSINDetailDataForEventIDAsync(int eventID);
        Task<List<ApplicationEventDetailData>> GetApplicationEventDetailsAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, EventQueue queue, string activeState = null);
    }
}
