using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model;
using System;
using System.Collections.Generic;
using FOAEA3.Model.Base;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    public class InMemoryApplicationEventDetail : IApplicationEventDetailRepository
    {
        public string CurrentSubmitter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string UserId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public async Task<List<ApplicationEventDetailData>> GetActiveTracingEventDetailsAsync(string enfSrv_Cd, string cycle)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<ApplicationEventDetailData>> GetApplicationEventDetailsAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, EventQueue queue, string activeState = null)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<ApplicationEventDetailData> GetEventSINDetailDataForEventIDAsync(int eventID)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<ApplicationEventDetailData>> GetRequestedLICINLicenceDenialEventDetailsAsync(string enfSrv_Cd, string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<DataList<ApplicationEventDetailData>> GetRequestedSINEventDetailDataForFileAsync(string enfSrv_Cd, string fileName)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<bool> SaveEventDetailAsync(ApplicationEventDetailData eventDetailData)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<bool> SaveEventDetailsAsync(List<ApplicationEventDetailData> eventDetailsData)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task UpdateOutboundEventDetailAsync(string activeState, string applicationState, string enfSrvCode, string writtenFile, List<int> eventIds)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }
    }
}
