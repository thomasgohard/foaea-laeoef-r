using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using System.Collections.Generic;

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
            EventDetailDB = repositories.ApplicationEventDetailRepository;
        }

        public List<ApplicationEventDetailData> GetApplicationEventDetailsForQueue(EventQueue queue)
        {
            return EventDetailDB.GetApplicationEventDetails(Application.Appl_EnfSrv_Cd, Application.Appl_CtrlCd, queue);
        }

        public bool SaveEventDetails()
        {
            return EventDetailDB.SaveEventDetails(EventDetails);
        }

        public bool SaveEventDetail(ApplicationEventDetailData eventDetailData)
        {
            return EventDetailDB.SaveEventDetail(eventDetailData);
        }

        public void UpdateOutboundEventDetail(string activeState, string applicationState, string enfSrvCode,
                                              string writtenFile, List<int> eventIds)
        {
            EventDetailDB.UpdateOutboundEventDetail(activeState, applicationState, enfSrvCode, writtenFile, eventIds);
        }

        public DataList<ApplicationEventDetailData> GetRequestedSINEventDetailDataForFile(string enfSrv_Cd, string fileName)
        {
            return EventDetailDB.GetRequestedSINEventDetailDataForFile(enfSrv_Cd, fileName);
        }

        public List<ApplicationEventDetailData> GetActiveTracingEventDetails(string enfSrv_Cd, string cycle)
        {
            return EventDetailDB.GetActiveTracingEventDetails(enfSrv_Cd, cycle);
        }
    }
}
