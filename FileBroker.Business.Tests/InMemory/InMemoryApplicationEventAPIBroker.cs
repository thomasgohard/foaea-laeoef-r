using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileBroker.Business.Tests.InMemory
{
    public class InMemoryApplicationEventAPIBroker : IApplicationEventAPIBroker
    {
        public async Task<List<SinInboundToApplData>> GetLatestSinEventDataSummaryAsync()
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<ApplicationEventData>> GetRequestedSINEventDataForFileAsync(string fileName)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<ApplicationEventDetailData>> GetRequestedSINEventDetailDataForFileAsync(string fileName)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task SaveEventAsync(ApplicationEventData eventData)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task SaveEventDetailAsync(ApplicationEventDetailData activeTraceEventDetail)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task UpdateOutboundEventDetailAsync(string actvSt_Cd, int appLiSt_Cd, string enfSrv_Cd, string newFilePath, List<int> eventIds)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }
    }
}
