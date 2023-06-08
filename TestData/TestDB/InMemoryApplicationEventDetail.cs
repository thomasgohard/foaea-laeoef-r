using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    public class InMemoryApplicationEventDetail : IApplicationEventDetailRepository
    {
        public string CurrentSubmitter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string UserId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Task<List<ApplicationEventDetailData>> GetActiveTracingEventDetails(string enfSrv_Cd, string cycle)
        {
            throw new NotImplementedException();
        }

        public Task<List<ApplicationEventDetailData>> GetApplicationEventDetails(string appl_EnfSrv_Cd, string appl_CtrlCd, EventQueue queue, string activeState = null)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationEventDetailData> GetEventSINDetailDataForEventID(int eventID)
        {
            throw new NotImplementedException();
        }

        public Task<List<ApplicationEventDetailData>> GetRequestedLICINLicenceDenialEventDetails(string enfSrv_Cd, string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task<DataList<ApplicationEventDetailData>> GetRequestedSINEventDetailDataForFile(string enfSrv_Cd, string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveEventDetail(ApplicationEventDetailData eventDetailData)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveEventDetails(List<ApplicationEventDetailData> eventDetailsData)
        {
            throw new NotImplementedException();
        }

        public Task UpdateOutboundEventDetail(string activeState, string applicationState, string enfSrvCode, string writtenFile, List<int> eventIds)
        {
            throw new NotImplementedException();
        }
    }
}
