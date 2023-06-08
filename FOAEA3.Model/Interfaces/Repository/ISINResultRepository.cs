using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface ISINResultRepository : IMessageList
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<DataList<SINResultData>> GetSINResults(string applEnfSrvCd, string applCtrlCd);
        Task<DataList<SINResultWithHistoryData>> GetSINResultsWithHistory(string applEnfSrvCd, string applCtrlCd);
        Task<List<SINOutgoingFederalData>> GetFederalSINOutgoingData(int maxRecords, string activeState, ApplicationState lifeState,
                                                                     string enfServiceCode);
        Task CreateSINResults(SINResultData resultData);
        Task InsertBulkData(List<SINResultData> responseData);
    }
}
