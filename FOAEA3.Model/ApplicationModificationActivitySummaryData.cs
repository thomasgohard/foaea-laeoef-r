using FOAEA3.Model.Enums;
using System;

namespace FOAEA3.Model
{
    public class ApplicationModificationActivitySummaryData
    {
        public string Appl_EnfSrv_Cd { get; set; }
        public string Appl_CtrlCd { get; set; }
        public string AppCtgy_Cd { get; set; }
        public ApplicationState AppLiSt_Cd { get; set; }
        public DateTime Appl_Create_Dte { get; set; }
        public string Appl_Create_Usr { get; set; }
        public DateTime? Appl_LastUpdate_Dte { get; set; }
        public string Appl_LastUpdate_Usr { get; set; }
    }
}
