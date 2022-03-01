using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Model.Interfaces
{
    public interface IActiveSummonsRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public ActiveSummonsCoreData GetActiveSummonsCore(DateTime payableDate, string appl_EnfSrv_Cd, string appl_CtrlCd);
        public ActiveSummonsData GetActiveSummonsData(DateTime payableDate, string appl_CtrlCd, string appl_EnfSrv_Cd, bool isVariation = false);
        public DateTime GetLegalDate(string appl_CtrlCd, string appl_EnfSrv_Cd);
    }
}
