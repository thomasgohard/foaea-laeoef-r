namespace FOAEA3.Model
{
    public class SinInboundToApplData
    {
        public int Event_Id { get; set; }
        public string Appl_EnfSrv_Cd { get; set; }
        public string Appl_CtrlCd { get; set; }
        public int Tot_Childs { get; set; }
        public int Tot_Closed { get; set; }
        public int Tot_Invalid { get; set; }
        public string Subm_SubmCd { get; set; }
        public string Appl_Dbtr_Cnfrmd_SIN { get; set; }
        public string Appl_Dbtr_RtrndBySrc_SIN { get; set; }
        public short AppLiSt_Cd { get; set; }
        public string ActvSt_Cd { get; set; }
        public string SVR_SIN { get; set; }
        public short? ValStat_Cd { get; set; }
    }
}
