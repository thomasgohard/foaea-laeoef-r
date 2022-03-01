using System;
using System.Collections.Generic;
using System.Text;

namespace FileBroker.Model
{
    public class FileAuditData
    {
        public string Appl_EnfSrv_Cd { get; set; }
        public string Appl_CtrlCd { get; set; }
        public string Appl_Source_RfrNr { get; set; }
        public string ApplicationMessage { get; set; }
        public string InboundFilename { get; set; }
        public DateTime? Timestamp { get; set; }
        public bool? IsCompleted { get; set; }
    }
}
