using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;

namespace FileBroker.Business.Tests.InMemory
{
    public class InMemoryApplicationEventAPIBroker : IApplicationEventAPIBroker
    {
        public List<SinInboundToApplData> GetLatestSinEventDataSummary()
        {
            throw new NotImplementedException();
        }

        public List<ApplicationEventData> GetRequestedSINEventDataForFile(string fileName)
        {
            throw new NotImplementedException();
        }

        public List<ApplicationEventDetailData> GetRequestedSINEventDetailDataForFile(string fileName)
        {
            throw new NotImplementedException();
        }

        public void SaveEvent(ApplicationEventData eventData)
        {
            throw new NotImplementedException();
        }

        public void SaveEventDetail(ApplicationEventDetailData activeTraceEventDetail)
        {
            throw new NotImplementedException();
        }

        public void UpdateOutboundEventDetail(string actvSt_Cd, int appLiSt_Cd, string enfSrv_Cd, string newFilePath, List<int> eventIds)
        {
            throw new NotImplementedException();
        }
    }
}
