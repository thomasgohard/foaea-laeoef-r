namespace FOAEA3.Model
{
    public record TracingOutgoingFederalData
    (
         int Event_dtl_Id,
         int Event_Reas_Cd,
         string Event_Reas_Text,
         string ActvSt_Cd,
         string Recordtype,
         string Appl_Dbtr_Cnfrmd_SIN, // Val_1
         string Appl_EnfSrv_Cd,       // Val_2
         string Appl_CtrlCd,          // Val_3
         int ReturnType               // Val_4
    );
}
