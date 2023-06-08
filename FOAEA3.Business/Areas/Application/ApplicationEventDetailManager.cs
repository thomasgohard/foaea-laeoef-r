using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Business.Areas.Application
{
    internal class ApplicationEventDetailManager
    {
        ApplicationData Application { get; }
        public List<ApplicationEventDetailData> EventDetails { get; }
        IApplicationEventDetailRepository EventDetailDB { get; }

        public ApplicationEventDetailManager(ApplicationData applicationData, IRepositories repositories)
        {
            Application = applicationData;
            EventDetails = new List<ApplicationEventDetailData>();
            EventDetailDB = repositories.ApplicationEventDetailTable;
        }

        public async Task<List<ApplicationEventDetailData>> GetApplicationEventDetailsForQueue(EventQueue queue)
        {
            return await EventDetailDB.GetApplicationEventDetails(Application.Appl_EnfSrv_Cd, Application.Appl_CtrlCd, queue);
        }

        public async Task<bool> SaveEventDetails()
        {
            return await EventDetailDB.SaveEventDetails(EventDetails);
        }

        public async Task<bool> SaveEventDetail(ApplicationEventDetailData eventDetailData)
        {
            return await EventDetailDB.SaveEventDetail(eventDetailData);
        }

        public async Task UpdateOutboundEventDetail(string activeState, string applicationState, string enfSrvCode,
                                              string writtenFile, List<int> eventIds)
        {
            await EventDetailDB.UpdateOutboundEventDetail(activeState, applicationState, enfSrvCode, writtenFile, eventIds);
        }

        public async Task<DataList<ApplicationEventDetailData>> GetRequestedSINEventDetailDataForFile(string enfSrv_Cd, string fileName)
        {
            return await EventDetailDB.GetRequestedSINEventDetailDataForFile(enfSrv_Cd, fileName);
        }

        public async Task<List<ApplicationEventDetailData>> GetActiveTracingEventDetails(string enfSrv_Cd, string cycle)
        {
            return await EventDetailDB.GetActiveTracingEventDetails(enfSrv_Cd, cycle);
        }

        public async Task<List<ApplicationEventDetailData>> GetRequestedLICINLicenceDenialEventDetails(string enfSrv_Cd, string appl_EnfSrv_Cd,
                                                                       string appl_CtrlCd)
        {
            return await EventDetailDB.GetRequestedLICINLicenceDenialEventDetails(enfSrv_Cd, appl_EnfSrv_Cd, appl_CtrlCd);
        }
    }
}
