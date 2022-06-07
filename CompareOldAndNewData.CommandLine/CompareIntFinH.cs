using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;

namespace CompareOldAndNewData.CommandLine
{
    internal static class CompareIntFinH
    {
        public static List<DiffData> Run(IRepositories repositories2, IRepositories repositories3,
                                         string enfSrv, string ctrlCd)
        {
            var diffs = new List<DiffData>();

            string key = ApplKey.MakeKey(enfSrv, ctrlCd) + " [A]";

            // active only

            var intFinH2 = repositories2.InterceptionRepository.GetInterceptionFinancialTerms(enfSrv, ctrlCd);
            var intFinH3 = repositories3.InterceptionRepository.GetInterceptionFinancialTerms(enfSrv, ctrlCd);

            if ((intFinH2 is null) && (intFinH3 is null))
                return diffs;

            if (intFinH2 is null)
                diffs.Add(new DiffData(key: key, colName: "",
                                       goodValue: "Not found in FOAEA 2!", badValue: ""));
            else if (intFinH3 is null)
                diffs.Add(new DiffData(key: key, colName: "",
                                       goodValue: "", badValue: "Not found in FOAEA 3!"));
            else
                CompareData(intFinH2, intFinH3, ref diffs, key);

            // all non-active

            var allIntFinHdata2 = repositories2.InterceptionRepository.GetAllInterceptionFinancialTerms(enfSrv, ctrlCd)
                                                                      .Where(m => m.ActvSt_Cd != "A").ToList();
            var allIntFinHdata3 = repositories3.InterceptionRepository.GetAllInterceptionFinancialTerms(enfSrv, ctrlCd)
                                                                      .Where(m => m.ActvSt_Cd != "A").ToList();

            foreach (var intFinH2_notActive in allIntFinHdata2)
            {
                string nonActiveKey = ApplKey.MakeKey(enfSrv, ctrlCd) + $" [{intFinH2_notActive.ActvSt_Cd}]/[{intFinH2_notActive.IntFinH_Dte}]";
                var intFinH3_notActive = allIntFinHdata3.Where(m => m.IntFinH_Dte == intFinH2_notActive.IntFinH_Dte).FirstOrDefault();
                if (intFinH3_notActive is null)
                    diffs.Add(new DiffData(key: nonActiveKey, colName: "", goodValue: "", badValue: "Missing in FOAEA 3!"));
                else
                    CompareData(intFinH2_notActive, intFinH3_notActive, ref diffs, nonActiveKey);
            }

            foreach (var intFinH3_notActive in allIntFinHdata3)
            {
                string nonActiveKey = ApplKey.MakeKey(enfSrv, ctrlCd) + $" [{intFinH3_notActive.ActvSt_Cd}]/[{intFinH3_notActive.IntFinH_Dte}]";
                var intFinH2_notActive = allIntFinHdata3.Where(m => m.IntFinH_Dte == intFinH3_notActive.IntFinH_Dte).FirstOrDefault();
                if (intFinH3_notActive is null)
                    diffs.Add(new DiffData(key: nonActiveKey, colName: "", goodValue: "Missing in FOAEA 2!", badValue: ""));
            }

            return diffs;
        }

