using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Model.Interfaces
{
    public interface ISINResultRepository : IMessageList
    {
        string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        DataList<SINResultData> GetSINResults(string applEnfSrvCd, string applCtrlCd);
        DataList<SINResultWithHistoryData> GetSINResultsWithHistory(string applEnfSrvCd, string applCtrlCd);
        List<SINOutgoingFederalData> GetFederalSINOutgoingData(int maxRecords, string activeState, ApplicationState lifeState,
                                                               string enfServiceCode);
        void CreateSINResults(SINResultData resultData);
        void InsertBulkData(List<SINResultData> responseData);
    }
}
