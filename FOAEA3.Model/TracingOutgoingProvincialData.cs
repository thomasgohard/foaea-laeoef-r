using System;

namespace FOAEA3.Model
{
    public class TracingOutgoingProvincialData
    {
        public string ActvSt_Cd {get; set; }
        public string Recordtype {get; set; }
        public string Appl_EnfSrv_Cd {get; set; }
        public string Subm_SubmCd {get; set; }
        public string Appl_CtrlCd {get; set; }          // Val_3
        public string Appl_Source_RfrNr {get; set; }    // Val_4
        public string Subm_Recpt_SubmCd {get; set; }    // Val_5
        public DateTime TrcRsp_Rcpt_Dte {get; set; }      // Val_6
        public string TrcRsp_SeqNr {get; set; }         // Val_7
        public string TrcSt_Cd {get; set; }             // Val_8
        public string AddrTyp_Cd {get; set; }           // Val_9
        public string TrcRsp_EmplNme {get; set; }       // Val_10
        public string TrcRsp_EmplNme1 {get; set; }      // Val_11
        public string TrcRsp_Addr_Ln {get; set; }       // Val_12
        public string TrcRsp_Addr_Ln1 {get; set; }      // Val_13
        public string TrcRsp_Addr_CityNme {get; set; }  // Val_14
        public string TrcRsp_Addr_PrvCd {get; set; }    // Val_15
        public string TrcRsp_Addr_CtryCd {get; set; }   // Val_16
        public string TrcRsp_Addr_PCd {get; set; }      // Val_17
        public DateTime TrcRsp_Addr_LstUpdte {get; set; } // Val_18
        public int Prcs_RecType {get; set; }         // Val19
        public string EnfSrv_Cd {get; set; }            // Val20
    }
}
