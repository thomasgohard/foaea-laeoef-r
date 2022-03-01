using FOAEA3.Model.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Model.Interfaces
{
    public interface ITraceResponseRepository : IMessageList
    {
        string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        DataList<TraceResponseData> GetTraceResponseForApplication(string applEnfSrvCd, string applCtrlCd, bool checkCycle = false);
        void InsertBulkData(List<TraceResponseData> responseData);
        void DeleteCancelledApplicationTraceResponseData(string applEnfSrvCd, string applCtrlCd, string enfSrvCd);
        void MarkResponsesAsViewed(string enfService);
    }
}