        private static void CompareData(InterceptionFinancialHoldbackData intFinH2, InterceptionFinancialHoldbackData intFinH3, ref List<DiffData> diffs, string key)
        {
            if (intFinH2.Appl_EnfSrv_Cd != intFinH3.Appl_EnfSrv_Cd) diffs.Add(new DiffData(key: key, colName: "Appl_EnfSrv_Cd", goodValue: intFinH2.Appl_EnfSrv_Cd, badValue: intFinH3.Appl_EnfSrv_Cd));
            if (intFinH2.Appl_CtrlCd != intFinH3.Appl_CtrlCd) diffs.Add(new DiffData(key: key, colName: "Appl_CtrlCd", goodValue: intFinH2.Appl_CtrlCd, badValue: intFinH3.Appl_CtrlCd));
            if (intFinH2.IntFinH_Dte != intFinH3.IntFinH_Dte) diffs.Add(new DiffData(key: key, colName: "IntFinH_Dte", goodValue: intFinH2.IntFinH_Dte, badValue: intFinH3.IntFinH_Dte));
            if (intFinH2.IntFinH_RcvtAffdvt_Dte != intFinH3.IntFinH_RcvtAffdvt_Dte) diffs.Add(new DiffData(key: key, colName: "IntFinH_RcvtAffdvt_Dte", goodValue: intFinH2.IntFinH_RcvtAffdvt_Dte, badValue: intFinH3.IntFinH_RcvtAffdvt_Dte));
            if (intFinH2.IntFinH_Affdvt_SubmCd != intFinH3.IntFinH_Affdvt_SubmCd) diffs.Add(new DiffData(key: key, colName: "IntFinH_Affdvt_SubmCd", goodValue: intFinH2.IntFinH_Affdvt_SubmCd, badValue: intFinH3.IntFinH_Affdvt_SubmCd));
            if (intFinH2.PymPr_Cd != intFinH3.PymPr_Cd) diffs.Add(new DiffData(key: key, colName: "PymPr_Cd", goodValue: intFinH2.PymPr_Cd, badValue: intFinH3.PymPr_Cd));
            if (intFinH2.IntFinH_NextRecalcDate_Cd != intFinH3.IntFinH_NextRecalcDate_Cd) diffs.Add(new DiffData(key: key, colName: "IntFinH_NextRecalcDate_Cd", goodValue: intFinH2.IntFinH_NextRecalcDate_Cd, badValue: intFinH3.IntFinH_NextRecalcDate_Cd));
            if (intFinH2.HldbTyp_Cd != intFinH3.HldbTyp_Cd) diffs.Add(new DiffData(key: key, colName: "HldbTyp_Cd", goodValue: intFinH2.HldbTyp_Cd, badValue: intFinH3.HldbTyp_Cd));
            if (intFinH2.IntFinH_DefHldbAmn_Money != intFinH3.IntFinH_DefHldbAmn_Money) diffs.Add(new DiffData(key: key, colName: "IntFinH_DefHldbAmn_Money", goodValue: intFinH2.IntFinH_DefHldbAmn_Money, badValue: intFinH3.IntFinH_DefHldbAmn_Money));
            if (intFinH2.IntFinH_DefHldbPrcnt != intFinH3.IntFinH_DefHldbPrcnt) diffs.Add(new DiffData(key: key, colName: "IntFinH_DefHldbPrcnt", goodValue: intFinH2.IntFinH_DefHldbPrcnt, badValue: intFinH3.IntFinH_DefHldbPrcnt));
            if (intFinH2.HldbCtg_Cd != intFinH3.HldbCtg_Cd) diffs.Add(new DiffData(key: key, colName: "HldbCtg_Cd", goodValue: intFinH2.HldbCtg_Cd, badValue: intFinH3.HldbCtg_Cd));
            if (intFinH2.IntFinH_CmlPrPym_Ind != intFinH3.IntFinH_CmlPrPym_Ind) diffs.Add(new DiffData(key: key, colName: "IntFinH_CmlPrPym_Ind", goodValue: intFinH2.IntFinH_CmlPrPym_Ind, badValue: intFinH3.IntFinH_CmlPrPym_Ind));
            if (intFinH2.IntFinH_MxmTtl_Money != intFinH3.IntFinH_MxmTtl_Money) diffs.Add(new DiffData(key: key, colName: "IntFinH_MxmTtl_Money", goodValue: intFinH2.IntFinH_MxmTtl_Money, badValue: intFinH3.IntFinH_MxmTtl_Money));
            if (intFinH2.IntFinH_PerPym_Money != intFinH3.IntFinH_PerPym_Money) diffs.Add(new DiffData(key: key, colName: "IntFinH_PerPym_Money", goodValue: intFinH2.IntFinH_PerPym_Money, badValue: intFinH3.IntFinH_PerPym_Money));
            if (intFinH2.IntFinH_LmpSum_Money != intFinH3.IntFinH_LmpSum_Money) diffs.Add(new DiffData(key: key, colName: "IntFinH_LmpSum_Money", goodValue: intFinH2.IntFinH_LmpSum_Money, badValue: intFinH3.IntFinH_LmpSum_Money));
            if (intFinH2.IntFinH_TtlAmn_Money != intFinH3.IntFinH_TtlAmn_Money) diffs.Add(new DiffData(key: key, colName: "IntFinH_TtlAmn_Money", goodValue: intFinH2.IntFinH_TtlAmn_Money, badValue: intFinH3.IntFinH_TtlAmn_Money));
            if (intFinH2.IntFinH_VarIss_Dte != intFinH3.IntFinH_VarIss_Dte) diffs.Add(new DiffData(key: key, colName: "IntFinH_VarIss_Dte", goodValue: intFinH2.IntFinH_VarIss_Dte, badValue: intFinH3.IntFinH_VarIss_Dte));
            if (intFinH2.IntFinH_CreateUsr != intFinH3.IntFinH_CreateUsr) diffs.Add(new DiffData(key: key, colName: "IntFinH_CreateUsr", goodValue: intFinH2.IntFinH_CreateUsr, badValue: intFinH3.IntFinH_CreateUsr));
            if (intFinH2.IntFinH_LiStCd != intFinH3.IntFinH_LiStCd) diffs.Add(new DiffData(key: key, colName: "IntFinH_LiStCd", goodValue: intFinH2.IntFinH_LiStCd, badValue: intFinH3.IntFinH_LiStCd));
            if (intFinH2.ActvSt_Cd != intFinH3.ActvSt_Cd) diffs.Add(new DiffData(key: key, colName: "ActvSt_Cd", goodValue: intFinH2.ActvSt_Cd, badValue: intFinH3.ActvSt_Cd));
            if (intFinH2.IntFinH_DefHldbAmn_Period != intFinH3.IntFinH_DefHldbAmn_Period) diffs.Add(new DiffData(key: key, colName: "IntFinH_DefHldbAmn_Period", goodValue: intFinH2.IntFinH_DefHldbAmn_Period, badValue: intFinH3.IntFinH_DefHldbAmn_Period));

        }
    }
}
