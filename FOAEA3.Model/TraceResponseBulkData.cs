using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOAEA3.Model
{
    public class TraceResponseBulkData
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

        public void LoadFrom(TraceResponseData data)
        {
            Appl_EnfSrv_Cd = data.Appl_EnfSrv_Cd;
            Appl_CtrlCd = data.Appl_CtrlCd;
            EnfSrv_Cd = data.EnfSrv_Cd;
            TrcRsp_Rcpt_Dte = data.TrcRsp_Rcpt_Dte;
            TrcRsp_SeqNr = data.TrcRsp_SeqNr;
            TrcRsp_EmplNme = data.TrcRsp_EmplNme;
            TrcRsp_EmplNme1 = data.TrcRsp_EmplNme1;
            TrcSt_Cd = data.TrcSt_Cd;
            TrcRsp_Addr_Ln = data.TrcRsp_Addr_Ln;
            TrcRsp_Addr_Ln1 = data.TrcRsp_Addr_Ln1;
            TrcRsp_Addr_CityNme = data.TrcRsp_Addr_CityNme;
            TrcRsp_Addr_PrvCd = data.TrcRsp_Addr_PrvCd;
            TrcRsp_Addr_CtryCd = data.TrcRsp_Addr_CtryCd;
            TrcRsp_Addr_PCd = data.TrcRsp_Addr_PCd;
            TrcRsp_Addr_LstUpdte = data.TrcRsp_Addr_LstUpdte;
            AddrTyp_Cd = data.AddrTyp_Cd;
            TrcRsp_SubmViewed_Ind = data.TrcRsp_SubmViewed_Ind;
            TrcRsp_RcptViewed_Ind = data.TrcRsp_RcptViewed_Ind;
            TrcRsp_SubmAddrUsed_Ind = data.TrcRsp_SubmAddrUsed_Ind;
            TrcRsp_SubmAddrHasValue_Ind = data.TrcRsp_SubmAddrHasValue_Ind;
            TrcRsp_Trace_CyclNr = data.TrcRsp_Trace_CyclNr;
            ActvSt_Cd = data.ActvSt_Cd;
            Prcs_RecType = data.Prcs_RecType;
            TrcRsp_RcptViewed_Dte = data.TrcRsp_RcptViewed_Dte;
        }
    }

}
