using System;
using System.Collections.Generic;

namespace FOAEA3.Model
{
    public class TracingOutgoingProvincialFinancialData
    {
        public string ActvSt_Cd { get; set; }
        public string Recordtype { get; set; }
        public string Appl_EnfSrv_Cd { get; set; }
        public string Subm_SubmCd { get; set; }
        public string Appl_CtrlCd { get; set; }
        public string Appl_Source_RfrNr { get; set; }
        public string Subm_Recpt_SubmCd { get; set; }
        public DateTime TrcRsp_Rcpt_Dte { get; set; }
        public string TrcRsp_SeqNr { get; set; }
        public string TrcSt_Cd { get; set; }
        public List<TraceFinancialResponseDetailData> TraceFinancialDetails { get; set; }
        public int Prcs_RecType { get; set; }
        public string EnfSrv_Cd { get; set; }
    }
}
