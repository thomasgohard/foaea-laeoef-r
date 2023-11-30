using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces.Repository;
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

        public Task<int> CreateTraceFinancialResponse(TraceFinancialResponseData data)
        {
            throw new NotImplementedException();
        }

        public Task<int> CreateTraceFinancialResponseDetail(TraceFinancialResponseDetailData data)
        {
            throw new NotImplementedException();
        }

        public Task<int> CreateTraceFinancialResponseDetailValue(TraceFinancialResponseDetailValueData data)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCancelledApplicationTraceResponseData(string applEnfSrvCd, string applCtrlCd, string enfSrvCd)
        {
            throw new NotImplementedException();
        }

        public Task<DataList<TraceFinancialResponseData>> GetActiveTraceResponseFinancialsForApplication(string applEnfSrvCd, string applCtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task<List<CraFieldData>> GetCraFields()
        {
            throw new NotImplementedException();
        }

        public Task<List<CraFormData>> GetCraForms()
        {
            throw new NotImplementedException();
        }

        public Task<DataList<TraceFinancialResponseDetailData>> GetTraceResponseFinancialDetails(int traceResponseFinancialId)
        {
            throw new NotImplementedException();
        }

        public Task<DataList<TraceFinancialResponseDetailValueData>> GetTraceResponseFinancialDetailValues(int traceResponseFinancialDetailId)
        {
            throw new NotImplementedException();
        }

        public Task<DataList<TraceFinancialResponseData>> GetTraceResponseFinancialsForApplication(string applEnfSrvCd, string applCtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task<DataList<TraceResponseData>> GetTraceResponseForApplication(string applEnfSrvCd, string applCtrlCd, bool checkCycle = false)
        {
            throw new NotImplementedException();
        }

        public Task InsertBulkData(List<TraceResponseData> responseData)
        {
            throw new NotImplementedException();
        }

        public Task MarkResponsesAsViewed(string recipientSubmCd)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTraceResponseFinancial(TraceFinancialResponseData data)
        {
            throw new NotImplementedException();
        }
    }
}
