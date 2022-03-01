using System;

namespace FOAEA3.Model
{
    public class SINChangeHistoryData
    {
        public int SINChangeHistoryId { get; set; }
        public string Appl_EnfSrv_Cd { get; set; }
        public string Appl_CtrlCd { get; set; }
        public string Appl_Dbtr_SurNme { get; set; }
        public string Appl_Dbtr_FrstNme { get; set; }
        public string Appl_Dbtr_MddleNme { get; set; }
        public string Appl_Dbtr_Cnfrmd_SIN { get; set; }
        public int? AppLiSt_Cd { get; set; }
        public DateTime? SINChangeHistoryDate { get; set; }
        public string SINChangeHistoryUser { get; set; }
        public string SINChangeHistoryComment { get; set; }
    }
}
