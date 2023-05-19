using System;

namespace FOAEA3.Model
{
    public class LicenceDenialOutgoingProvincialData
    {
        public int Event_dtl_Id { get; set; }
        public int Event_Reas_Cd { get; set; }
        public int Event_Reas_Text { get; set; }
        public string ActvSt_Cd { get; set; }
        public string Recordtype { get; set; }
        public string Appl_EnfSrv_Cd { get; set; }       // Val_1 
        public string Subm_SubmCd { get; set; }          // Val_2
        public string Appl_CtrlCd { get; set; }          // Val_3
        public string Appl_Source_RfrNr { get; set; }    // Val_4
        public string Subm_Recpt_SubmCd { get; set; }    // Val_5
        public DateTime LicRsp_Rcpt_Dte { get; set; }    // Val_6
        public string LicRsp_SeqNr { get; set; }         // Val_7
        public short RqstStat_Cd { get; set; }           // Val_8
        public string EnfSrv_Cd { get; set; }            // Val_9
    }
}
