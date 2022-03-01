using System;

namespace FOAEA3.Model
{
    public class InterceptionFinancialHoldbackData
    {
        public string Appl_EnfSrv_Cd { get; set; }
        public string Appl_CtrlCd { get; set; }
        public DateTime IntFinH_Dte { get; set; }
        public DateTime? IntFinH_RcvtAffdvt_Dte { get; set; }
        public string IntFinH_Affdvt_SubmCd { get; set; }
        public string PymPr_Cd { get; set; }
        public int? IntFinH_NextRecalcDate_Cd { get; set; }
        public string HldbTyp_Cd { get; set; }
        public decimal? IntFinH_DefHldbAmn_Money { get; set; }
        public int? IntFinH_DefHldbPrcnt { get; set; }
        public string HldbCtg_Cd { get; set; }
        public byte? IntFinH_CmlPrPym_Ind { get; set; }
        public decimal? IntFinH_MxmTtl_Money { get; set; }
        public decimal? IntFinH_PerPym_Money { get; set; }
        public decimal IntFinH_LmpSum_Money { get; set; }
        public decimal IntFinH_TtlAmn_Money { get; set; }
        public DateTime? IntFinH_VarIss_Dte { get; set; }
        public string IntFinH_CreateUsr { get; set; }
        public short IntFinH_LiStCd { get; set; }
        public string ActvSt_Cd { get; set; }
        public string IntFinH_DefHldbAmn_Period { get; set; }
    }
}
