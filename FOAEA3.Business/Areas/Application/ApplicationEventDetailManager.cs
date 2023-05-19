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

        public async Task<List<ApplicationEventDetailData>> GetApplicationEventDetailsForQueueAsync(EventQueue queue)
        {
            return await EventDetailDB.GetApplicationEventDetailsAsync(Application.Appl_EnfSrv_Cd, Application.Appl_CtrlCd, queue);
        }

        public async Task<bool> SaveEventDetailsAsync()
        {
            return await EventDetailDB.SaveEventDetailsAsync(EventDetails);
        }

        public async Task<bool> SaveEventDetailAsync(ApplicationEventDetailData eventDetailData)
        {
            return await EventDetailDB.SaveEventDetailAsync(eventDetailData);
        }

        public async Task UpdateOutboundEventDetailAsync(string activeState, string applicationState, string enfSrvCode,
                                              string writtenFile, List<int> eventIds)
        {
            await EventDetailDB.UpdateOutboundEventDetailAsync(activeState, applicationState, enfSrvCode, writtenFile, eventIds);
        }

        public async Task<DataList<ApplicationEventDetailData>> GetRequestedSINEventDetailDataForFileAsync(string enfSrv_Cd, string fileName)
        {
            return await EventDetailDB.GetRequestedSINEventDetailDataForFileAsync(enfSrv_Cd, fileName);
        }

        public async Task<List<ApplicationEventDetailData>> GetActiveTracingEventDetailsAsync(string enfSrv_Cd, string cycle)
        {
            return await EventDetailDB.GetActiveTracingEventDetailsAsync(enfSrv_Cd, cycle);
        }

        public async Task<List<ApplicationEventDetailData>> GetRequestedLICINLicenceDenialEventDetailsAsync(string enfSrv_Cd, string appl_EnfSrv_Cd,
                                                                       string appl_CtrlCd)
        {
            return await EventDetailDB.GetRequestedLICINLicenceDenialEventDetailsAsync(enfSrv_Cd, appl_EnfSrv_Cd, appl_CtrlCd);
        }
    }
}
