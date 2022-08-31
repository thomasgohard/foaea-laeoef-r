using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    public class InMemorySummonsSummary : ISummonsSummaryRepository
    {
        public string CurrentSubmitter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string UserId { get; set; }

        public async Task CreateSummonsSummaryAsync(SummonsSummaryData summSmryData)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<SummonsSummaryData>> GetAmountOwedRecordsAsync()
        {
            return await Task.Run(() =>
            {
                return InMemData.SummSmryTestData.FindAll(m => m.SummSmry_Recalc_Dte <= DateTime.Now.Date);
            });
        }

        public async Task<decimal> GetFeesOwedTotalAsync(int yearsCount, DateTime finTermsEffectiveDate, bool isFeeCumulative)
        {
            return await Task.Run(() =>
            {
                return 38.0M;  // these are the annual fees since 1999-03-01});
            });
        }

        public async Task<List<SummonsSummaryData>> GetSummonsSummaryAsync(string appl_EnfSrv_Cd = "", string appl_CtrlCd = "", string debtorId = "")
        {
            return await Task.Run(() =>
            {
                if (!string.IsNullOrEmpty(appl_CtrlCd))
                    return InMemData.SummSmryTestData.FindAll(m => m.Appl_CtrlCd == appl_CtrlCd);
                else
                    return InMemData.SummSmryTestData;
            });
        }

        public async Task UpdateSummonsSummaryAsync(SummonsSummaryData summSmryData)
        {
            await Task.Run(() =>
            {
                var currentSummSmry = InMemData.SummSmryTestData.FindAll(m => (m.Appl_CtrlCd == summSmryData.Appl_CtrlCd) && (m.Appl_EnfSrv_Cd == summSmryData.Appl_EnfSrv_Cd)).FirstOrDefault();

                currentSummSmry.Appl_EnfSrv_Cd = summSmryData.Appl_EnfSrv_Cd;
                currentSummSmry.Appl_CtrlCd = summSmryData.Appl_CtrlCd;
                currentSummSmry.Dbtr_Id = summSmryData.Dbtr_Id;
                currentSummSmry.Appl_JusticeNrSfx = summSmryData.Appl_JusticeNrSfx;
                currentSummSmry.Start_Dte = summSmryData.Start_Dte;
                currentSummSmry.End_Dte = summSmryData.End_Dte;
                currentSummSmry.ActualEnd_Dte = summSmryData.ActualEnd_Dte;
                currentSummSmry.CourtRefNr = summSmryData.CourtRefNr;
                currentSummSmry.FeePaidTtl_Money = summSmryData.FeePaidTtl_Money;
                currentSummSmry.LmpSumPaidTtl_Money = summSmryData.LmpSumPaidTtl_Money;
                currentSummSmry.PerPymPaidTtl_Money = summSmryData.PerPymPaidTtl_Money;
                currentSummSmry.HldbAmtTtl_Money = summSmryData.HldbAmtTtl_Money;
                currentSummSmry.Appl_TotalAmnt = summSmryData.Appl_TotalAmnt;
                currentSummSmry.FeeDivertedTtl_Money = summSmryData.FeeDivertedTtl_Money;
                currentSummSmry.LmpSumDivertedTtl_Money = summSmryData.LmpSumDivertedTtl_Money;
                currentSummSmry.PerPymDivertedTtl_Money = summSmryData.PerPymDivertedTtl_Money;
                currentSummSmry.FeeOwedTtl_Money = summSmryData.FeeOwedTtl_Money;
                currentSummSmry.LmpSumOwedTtl_Money = summSmryData.LmpSumOwedTtl_Money;
                currentSummSmry.PerPymOwedTtl_Money = summSmryData.PerPymOwedTtl_Money;
                currentSummSmry.SummSmry_LastCalc_Dte = summSmryData.SummSmry_LastCalc_Dte;
                currentSummSmry.SummSmry_Recalc_Dte = summSmryData.SummSmry_Recalc_Dte;
                currentSummSmry.SummSmry_Vary_Cnt = summSmryData.SummSmry_Vary_Cnt;
            });
        }
    }
}
