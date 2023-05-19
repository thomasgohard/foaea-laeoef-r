using System;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IActiveSummonsRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public Task<ActiveSummonsCoreData> GetActiveSummonsCoreAsync(DateTime payableDate, string appl_EnfSrv_Cd, string appl_CtrlCd);
        public Task<ActiveSummonsData> GetActiveSummonsDataAsync(DateTime payableDate, string appl_CtrlCd, string appl_EnfSrv_Cd, bool isVariation = false);
        public Task<DateTime> GetLegalDateAsync(string appl_CtrlCd, string appl_EnfSrv_Cd);
    }
}
