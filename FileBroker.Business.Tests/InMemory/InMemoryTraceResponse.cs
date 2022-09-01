using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileBroker.Business.Tests.InMemory
{
    public class InMemoryTraceResponse : ITraceResponseRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public MessageDataList Messages => throw new NotImplementedException();

        public Task DeleteCancelledApplicationTraceResponseDataAsync(string applEnfSrvCd, string applCtrlCd, string enfSrvCd)
        {
            throw new NotImplementedException();
        }

        public Task<DataList<TraceResponseData>> GetTraceResponseForApplicationAsync(string applEnfSrvCd, string applCtrlCd, bool checkCycle = false)
        {
            throw new NotImplementedException();
        }

        public Task InsertBulkDataAsync(List<TraceResponseData> responseData)
        {
            throw new NotImplementedException();
        }

        public Task MarkResponsesAsViewedAsync(string enfService)
        {
            throw new NotImplementedException();
        }
    }
}
