using System;

namespace FOAEA3.Model
{
    public class SummonsSummaryFixedAmountData
    {
        public string Appl_EnfSrv_Cd { get; set; }
        public string Appl_CtrlCd { get; set; }
        public DateTime SummSmry_LastFixedAmountCalc_Dte { get; set; }
        public DateTime SummSmry_FixedAmount_Recalc_Dte { get; set; }
    }
}
