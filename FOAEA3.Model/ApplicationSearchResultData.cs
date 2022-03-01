using FOAEA3.Model.Enums;
using System;
using System.Diagnostics;

namespace FOAEA3.Model
{
    [DebuggerDisplay("{Appl_EnfSrv_Cd}-{Appl_CtrlCd} [{AppCtgy_Cd} at {AppLiSt_Cd}]")]
    public class ApplicationSearchResultData
    {
        public string AppCtgy_Cd { get; set; }
        public string Appl_EnfSrv_Cd { get; set; }
        public string Subm_SubmCd { get; set; }
        public string Appl_CtrlCd { get; set; }
        public string Subm_Recpt_SubmCd { get; set; }
        public DateTime Appl_Rcptfrm_Dte { get; set; }
        public DateTime? Appl_Expiry_Dte { get; set; }
        public string Appl_Dbtr_SurNme { get; set; }
        public string Appl_Source_RfrNr { get; set; }
        public string Appl_JusticeNr { get; set; }
        public string ActvSt_Cd { get; set; }
        public ApplicationState AppLiSt_Cd { get; set; }
    }
}
