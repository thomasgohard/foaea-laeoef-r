using System;

namespace FOAEA3.Model
{
    public class ElectronicSummonsDocumentRequiredData
    {
        public string Appl_EnfSrv_Cd { get; set; }
        public string Appl_CtrlCd { get; set; }
        public byte ESDRequired { get; set; }
        public DateTime ESDRequiredDate { get; set; }
        public DateTime? ESDReceivedDate { get; set; }
    }
}
