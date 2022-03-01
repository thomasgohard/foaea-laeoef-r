namespace FOAEA3.Model
{
    public class TracingOutgoingFederalData
    {
        public int Event_dtl_Id { get; set; }
        public int Event_Reas_Cd { get; set; }
        public string Event_Reas_Text { get; set; }
        public string ActvSt_Cd { get; set; }
        public string Recordtype { get; set; }
        public string Appl_Dbtr_Cnfrmd_SIN { get; set; } // Val_1
        public string Appl_EnfSrv_Cd { get; set; }       // Val_2
        public string Appl_CtrlCd { get; set; }          // Val_3
        public int ReturnType { get; set; }              // Val_4
    }
}
