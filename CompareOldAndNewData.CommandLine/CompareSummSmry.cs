using FOAEA3.Common.Helpers;
using FOAEA3.Model.Interfaces.Repository;

namespace CompareOldAndNewData.CommandLine
{
    internal static class CompareSummSmry
    {
        public static async Task<List<DiffData>> RunAsync(string tableName, IRepositories_Finance repositories2, IRepositories_Finance repositories3,
                                         string enfSrv, string ctrlCd, string category)
        {
            var diffs = new List<DiffData>();

            string key = ApplKey.MakeKey(enfSrv, ctrlCd) + " " + category + " ";

            var summSmry2 = (await repositories2.SummonsSummaryRepository.GetSummonsSummaryAsync(enfSrv, ctrlCd)).FirstOrDefault();
            var summSmry3 = (await repositories3.SummonsSummaryRepository.GetSummonsSummaryAsync(enfSrv, ctrlCd)).FirstOrDefault();

            if ((summSmry2 is null) && (summSmry3 is null))
                return diffs;

            if (summSmry2 is null)
            {
                diffs.Add(new DiffData(tableName, key: key, colName: "",
                                       goodValue: "", badValue: "Not found in FOAEA 3!"));
                return diffs;
            }

            if (summSmry3 is null)
            {
                diffs.Add(new DiffData(tableName, key: key, colName: "",
                                       goodValue: "Not found in FOAEA 2!", badValue: ""));
                return diffs;
            }

            if (summSmry2.Dbtr_Id != summSmry3.Dbtr_Id) diffs.Add(new DiffData(tableName, key: key, colName: "Dbtr_Id", goodValue: summSmry2.Dbtr_Id, badValue: summSmry3.Dbtr_Id));
            if (summSmry2.Appl_JusticeNrSfx != summSmry3.Appl_JusticeNrSfx) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_JusticeNrSfx", goodValue: summSmry2.Appl_JusticeNrSfx, badValue: summSmry3.Appl_JusticeNrSfx));
            if (summSmry2.Start_Dte != summSmry3.Start_Dte) diffs.Add(new DiffData(tableName, key: key, colName: "Start_Dte", goodValue: summSmry2.Start_Dte, badValue: summSmry3.Start_Dte));
            if (summSmry2.End_Dte != summSmry3.End_Dte) diffs.Add(new DiffData(tableName, key: key, colName: "End_Dte", goodValue: summSmry2.End_Dte, badValue: summSmry3.End_Dte));
            if (summSmry2.ActualEnd_Dte?.Date != summSmry3.ActualEnd_Dte?.Date) diffs.Add(new DiffData(tableName, key: key, colName: "ActualEnd_Dte", goodValue: summSmry2.ActualEnd_Dte, badValue: summSmry3.ActualEnd_Dte));
            if (summSmry2.CourtRefNr != summSmry3.CourtRefNr) diffs.Add(new DiffData(tableName, key: key, colName: "CourtRefNr", goodValue: summSmry2.CourtRefNr, badValue: summSmry3.CourtRefNr));
            if (summSmry2.FeePaidTtl_Money != summSmry3.FeePaidTtl_Money) diffs.Add(new DiffData(tableName, key: key, colName: "FeePaidTtl_Money", goodValue: summSmry2.FeePaidTtl_Money, badValue: summSmry3.FeePaidTtl_Money));
            if (summSmry2.LmpSumPaidTtl_Money != summSmry3.LmpSumPaidTtl_Money) diffs.Add(new DiffData(tableName, key: key, colName: "LmpSumPaidTtl_Money", goodValue: summSmry2.LmpSumPaidTtl_Money, badValue: summSmry3.LmpSumPaidTtl_Money));
            if (summSmry2.PerPymPaidTtl_Money != summSmry3.PerPymPaidTtl_Money) diffs.Add(new DiffData(tableName, key: key, colName: "PerPymPaidTtl_Money", goodValue: summSmry2.PerPymPaidTtl_Money, badValue: summSmry3.PerPymPaidTtl_Money));
            if (summSmry2.HldbAmtTtl_Money != summSmry3.HldbAmtTtl_Money) diffs.Add(new DiffData(tableName, key: key, colName: "HldbAmtTtl_Money", goodValue: summSmry2.HldbAmtTtl_Money, badValue: summSmry3.HldbAmtTtl_Money));
            if (summSmry2.Appl_TotalAmnt != summSmry3.Appl_TotalAmnt) diffs.Add(new DiffData(tableName, key: key, colName: "Appl_TotalAmnt", goodValue: summSmry2.Appl_TotalAmnt, badValue: summSmry3.Appl_TotalAmnt));
            if (summSmry2.FeeDivertedTtl_Money != summSmry3.FeeDivertedTtl_Money) diffs.Add(new DiffData(tableName, key: key, colName: "FeeDivertedTtl_Money", goodValue: summSmry2.FeeDivertedTtl_Money, badValue: summSmry3.FeeDivertedTtl_Money));
            if (summSmry2.LmpSumDivertedTtl_Money != summSmry3.LmpSumDivertedTtl_Money) diffs.Add(new DiffData(tableName, key: key, colName: "LmpSumDivertedTtl_Money", goodValue: summSmry2.LmpSumDivertedTtl_Money, badValue: summSmry3.LmpSumDivertedTtl_Money));
            if (summSmry2.PerPymDivertedTtl_Money != summSmry3.PerPymDivertedTtl_Money) diffs.Add(new DiffData(tableName, key: key, colName: "PerPymDivertedTtl_Money", goodValue: summSmry2.PerPymDivertedTtl_Money, badValue: summSmry3.PerPymDivertedTtl_Money));
            if (summSmry2.FeeOwedTtl_Money != summSmry3.FeeOwedTtl_Money) diffs.Add(new DiffData(tableName, key: key, colName: "FeeOwedTtl_Money", goodValue: summSmry2.FeeOwedTtl_Money, badValue: summSmry3.FeeOwedTtl_Money));
            if (summSmry2.LmpSumOwedTtl_Money != summSmry3.LmpSumOwedTtl_Money) diffs.Add(new DiffData(tableName, key: key, colName: "LmpSumOwedTtl_Money", goodValue: summSmry2.LmpSumOwedTtl_Money, badValue: summSmry3.LmpSumOwedTtl_Money));
            if (summSmry2.PerPymOwedTtl_Money != summSmry3.PerPymOwedTtl_Money) diffs.Add(new DiffData(tableName, key: key, colName: "PerPymOwedTtl_Money", goodValue: summSmry2.PerPymOwedTtl_Money, badValue: summSmry3.PerPymOwedTtl_Money));
            if (summSmry2.SummSmry_LastCalc_Dte?.Date != summSmry3.SummSmry_LastCalc_Dte?.Date) diffs.Add(new DiffData(tableName, key: key, colName: "SummSmry_LastCalc_Dte", goodValue: summSmry2.SummSmry_LastCalc_Dte, badValue: summSmry3.SummSmry_LastCalc_Dte));
            if (summSmry2.SummSmry_Recalc_Dte?.Date != summSmry3.SummSmry_Recalc_Dte?.Date) diffs.Add(new DiffData(tableName, key: key, colName: "SummSmry_Recalc_Dte", goodValue: summSmry2.SummSmry_Recalc_Dte, badValue: summSmry3.SummSmry_Recalc_Dte));
            if (summSmry2.SummSmry_Vary_Cnt != summSmry3.SummSmry_Vary_Cnt) diffs.Add(new DiffData(tableName, key: key, colName: "SummSmry_Vary_Cnt", goodValue: summSmry2.SummSmry_Vary_Cnt, badValue: summSmry3.SummSmry_Vary_Cnt));

            return diffs;
        }
    }
}
