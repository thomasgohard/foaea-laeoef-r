using FOAEA3.Model;
using System;
using TestData.Helpers;

namespace TestData.Data
{
    public static class SummSmryTestData
    {
        public static void SetupAmountOwedTestData(int periodCount = 1, string periodicPaymentCode = "C")
        {
            InMemData.SummSmryTestData.Clear();

            var calculatedCreationDate = PeriodHelper.AdjustNowForPeriod(DateTime.Now, periodCount, periodicPaymentCode);
            var calculatedStartDate = calculatedCreationDate.AddDays(35);

            InMemData.SummSmryTestData.Add(new SummonsSummaryData
            {   // new application at 35 days (before amount owed calculations)
                Appl_EnfSrv_Cd = "ON01",
                Appl_CtrlCd = "00002",
                Dbtr_Id = "ABC0002",
                Appl_JusticeNrSfx = "A",
                Start_Dte = calculatedStartDate,
                End_Dte = calculatedStartDate.AddYears(5).AddDays(-1),
                ActualEnd_Dte = null,
                CourtRefNr = null,
                FeePaidTtl_Money = 0M,
                LmpSumPaidTtl_Money = 0M,
                PerPymPaidTtl_Money = 0M,
                HldbAmtTtl_Money = 0M,
                Appl_TotalAmnt = 0M,
                FeeDivertedTtl_Money = 0M,
                LmpSumDivertedTtl_Money = 0M,
                PerPymDivertedTtl_Money = 0M,
                FeeOwedTtl_Money = 0M,
                LmpSumOwedTtl_Money = 0M,
                PerPymOwedTtl_Money = 0M,
                SummSmry_LastCalc_Dte = null,
                SummSmry_Recalc_Dte = DateTime.Now.Date,
                SummSmry_Vary_Cnt = 0
            });

        }
        
    }
}
