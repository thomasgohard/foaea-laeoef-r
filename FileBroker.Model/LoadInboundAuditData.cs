using System;

namespace FOAEA3.Model
{
    public class LoadInboundAuditData
    {
        public string Appl_EnfSrv_Cd { get; set; }
        public string Appl_CtrlCd { get; set; }
        public string Appl_Source_RfrNr { get; set; }
        public string InboundFilename { get; set; }
        public DateTime? Timestamp { get; set; }
        public bool? IsCompleted { get; set; }
    }
}
