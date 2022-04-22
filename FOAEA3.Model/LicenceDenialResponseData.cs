using System;
using System.Diagnostics;

namespace FOAEA3.Model
{
    [DebuggerDisplay("{Appl_EnfSrv_Cd}-{Appl_CtrlCd} [{EnfSrv_Cd}/{LicRspType}]")]
    public class LicenceDenialResponseData
    {
        public string Appl_EnfSrv_Cd { get; set; }
        public string Appl_CtrlCd { get; set; }
        public string EnfSrv_Cd { get; set; }
        public DateTime LicRsp_Rcpt_Dte { get; set; }
        public short LicRsp_SeqNr { get; set; }
        public short RqstStat_Cd { get; set; }
        public string LicRsp_Comments { get; set; }
        public string LicRspFilename { get; set; }
        public string LicRspType { get; set; }
        public string LicRspSource_RefNo { get; set; }
        public bool LicRsp_RcptViewed_Ind { get; set; }
        public DateTime? LicRsp_RcptViewed_Date { get; set; }
    }
}
