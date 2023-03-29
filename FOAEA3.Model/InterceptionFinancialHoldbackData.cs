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

        public bool ValuesEqual(object obj)
        {
            if (obj is not InterceptionFinancialHoldbackData newTerms)
                return false;

            if ((newTerms.Appl_EnfSrv_Cd == Appl_EnfSrv_Cd) &&
                (newTerms.Appl_CtrlCd == Appl_CtrlCd) &&
                (newTerms.IntFinH_Dte == IntFinH_Dte) &&
                (newTerms.IntFinH_RcvtAffdvt_Dte == IntFinH_RcvtAffdvt_Dte) &&
                (newTerms.IntFinH_Affdvt_SubmCd == IntFinH_Affdvt_SubmCd) &&
                (newTerms.PymPr_Cd == PymPr_Cd) &&
                (newTerms.IntFinH_NextRecalcDate_Cd == IntFinH_NextRecalcDate_Cd) &&
                (newTerms.HldbTyp_Cd == HldbTyp_Cd) &&
                (newTerms.IntFinH_DefHldbAmn_Money == IntFinH_DefHldbAmn_Money) &&
                (newTerms.IntFinH_DefHldbPrcnt == IntFinH_DefHldbPrcnt) &&
                (newTerms.HldbCtg_Cd == HldbCtg_Cd) &&
                (newTerms.IntFinH_CmlPrPym_Ind == IntFinH_CmlPrPym_Ind) &&
                (newTerms.IntFinH_MxmTtl_Money == IntFinH_MxmTtl_Money) &&
                (newTerms.IntFinH_PerPym_Money == IntFinH_PerPym_Money) &&
                (newTerms.IntFinH_LmpSum_Money == IntFinH_LmpSum_Money) &&
                (newTerms.IntFinH_TtlAmn_Money == IntFinH_TtlAmn_Money) &&
                (newTerms.IntFinH_VarIss_Dte == IntFinH_VarIss_Dte) &&
                (newTerms.IntFinH_CreateUsr == IntFinH_CreateUsr) &&
                (newTerms.IntFinH_LiStCd == IntFinH_LiStCd) &&
                (newTerms.ActvSt_Cd == ActvSt_Cd) &&
                (newTerms.IntFinH_DefHldbAmn_Period == IntFinH_DefHldbAmn_Period))
            {
                return true;
            }
            else
                return false;
        }
    }
}
