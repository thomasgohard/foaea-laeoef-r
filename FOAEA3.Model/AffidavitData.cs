using System;

namespace FOAEA3.Model
{
    public class AffidavitData
    {
        public int Event_Id { get; set; }
        public string EnfSrv_Cd { get; set; }
        public string Appl_CtrlCd { get; set; }
        public string Subm_Affdvt_SubmCd { get; set; }
        public DateTime Affdvt_FileRecv_Dte { get; set; }
        public DateTime Affdvt_Sworn_Dte { get; set; }
        public string AppCtgy_Cd { get; set; }
        public string OriginalFileName { get; set; }
        public DateTime? Affidvt_Prcs_Dte { get; set; }
        public string Affidvt_ActvSt_Cd { get; set; }
    }
}
