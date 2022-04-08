using FOAEA3.Model.Base;
using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface ILicenceDenialResponseRepository : IMessageList
    {
        string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        LicenceDenialResponseData GetLastResponseData(string applEnfSrvCd, string applCtrlCd);
        //DataList<TraceResponseData> GetLicenceDenialResponseForApplication(string applEnfSrvCd, string applCtrlCd, bool checkCycle = false);
        void InsertBulkData(List<LicenceDenialResponseData> responseData);
        void MarkResponsesAsViewed(string enfService);

    }
}
