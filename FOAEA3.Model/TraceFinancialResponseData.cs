using System;
using System.Collections.Generic;

namespace FOAEA3.Model
{
    public class TraceFinancialResponseData
    {
        public int TrcRspFin_Id { get; set; }
        public string Appl_EnfSrv_Cd { get; set; }
        public string Appl_CtrlCd { get; set; }
        public string EnfSrv_Cd { get; set; }
        public DateTime TrcRsp_Rcpt_Dte { get; set; }
        public int TrcRsp_SeqNr { get; set; }
        public string TrcSt_Cd { get; set; }
        public short TrcRsp_Trace_CyclNr { get; set; }
        public string ActvSt_Cd { get; set; }
        public int? Prcs_RecType { get; set; }
        public DateTime? TrcRsp_RcptViewed_Dte { get; set; }
        public string Sin { get; set; }
        public string SinXref { get; set; }

        public List<TraceFinancialResponseDetailData> TraceFinancialDetails { get; set; }
    }
}
