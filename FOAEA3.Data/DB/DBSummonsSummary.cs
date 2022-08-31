using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBSummonsSummary : DBbase, ISummonsSummaryRepository
    {
        public DBSummonsSummary(IDBToolsAsync mainDB) : base(mainDB)
        {

        }
        public async Task<List<SummonsSummaryData>> GetSummonsSummaryAsync(string appl_EnfSrv_Cd = "", string appl_CtrlCd = "", string debtorId = "")
        {
            var parameters = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(appl_EnfSrv_Cd)) parameters.Add("Appl_Enfsrv_Cd", appl_EnfSrv_Cd);
            if (!string.IsNullOrEmpty(appl_CtrlCd)) parameters.Add("Appl_CtrlCd", appl_CtrlCd);
            if (!string.IsNullOrEmpty(debtorId)) parameters.Add("DebtorId", debtorId);

            List<SummonsSummaryData> data = await MainDB.GetDataFromStoredProcAsync<SummonsSummaryData>("GetSummSmryInfo", parameters, FillDataFromReader);

            return data;

        }

        public async Task<List<SummonsSummaryData>> GetAmountOwedRecordsAsync()
        {
            return await MainDB.GetDataFromStoredProcAsync<SummonsSummaryData>("GetAmountOwedRecords", FillDataFromReader);
        }

        public async Task<decimal> GetFeesOwedTotalAsync(int yearsCount, DateTime finTermsEffectiveDate, bool isFeeCumulative)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"SummYrsActiveCnt", yearsCount},
                    {"FinTrmSummEffvDte", finTermsEffectiveDate }
                };

            if (isFeeCumulative)
                return await MainDB.GetDataFromStoredProcAsync<decimal>("GetFeesOwedTtl", parameters);
            else
                return await MainDB.GetDataFromStoredProcAsync<decimal>("GetFeesOwedTtlNonCumulative", parameters);
        }

        public async Task CreateSummonsSummaryAsync(SummonsSummaryData summSmryData)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Appl_EnfSrv_Cd", summSmryData.Appl_EnfSrv_Cd},
                {"Appl_CtrlCd", summSmryData.Appl_CtrlCd},
                {"Dbtr_Id", summSmryData.Dbtr_Id},
                {"Appl_JusticeNrSfx", summSmryData.Appl_JusticeNrSfx},
                {"Start_Dte", summSmryData.Start_Dte},
                {"End_Dte", summSmryData.End_Dte},
                {"FeePaidTtl_Money", summSmryData.FeePaidTtl_Money},
                {"LmpSumPaidTtl_Money", summSmryData.LmpSumPaidTtl_Money},
                {"PerPymPaidTtl_Money", summSmryData.PerPymPaidTtl_Money},
                {"HldbAmtTtl_Money", summSmryData.HldbAmtTtl_Money},
                {"Appl_TotalAmnt", summSmryData.Appl_TotalAmnt},
                {"FeeDivertedTtl_Money", summSmryData.FeeDivertedTtl_Money},
                {"LmpSumDivertedTtl_Money", summSmryData.LmpSumDivertedTtl_Money},
                {"PerPymDivertedTtl_Money", summSmryData.PerPymDivertedTtl_Money},
                {"FeeOwedTtl_Money", summSmryData.FeeOwedTtl_Money},
                {"LmpSumOwedTtl_Money", summSmryData.LmpSumOwedTtl_Money},
                {"PerPymOwedTtl_Money", summSmryData.PerPymOwedTtl_Money},
                {"SummSmry_Recalc_Dte", summSmryData.SummSmry_Recalc_Dte},
                {"SummSmry_Vary_Cnt", summSmryData.SummSmry_Vary_Cnt}
            };

            if (summSmryData.ActualEnd_Dte.HasValue)
                parameters.Add("ActualEnd_Dte", summSmryData.ActualEnd_Dte);

            if (!string.IsNullOrEmpty(summSmryData.CourtRefNr))
                parameters.Add("CourtRefNr", summSmryData.CourtRefNr);

            if (summSmryData.SummSmry_LastCalc_Dte.HasValue)
                parameters.Add("SummSmry_LastCalc_Dte", summSmryData.SummSmry_LastCalc_Dte);

            _ = await MainDB.ExecProcAsync("SummSmry_Insert", parameters);
        }

        public async Task UpdateSummonsSummaryAsync(SummonsSummaryData summSmryData)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Appl_EnfSrv_Cd", summSmryData.Appl_EnfSrv_Cd},
                {"Appl_CtrlCd", summSmryData.Appl_CtrlCd},
                {"Dbtr_Id", summSmryData.Dbtr_Id},
                {"Start_Dte", summSmryData.Start_Dte},
                {"End_Dte", summSmryData.End_Dte},
                {"FeePaidTtl_Money", summSmryData.FeePaidTtl_Money},
                {"LmpSumPaidTtl_Money", summSmryData.LmpSumPaidTtl_Money},
                {"PerPymPaidTtl_Money", summSmryData.PerPymPaidTtl_Money},
                {"HldbAmtTtl_Money", summSmryData.HldbAmtTtl_Money},
                {"Appl_TotalAmnt", summSmryData.Appl_TotalAmnt},
                {"FeeDivertedTtl_Money", summSmryData.FeeDivertedTtl_Money},
                {"LmpSumDivertedTtl_Money", summSmryData.LmpSumDivertedTtl_Money},
                {"PerPymDivertedTtl_Money", summSmryData.PerPymDivertedTtl_Money},
                {"FeeOwedTtl_Money", summSmryData.FeeOwedTtl_Money},
                {"LmpSumOwedTtl_Money", summSmryData.LmpSumOwedTtl_Money},
                {"PerPymOwedTtl_Money", summSmryData.PerPymOwedTtl_Money},
                {"SummSmry_Vary_Cnt", summSmryData.SummSmry_Vary_Cnt}
            };

            // also add nullable values if they have a value otherwise add DBNull (regular nulls don't work for SQL)

            if (summSmryData.Appl_JusticeNrSfx != null)
                parameters.Add("Appl_JusticeNrSfx", summSmryData.Appl_JusticeNrSfx);
            else
                parameters.Add("Appl_JusticeNrSfx", DBNull.Value);

            if (summSmryData.ActualEnd_Dte.HasValue)
                parameters.Add("ActualEnd_Dte", summSmryData.ActualEnd_Dte);
            else
                parameters.Add("ActualEnd_Dte", DBNull.Value);

            if (summSmryData.CourtRefNr != null)
                parameters.Add("CourtRefNr", summSmryData.CourtRefNr);
            else
                parameters.Add("CourtRefNr", DBNull.Value);

            if (summSmryData.SummSmry_LastCalc_Dte.HasValue)
                parameters.Add("SummSmry_LastCalc_Dte", summSmryData.SummSmry_LastCalc_Dte);
            else
                parameters.Add("SummSmry_LastCalc_Dte", DBNull.Value);

            if (summSmryData.SummSmry_Recalc_Dte.HasValue)
                parameters.Add("SummSmry_Recalc_Dte", summSmryData.SummSmry_Recalc_Dte.Value);
            else
                parameters.Add("SummSmry_Recalc_Dte", DBNull.Value);


            _ = await MainDB.ExecProcAsync("SummSmryUpdate", parameters);

        }

        private void FillDataFromReader(IDBHelperReader rdr, SummonsSummaryData data)
        {
            data.Appl_EnfSrv_Cd = rdr["Appl_EnfSrv_Cd"] as string;
            data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string;
            data.Dbtr_Id = rdr["Dbtr_Id"] as string;
            data.Appl_JusticeNrSfx = rdr["Appl_JusticeNrSfx"] as string; // can be null 
            data.Start_Dte = (DateTime)rdr["Start_Dte"];
            data.End_Dte = (DateTime)rdr["End_Dte"];
            data.ActualEnd_Dte = rdr["ActualEnd_Dte"] as DateTime?; // can be null 
            data.CourtRefNr = rdr["CourtRefNr"] as string; // can be null 
            data.FeePaidTtl_Money = (decimal)rdr["FeePaidTtl_Money"];
            data.LmpSumPaidTtl_Money = (decimal)rdr["LmpSumPaidTtl_Money"];
            data.PerPymPaidTtl_Money = (decimal)rdr["PerPymPaidTtl_Money"];
            data.HldbAmtTtl_Money = (decimal)rdr["HldbAmtTtl_Money"];
            data.Appl_TotalAmnt = (decimal)rdr["Appl_TotalAmnt"];
            data.FeeDivertedTtl_Money = (decimal)rdr["FeeDivertedTtl_Money"];
            data.LmpSumDivertedTtl_Money = (decimal)rdr["LmpSumDivertedTtl_Money"];
            data.PerPymDivertedTtl_Money = (decimal)rdr["PerPymDivertedTtl_Money"];
            data.FeeOwedTtl_Money = (decimal)rdr["FeeOwedTtl_Money"];
            data.LmpSumOwedTtl_Money = (decimal)rdr["LmpSumOwedTtl_Money"];
            data.PerPymOwedTtl_Money = (decimal)rdr["PerPymOwedTtl_Money"];
            data.SummSmry_LastCalc_Dte = rdr["SummSmry_LastCalc_Dte"] as DateTime?; // can be null 
            data.SummSmry_Recalc_Dte = rdr["SummSmry_Recalc_Dte"] as DateTime?; // can be null 
            data.SummSmry_Vary_Cnt = (short)rdr["SummSmry_Vary_Cnt"];
        }

    }
}
