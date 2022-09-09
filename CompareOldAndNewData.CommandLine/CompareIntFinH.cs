using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;

namespace CompareOldAndNewData.CommandLine
{
    internal static class CompareIntFinH
    {
        public static async Task<List<DiffData>> RunAsync(string tableName, IRepositories repositories2, IRepositories repositories3,
                                         string enfSrv, string ctrlCd)
        {
            var diffs = new List<DiffData>();

            var allIntFinHdata2 = await repositories2.InterceptionTable.GetAllInterceptionFinancialTermsAsync(enfSrv, ctrlCd);
            var allIntFinHdata3 = await repositories3.InterceptionTable.GetAllInterceptionFinancialTermsAsync(enfSrv, ctrlCd);

            foreach (var intFinH2item in allIntFinHdata2)
            {
                string key = ApplKey.MakeKey(enfSrv, ctrlCd) + $" [{intFinH2item.ActvSt_Cd}]/[{intFinH2item.IntFinH_Dte.Date}]";
                var intFinH3item = allIntFinHdata3.Where(m => (m.IntFinH_Dte.Date == intFinH2item.IntFinH_Dte.Date) ||
                                                              ((m.ActvSt_Cd == "A") && (m.ActvSt_Cd == intFinH2item.ActvSt_Cd)) ||
                                                              ((m.ActvSt_Cd == "P") && (m.ActvSt_Cd == intFinH2item.ActvSt_Cd))).FirstOrDefault();
                if (intFinH3item is null)
                    diffs.Add(new DiffData(tableName, key: key, colName: "", goodValue: "", badValue: "Missing in FOAEA 3!"));
                else
                    CompareData(tableName, intFinH2item, intFinH3item, ref diffs, key);
            }

            foreach (var intFinH3item in allIntFinHdata3)
            {
                string key = ApplKey.MakeKey(enfSrv, ctrlCd) + $" [{intFinH3item.ActvSt_Cd}]/[{intFinH3item.IntFinH_Dte.Date}]";
                var intFinH2item = allIntFinHdata2.Where(m => (m.IntFinH_Dte.Date == intFinH3item.IntFinH_Dte.Date) ||
                                                              ((m.ActvSt_Cd == "A") && (m.ActvSt_Cd == intFinH3item.ActvSt_Cd)) ||
                                                              ((m.ActvSt_Cd == "P") && (m.ActvSt_Cd == intFinH3item.ActvSt_Cd))).FirstOrDefault();
                if (intFinH2item is null)
                    diffs.Add(new DiffData(tableName, key: key, colName: "", goodValue: "Missing in FOAEA 2!", badValue: ""));
            }

            return diffs;
        }

