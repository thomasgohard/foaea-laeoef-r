using System;

namespace FOAEA3.Model
{
    public class ApplicationJusticeNumberData
    {
        public string Appl_EnfSrv_Cd { get; set; }
        public string Appl_CtrlCd { get; set; }
        public string Appl_Dbtr_Cnfrmd_SIN { get; set; }
        public string DebtorID { get; set; }
        public string DebtorIDSuffix { get; set; }
        public DateTime StartDate { get; set; }

        /*
			 A.Appl_EnfSrv_Cd,
			 A.Appl_CtrlCd,
			 A.Appl_Dbtr_Cnfrmd_SIN,
             S.Dbtr_Id AS DebtorID,
             S.Appl_JusticeNrSfx AS DebtorIDSuffix,
             S.Start_Dte AS StartDate         
         */
    }
}
