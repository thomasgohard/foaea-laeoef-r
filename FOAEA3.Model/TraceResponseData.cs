using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Model
{
    public class TraceResponseData
    {
        public string Appl_EnfSrv_Cd { get; set; }
        public string Appl_CtrlCd { get; set; }
        public string EnfSrv_Cd { get; set; }
        public DateTime TrcRsp_Rcpt_Dte { get; set; }
        public int TrcRsp_SeqNr { get; set; }
        public string TrcRsp_EmplNme { get; set; }
        public string TrcRsp_EmplNme1 { get; set; }
        public string TrcSt_Cd { get; set; }
        public string TrcRsp_Addr_Ln { get; set; }
        public string TrcRsp_Addr_Ln1 { get; set; }
        public string TrcRsp_Addr_CityNme { get; set; }
        public string TrcRsp_Addr_PrvCd { get; set; }
        public string TrcRsp_Addr_CtryCd { get; set; }
        public string TrcRsp_Addr_PCd { get; set; }
        public DateTime? TrcRsp_Addr_LstUpdte { get; set; }
        public string AddrTyp_Cd { get; set; }
        public byte TrcRsp_SubmViewed_Ind { get; set; }
        public byte TrcRsp_RcptViewed_Ind { get; set; }
        public byte TrcRsp_SubmAddrUsed_Ind { get; set; }
        public byte TrcRsp_SubmAddrHasValue_Ind { get; set; }
        public short TrcRsp_Trace_CyclNr { get; set; }
        public string ActvSt_Cd { get; set; }
        public int? Prcs_RecType { get; set; }
        public DateTime? TrcRsp_RcptViewed_Dte { get; set; }

        // extra values

        public string Subm_SubmCd { get; set; }
        public string Address { get; set; }
        public string Originator { get; set; }
    }
}
