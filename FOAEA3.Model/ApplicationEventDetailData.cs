using FOAEA3.Model.Enums;
using System;
using System.Diagnostics;

namespace FOAEA3.Model
{
    [DebuggerDisplay("{Event_Id}: {EnfSrv_Cd} {Event_Reas_Text} [{Queue}]: {ActvSt_Cd}")]
    public class ApplicationEventDetailData
    {
        public EventQueue Queue { get; set; }
        public int Event_dtl_Id { get; set; }
        public string EnfSrv_Cd { get; set; }
        public int? Event_Id { get; set; }
        public DateTime Event_TimeStamp { get; set; }
        public DateTime? Event_Compl_Dte { get; set; }
        public EventCode? Event_Reas_Cd { get; set; }
        public string Event_Reas_Text { get; set; }
        public string Event_Priority_Ind { get; set; }
        public DateTime? Event_Effctv_Dte { get; set; }
        public short AppLiSt_Cd { get; set; }
        public string ActvSt_Cd { get; set; }
    }
}
