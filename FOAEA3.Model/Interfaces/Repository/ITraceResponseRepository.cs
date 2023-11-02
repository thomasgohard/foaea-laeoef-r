using FOAEA3.Model.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface ITraceResponseRepository : IMessageList
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<DataList<TraceResponseData>> GetTraceResponseForApplication(string applEnfSrvCd, string applCtrlCd, bool checkCycle = false);
        Task<List<CraFieldData>> GetCraFields();
        Task<List<CraFormData>> GetCraForms();

        Task InsertBulkData(List<TraceResponseData> responseData);

        Task<DataList<TraceFinancialResponseData>> GetTraceResponseFinancialsForApplication(string applEnfSrvCd, string applCtrlCd);
        Task<DataList<TraceFinancialResponseData>> GetActiveTraceResponseFinancialsForApplication(string applEnfSrvCd, string applCtrlCd);

        Task<DataList<TraceFinancialResponseDetailData>> GetTraceResponseFinancialDetails(int traceResponseFinancialId);
        Task<DataList<TraceFinancialResponseDetailValueData>> GetTraceResponseFinancialDetailValues(int traceResponseFinancialDetailId);
        Task<int> CreateTraceFinancialResponse(TraceFinancialResponseData data);
        Task<int> CreateTraceFinancialResponseDetail(TraceFinancialResponseDetailData data);
        Task<int> CreateTraceFinancialResponseDetailValue(TraceFinancialResponseDetailValueData data);
        Task UpdateTraceResponseFinancial(TraceFinancialResponseData data);

        Task DeleteCancelledApplicationTraceResponseData(string applEnfSrvCd, string applCtrlCd, string enfSrvCd);
        Task MarkResponsesAsViewed(string enfService);
    }
}
