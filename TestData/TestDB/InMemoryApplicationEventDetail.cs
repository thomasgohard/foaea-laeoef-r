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

        public Task<List<ApplicationEventDetailData>> GetActiveTracingEventDetailsAsync(string enfSrv_Cd, string cycle)
        {
            throw new NotImplementedException();
        }

        public Task<List<ApplicationEventDetailData>> GetApplicationEventDetailsAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, EventQueue queue, string activeState = null)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationEventDetailData> GetEventSINDetailDataForEventIDAsync(int eventID)
        {
            throw new NotImplementedException();
        }

        public Task<List<ApplicationEventDetailData>> GetRequestedLICINLicenceDenialEventDetailsAsync(string enfSrv_Cd, string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task<DataList<ApplicationEventDetailData>> GetRequestedSINEventDetailDataForFileAsync(string enfSrv_Cd, string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveEventDetailAsync(ApplicationEventDetailData eventDetailData)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveEventDetailsAsync(List<ApplicationEventDetailData> eventDetailsData)
        {
            throw new NotImplementedException();
        }

        public Task UpdateOutboundEventDetailAsync(string activeState, string applicationState, string enfSrvCode, string writtenFile, List<int> eventIds)
        {
            throw new NotImplementedException();
        }
    }
}
