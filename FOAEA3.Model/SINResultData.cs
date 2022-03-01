using System;

namespace FOAEA3.Model
{
    public class SINResultData
    {
        public string Appl_EnfSrv_Cd { get; set; }
        public string Appl_CtrlCd { get; set; }
        public DateTime SVR_TimeStamp { get; set; }
        public string SVR_TolCd { get; set; }
        public string SVR_SIN { get; set; }
        public short? SVR_DOB_TolCd { get; set; }
        public short? SVR_GvnNme_TolCd { get; set; }
        public short? SVR_MddlNme_TolCd { get; set; }
        public short? SVR_SurNme_TolCd { get; set; }
        public short? SVR_MotherNme_TolCd { get; set; }
        public short? SVR_Gendr_TolCd { get; set; }
        public short ValStat_Cd { get; set; }
        public string ActvSt_Cd { get; set; }
    }
}
