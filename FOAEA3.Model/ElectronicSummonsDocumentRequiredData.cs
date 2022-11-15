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

    /*
        data.Appl_EnfSrv_Cd = rdr["Appl_EnfSrv_Cd"] as string;
        data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string;
        data.ESDRequired = (byte)rdr["ESDRequired"];
        data.ESDRequiredDate = (DateTime)rdr["ESDRequiredDate"];
        data.ESDReceivedDate = rdr["ESDReceivedDate"] as DateTime?; // can be null      
     */
}
