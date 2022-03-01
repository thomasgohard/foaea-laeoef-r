using System;

namespace FOAEA3.Model
{
    public class SummonsSummaryData
    {
        public string Appl_EnfSrv_Cd { get; set; }
        public string Appl_CtrlCd { get; set; }
        public string Dbtr_Id { get; set; }
        public string Appl_JusticeNrSfx { get; set; }
        public DateTime Start_Dte { get; set; }
        public DateTime End_Dte { get; set; }
        public DateTime? ActualEnd_Dte { get; set; }
        public string CourtRefNr { get; set; }
        public decimal FeePaidTtl_Money { get; set; }
        public decimal LmpSumPaidTtl_Money { get; set; }
        public decimal PerPymPaidTtl_Money { get; set; }
        public decimal HldbAmtTtl_Money { get; set; }
        public decimal Appl_TotalAmnt { get; set; }
        public decimal FeeDivertedTtl_Money { get; set; }
        public decimal LmpSumDivertedTtl_Money { get; set; }
        public decimal PerPymDivertedTtl_Money { get; set; }
        public decimal FeeOwedTtl_Money { get; set; }
        public decimal LmpSumOwedTtl_Money { get; set; }
        public decimal PerPymOwedTtl_Money { get; set; }
        public DateTime? SummSmry_LastCalc_Dte { get; set; }
        public DateTime? SummSmry_Recalc_Dte { get; set; }
        public short SummSmry_Vary_Cnt { get; set; }

        public decimal PreBalance { get; set; }
    }
}
