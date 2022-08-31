using DBHelper;
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

        public async Task DeleteCancelledApplicationTraceResponseDataAsync(string applEnfSrvCd, string applCtrlCd, string enfSrvCd)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<DataList<TraceResponseData>> GetTraceResponseForApplicationAsync(string applEnfSrvCd, string applCtrlCd, bool checkCycle = false)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task InsertBulkDataAsync(List<TraceResponseData> responseData)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task MarkResponsesAsViewedAsync(string enfService)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }
    }
}
