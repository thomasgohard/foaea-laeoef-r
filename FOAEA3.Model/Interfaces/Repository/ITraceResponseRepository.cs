using FOAEA3.Model.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface ITraceResponseRepository : IMessageList
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<DataList<TraceResponseData>> GetTraceResponseForApplicationAsync(string applEnfSrvCd, string applCtrlCd, bool checkCycle = false);
        Task InsertBulkDataAsync(List<TraceResponseData> responseData);
        Task DeleteCancelledApplicationTraceResponseDataAsync(string applEnfSrvCd, string applCtrlCd, string enfSrvCd);
        Task MarkResponsesAsViewedAsync(string enfService);
    }
}
