namespace FOAEA3.Model
{
    public record TracingOutgoingFederalData
    {
        public int Event_dtl_Id { get; init; }
        public int Event_Reas_Cd { get; init; }
        public string Event_Reas_Text { get; init; }
        public string ActvSt_Cd { get; init; }
        public string Recordtype { get; init; }
        public string Appl_Dbtr_Cnfrmd_SIN { get; init; } // Val_1
        public string Appl_EnfSrv_Cd { get; init; }       // Val_2
        public string Appl_CtrlCd { get; init; }          // Val_3
        public int ReturnType { get; init; }              // Val_4
    }
}
