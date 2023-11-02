using FOAEA3.Model.Enums;
using System;
using System.Diagnostics;

namespace FOAEA3.Model
{
    [DebuggerDisplay("{Event_Id}: {Appl_EnfSrv_Cd}-{Appl_CtrlCd} {Event_Reas_Cd} [{Queue}]: {ActvSt_Cd}")]
    public class ApplicationEventData
    {
        public EventQueue Queue { get; set; }

        public int Event_Id { get; set; }

        public string Appl_CtrlCd { get; set; }

        public string Subm_SubmCd { get; set; }

        public string Appl_EnfSrv_Cd { get; set; }

        public string Subm_Recpt_SubmCd { get; set; }

        public string Event_RecptSubm_ActvStCd { get; set; }

        public DateTime? Appl_Rcptfrm_Dte { get; set; }

        public string Subm_Update_SubmCd { get; set; }

        public DateTime Event_TimeStamp { get; set; }

        public DateTime? Event_Compl_Dte { get; set; }

        public EventCode? Event_Reas_Cd { get; set; }

        public string Event_Reas_Text { get; set; }

        public string Event_Priority_Ind { get; set; }

        public DateTime Event_Effctv_Dte { get; set; }

        public string ActvSt_Cd { get; set; }

        public ApplicationState AppLiSt_Cd { get; set; }

        public string AppCtgy_Cd { get; set; }
    }
}
