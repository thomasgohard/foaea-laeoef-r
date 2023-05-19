using FOAEA3.Model.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface ILicenceDenialResponseRepository : IMessageList
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<LicenceDenialResponseData> GetLastResponseDataAsync(string applEnfSrvCd, string applCtrlCd);
        //DataList<TraceResponseData> GetLicenceDenialResponseForApplication(string applEnfSrvCd, string applCtrlCd, bool checkCycle = false);
        Task InsertBulkDataAsync(List<LicenceDenialResponseData> responseData);
        Task MarkResponsesAsViewedAsync(string enfService);

    }
}
