using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileBroker.Business.Tests.InMemory
{
    public class InMemoryApplicationEventAPIBroker : IApplicationEventAPIBroker
    {
        public IAPIBrokerHelper ApiHelper => throw new NotImplementedException();

        public string Token { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Task<List<ApplicationEventData>> GetEvents(string appl_EnfSrvCd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task<List<SinInboundToApplData>> GetLatestSinEventDataSummary()
        {
            throw new NotImplementedException();
        }

        public Task<List<ApplicationEventData>> GetRequestedSINEventDataForFile(string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<List<ApplicationEventDetailData>> GetRequestedSINEventDetailDataForFile(string fileName)
        {
            throw new NotImplementedException();
        }

        public Task SaveEvent(ApplicationEventData eventData)
        {
            throw new NotImplementedException();
        }

        public Task SaveEventDetail(ApplicationEventDetailData activeTraceEventDetail)
        {
            throw new NotImplementedException();
        }

        public Task UpdateOutboundEventDetail(string actvSt_Cd, int appLiSt_Cd, string enfSrv_Cd, string newFilePath, List<int> eventIds)
        {
            throw new NotImplementedException();
        }
    }
}
