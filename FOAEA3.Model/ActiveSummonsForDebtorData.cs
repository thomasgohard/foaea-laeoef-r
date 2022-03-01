using System;

namespace FOAEA3.Model
{
    public class ActiveSummonsData
    {
        public string Subm_SubmCd { get; set; }
        public string Appl_JusticeNr { get; set; }
        public decimal IntFinH_LmpSum_Money { get; set; }
        public decimal? IntFinH_PerPym_Money { get; set; }
        public decimal? IntFinH_MxmTtl_Money { get; set; }
        public string PymPr_Cd { get; set; }
        public int? IntFinH_NextRecalcDate_Cd { get; set; }
        public byte? IntFinH_CmlPrPym_Ind { get; set; }
        public string HldbCtg_Cd { get; set; }
        public int? IntFinH_DefHldbPrcnt { get; set; }
        public decimal? IntFinH_DefHldbAmn_Money { get; set; }
        public string IntFinH_DefHldbAmn_Period { get; set; }
        public string HldbTyp_Cd { get; set; }
        public DateTime Start_Dte { get; set; }
        public decimal FeeDivertedTtl_Money { get; set; }
        public decimal LmpSumDivertedTtl_Money { get; set; }
        public decimal PerPymDivertedTtl_Money { get; set; }
        public decimal HldbAmtTtl_Money { get; set; }
        public decimal Appl_TotalAmnt { get; set; }
        public DateTime IntFinH_Dte { get; set; }
        public DateTime End_Dte { get; set; }
        public DateTime? Appl_RecvAffdvt_Dte { get; set; }
        public DateTime? IntFinH_VarIss_Dte { get; set; }
        public decimal LmpSumOwedTtl_Money { get; set; }
        public decimal PerPymOwedTtl_Money { get; set; }
        public string Appl_EnfSrv_Cd { get; set; }
        public DateTime? VarEnterDte { get; set; }
        public string Appl_CtrlCd { get; set; }
    }
}