        private static void CompareData(string tableName, InterceptionFinancialHoldbackData intFinH2, InterceptionFinancialHoldbackData intFinH3, ref List<DiffData> diffs, string key)
        {
            if (intFinH2.Appl_EnfSrv_Cd != intFinH3.Appl_EnfSrv_Cd) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_EnfSrv_Cd", goodValue: intFinH2.Appl_EnfSrv_Cd, badValue: intFinH3.Appl_EnfSrv_Cd));
            if (intFinH2.Appl_CtrlCd != intFinH3.Appl_CtrlCd) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_CtrlCd", goodValue: intFinH2.Appl_CtrlCd, badValue: intFinH3.Appl_CtrlCd));
            if (intFinH2.IntFinH_Dte.Date != intFinH3.IntFinH_Dte.Date) diffs.Add(new DiffData(tableName, key: key, colName: "IntFinH_Dte", goodValue: intFinH2.IntFinH_Dte, badValue: intFinH3.IntFinH_Dte));
            if (intFinH2.IntFinH_RcvtAffdvt_Dte?.Date != intFinH3.IntFinH_RcvtAffdvt_Dte?.Date) diffs.Add(new DiffData(tableName, key: key, colName: "IntFinH_RcvtAffdvt_Dte", goodValue: intFinH2.IntFinH_RcvtAffdvt_Dte, badValue: intFinH3.IntFinH_RcvtAffdvt_Dte));
            if (intFinH2.IntFinH_Affdvt_SubmCd != intFinH3.IntFinH_Affdvt_SubmCd) diffs.Add(new DiffData(tableName, key: key, colName: "IntFinH_Affdvt_SubmCd", goodValue: intFinH2.IntFinH_Affdvt_SubmCd, badValue: intFinH3.IntFinH_Affdvt_SubmCd));
            if (intFinH2.PymPr_Cd != intFinH3.PymPr_Cd) diffs.Add(new DiffData(tableName, key: key, colName: "PymPr_Cd", goodValue: intFinH2.PymPr_Cd, badValue: intFinH3.PymPr_Cd));
            if (intFinH2.IntFinH_NextRecalcDate_Cd != intFinH3.IntFinH_NextRecalcDate_Cd) diffs.Add(new DiffData(tableName, key: key, colName: "IntFinH_NextRecalcDate_Cd", goodValue: intFinH2.IntFinH_NextRecalcDate_Cd, badValue: intFinH3.IntFinH_NextRecalcDate_Cd));
            if (intFinH2.HldbTyp_Cd != intFinH3.HldbTyp_Cd) diffs.Add(new DiffData(tableName, key: key, colName: "HldbTyp_Cd", goodValue: intFinH2.HldbTyp_Cd, badValue: intFinH3.HldbTyp_Cd));
            if (intFinH2.IntFinH_DefHldbAmn_Money != intFinH3.IntFinH_DefHldbAmn_Money) diffs.Add(new DiffData(tableName, key: key, colName: "IntFinH_DefHldbAmn_Money", goodValue: intFinH2.IntFinH_DefHldbAmn_Money, badValue: intFinH3.IntFinH_DefHldbAmn_Money));
            if (intFinH2.IntFinH_DefHldbPrcnt != intFinH3.IntFinH_DefHldbPrcnt) diffs.Add(new DiffData(tableName, key: key, colName: "IntFinH_DefHldbPrcnt", goodValue: intFinH2.IntFinH_DefHldbPrcnt, badValue: intFinH3.IntFinH_DefHldbPrcnt));
            if (intFinH2.HldbCtg_Cd != intFinH3.HldbCtg_Cd) diffs.Add(new DiffData(tableName, key: key, colName: "HldbCtg_Cd", goodValue: intFinH2.HldbCtg_Cd, badValue: intFinH3.HldbCtg_Cd));
            if (intFinH2.IntFinH_CmlPrPym_Ind != intFinH3.IntFinH_CmlPrPym_Ind) diffs.Add(new DiffData(tableName, key: key, colName: "IntFinH_CmlPrPym_Ind", goodValue: intFinH2.IntFinH_CmlPrPym_Ind, badValue: intFinH3.IntFinH_CmlPrPym_Ind));
            if (intFinH2.IntFinH_MxmTtl_Money != intFinH3.IntFinH_MxmTtl_Money) diffs.Add(new DiffData(tableName, key: key, colName: "IntFinH_MxmTtl_Money", goodValue: intFinH2.IntFinH_MxmTtl_Money, badValue: intFinH3.IntFinH_MxmTtl_Money));
            if (intFinH2.IntFinH_PerPym_Money != intFinH3.IntFinH_PerPym_Money) diffs.Add(new DiffData(tableName, key: key, colName: "IntFinH_PerPym_Money", goodValue: intFinH2.IntFinH_PerPym_Money, badValue: intFinH3.IntFinH_PerPym_Money));
            if (intFinH2.IntFinH_LmpSum_Money != intFinH3.IntFinH_LmpSum_Money) diffs.Add(new DiffData(tableName, key: key, colName: "IntFinH_LmpSum_Money", goodValue: intFinH2.IntFinH_LmpSum_Money, badValue: intFinH3.IntFinH_LmpSum_Money));
            if (intFinH2.IntFinH_TtlAmn_Money != intFinH3.IntFinH_TtlAmn_Money) diffs.Add(new DiffData(tableName, key: key, colName: "IntFinH_TtlAmn_Money", goodValue: intFinH2.IntFinH_TtlAmn_Money, badValue: intFinH3.IntFinH_TtlAmn_Money));
            if (intFinH2.IntFinH_VarIss_Dte?.Date != intFinH3.IntFinH_VarIss_Dte?.Date) diffs.Add(new DiffData(tableName, key: key, colName: "IntFinH_VarIss_Dte", goodValue: intFinH2.IntFinH_VarIss_Dte, badValue: intFinH3.IntFinH_VarIss_Dte));
            if (intFinH2.IntFinH_CreateUsr != intFinH3.IntFinH_CreateUsr) diffs.Add(new DiffData(tableName, key: key, colName: "IntFinH_CreateUsr", goodValue: intFinH2.IntFinH_CreateUsr, badValue: intFinH3.IntFinH_CreateUsr));
            if (intFinH2.IntFinH_LiStCd != intFinH3.IntFinH_LiStCd) diffs.Add(new DiffData(tableName, key: key, colName: "IntFinH_LiStCd", goodValue: intFinH2.IntFinH_LiStCd, badValue: intFinH3.IntFinH_LiStCd));
            if (intFinH2.ActvSt_Cd != intFinH3.ActvSt_Cd) diffs.Add(new DiffData(tableName, key: key, colName: "ActvSt_Cd", goodValue: intFinH2.ActvSt_Cd, badValue: intFinH3.ActvSt_Cd));
            if (intFinH2.IntFinH_DefHldbAmn_Period != intFinH3.IntFinH_DefHldbAmn_Period) diffs.Add(new DiffData(tableName, key: key, colName: "IntFinH_DefHldbAmn_Period", goodValue: intFinH2.IntFinH_DefHldbAmn_Period, badValue: intFinH3.IntFinH_DefHldbAmn_Period));

        }
    }
}
