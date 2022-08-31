using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;

namespace CompareOldAndNewData.CommandLine
{
    internal static class CompareHldbCnd
    {
        public static async Task<List<DiffData>> RunAsync(string tableName, IRepositories repositories2, IRepositories repositories3,
                                         string enfSrv, string ctrlCd)
        {
            var diffs = new List<DiffData>();

            var holdback2 = await repositories2.InterceptionRepository.GetAllHoldbackConditionsAsync(enfSrv, ctrlCd);
            var holdback3 = await repositories3.InterceptionRepository.GetAllHoldbackConditionsAsync(enfSrv, ctrlCd);

            foreach (var holdbackItem2 in holdback2)
            {
                string key = ApplKey.MakeKey(enfSrv, ctrlCd) + $" [{holdbackItem2.ActvSt_Cd}]/[{holdbackItem2.IntFinH_Dte.Date}]/[{holdbackItem2.EnfSrv_Cd}]";
                var holdbackItem3 = holdback3.Where(m => ((m.IntFinH_Dte.Date == holdbackItem2.IntFinH_Dte.Date) && (m.EnfSrv_Cd == holdbackItem2.EnfSrv_Cd)) || 
                                                          ((m.ActvSt_Cd == "A") && (m.ActvSt_Cd == holdbackItem2.ActvSt_Cd) && (m.EnfSrv_Cd == holdbackItem2.EnfSrv_Cd)) ||
                                                          ((m.ActvSt_Cd == "P") && (m.ActvSt_Cd == holdbackItem2.ActvSt_Cd) && (m.EnfSrv_Cd == holdbackItem2.EnfSrv_Cd))).FirstOrDefault();
                if (holdbackItem3 is null)
                    diffs.Add(new DiffData(tableName, key: key, colName: "", goodValue: "", badValue: "Missing in FOAEA 3!"));
                else
                    CompareData(tableName, holdbackItem2, holdbackItem3, ref diffs, key);
            }

            foreach (var holdbackItem3 in holdback3)
            {
                string key = ApplKey.MakeKey(enfSrv, ctrlCd) + $" [{holdbackItem3.ActvSt_Cd}]/[{holdbackItem3.IntFinH_Dte.Date}]/[{holdbackItem3.EnfSrv_Cd}]";
                var holdbackItem2 = holdback2.Where(m => ((m.IntFinH_Dte.Date == holdbackItem3.IntFinH_Dte.Date) && (m.EnfSrv_Cd == holdbackItem3.EnfSrv_Cd)) ||
                                                          ((m.ActvSt_Cd == "A") && (m.ActvSt_Cd == holdbackItem3.ActvSt_Cd) && (m.EnfSrv_Cd == holdbackItem3.EnfSrv_Cd)) ||
                                                          ((m.ActvSt_Cd == "P") && (m.ActvSt_Cd == holdbackItem3.ActvSt_Cd) && (m.EnfSrv_Cd == holdbackItem3.EnfSrv_Cd))).FirstOrDefault();
                if (holdbackItem2 is null)
                    diffs.Add(new DiffData(tableName, key: key, colName: "", goodValue: "Missing in FOAEA 2!", badValue: ""));
            }

            return diffs;
        }

        private static void CompareData(string tableName, HoldbackConditionData holdback2, HoldbackConditionData holdback3, ref List<DiffData> diffs, string key)
        {
            if (holdback2.Appl_EnfSrv_Cd != holdback3.Appl_EnfSrv_Cd) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_EnfSrv_Cd", goodValue: holdback2.Appl_EnfSrv_Cd, badValue: holdback3.Appl_EnfSrv_Cd));
            if (holdback2.Appl_CtrlCd != holdback3.Appl_CtrlCd) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_CtrlCd", goodValue: holdback2.Appl_CtrlCd, badValue: holdback3.Appl_CtrlCd));
            if (holdback2.IntFinH_Dte.Date != holdback3.IntFinH_Dte.Date) diffs.Add(new DiffData(tableName, key: key, colName: "IntFinH_Dte", goodValue: holdback2.IntFinH_Dte, badValue: holdback3.IntFinH_Dte));
            if (holdback2.EnfSrv_Cd != holdback3.EnfSrv_Cd) diffs.Add(new DiffData(tableName, key: key, colName: "EnfSrv_Cd", goodValue: holdback2.EnfSrv_Cd, badValue: holdback3.EnfSrv_Cd));
            if (holdback2.HldbCnd_MxmPerChq_Money != holdback3.HldbCnd_MxmPerChq_Money) diffs.Add(new DiffData(tableName, key: key, colName: "HldbCnd_MxmPerChq_Money", goodValue: holdback2.HldbCnd_MxmPerChq_Money, badValue: holdback3.HldbCnd_MxmPerChq_Money));
            if (holdback2.HldbCnd_SrcHldbAmn_Money != holdback3.HldbCnd_SrcHldbAmn_Money) diffs.Add(new DiffData(tableName, key: key, colName: "HldbCnd_SrcHldbAmn_Money", goodValue: holdback2.HldbCnd_SrcHldbAmn_Money, badValue: holdback3.HldbCnd_SrcHldbAmn_Money));
            if (holdback2.HldbCnd_SrcHldbPrcnt != holdback3.HldbCnd_SrcHldbPrcnt) diffs.Add(new DiffData(tableName, key: key, colName: "HldbCnd_SrcHldbPrcnt", goodValue: holdback2.HldbCnd_SrcHldbPrcnt, badValue: holdback3.HldbCnd_SrcHldbPrcnt));
            if (holdback2.HldbCtg_Cd != holdback3.HldbCtg_Cd) diffs.Add(new DiffData(tableName, key: key, colName: "HldbCtg_Cd", goodValue: holdback2.HldbCtg_Cd, badValue: holdback3.HldbCtg_Cd));
            if (holdback2.HldbCnd_LiStCd != holdback3.HldbCnd_LiStCd) diffs.Add(new DiffData(tableName, key: key, colName: "HldbCnd_LiStCd", goodValue: holdback2.HldbCnd_LiStCd, badValue: holdback3.HldbCnd_LiStCd));
            if (holdback2.ActvSt_Cd != holdback3.ActvSt_Cd) diffs.Add(new DiffData(tableName, key: key, colName: "ActvSt_Cd ", goodValue: holdback2.ActvSt_Cd, badValue: holdback3.ActvSt_Cd));
        }
    }
}
