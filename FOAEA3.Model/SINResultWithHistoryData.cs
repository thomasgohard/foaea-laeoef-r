using System;

namespace FOAEA3.Model
{
    public class SINResultWithHistoryData
    {
        public string Appl_EnfSrv_Cd { get; set; }
        public string Subm_SubmCd { get; set; }
        public string Appl_CtrlCd { get; set; }
        public string Appl_Source_RfrNr { get; set; }
        public DateTime SVR_TimeStamp { get; set; }
        public string SVR_DOB_TolCd { get; set; }
        public string SVR_GvnNme_TolCd { get; set; }
        public string SVR_MddlNme_TolCd { get; set; }
        public string SVR_SurNme_TolCd { get; set; }
        public string SVR_MotherNme_TolCd { get; set; }
        public string SVR_Gendr_TolCd { get; set; }
        public string SVR_TolCd { get; set; }
        public short ValStat_Cd { get; set; }
        public string ActvSt_Cd { get; set; }
        public DateTime? Appl_Dbtr_Brth_Dte { get; set; }
        public string Appl_Dbtr_FrstNme { get; set; }
        public string Appl_Dbtr_MddleNme { get; set; }
        public string Appl_Dbtr_SurNme { get; set; }
        public string Appl_Dbtr_Parent_SurNme { get; set; }
        public string Appl_Dbtr_Gendr_Cd { get; set; }
        public string Appl_Dbtr_Entrd_SIN { get; set; }
    }
}
