using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface ISINResultRepository : IMessageList
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<DataList<SINResultData>> GetSINResultsAsync(string applEnfSrvCd, string applCtrlCd);
        Task<DataList<SINResultWithHistoryData>> GetSINResultsWithHistoryAsync(string applEnfSrvCd, string applCtrlCd);
        Task<List<SINOutgoingFederalData>> GetFederalSINOutgoingDataAsync(int maxRecords, string activeState, ApplicationState lifeState,
                                                               string enfServiceCode);
        Task CreateSINResultsAsync(SINResultData resultData);
        Task InsertBulkDataAsync(List<SINResultData> responseData);
    }
}
