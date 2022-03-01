using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Text;
using TestData.Helpers;

namespace TestData.Data
{
    public static class InterceptionFinHoldbackTestData
    {
        public static void SetupInterceptionFinHoldbackTestData(int periodCount = 1, decimal periodicPaymentOwed = 10.0M, string periodicPaymentCode = "C")
        {
            InMemData.IntFinHoldbackTestData.Clear();

            var calculatedCreationDate = PeriodHelper.AdjustNowForPeriod(DateTime.Now, periodCount, periodicPaymentCode);

            InMemData.IntFinHoldbackTestData.Add(new InterceptionFinancialHoldbackData
            {
                Appl_EnfSrv_Cd = "ON01",
                Appl_CtrlCd = "00002",
                IntFinH_LmpSum_Money = 0.0M,
                IntFinH_PerPym_Money = periodicPaymentOwed,
                PymPr_Cd = periodicPaymentCode,
                HldbCtg_Cd = "0",
                HldbTyp_Cd = "T",
                IntFinH_CmlPrPym_Ind = 1,
                IntFinH_TtlAmn_Money = 0.0M,
                IntFinH_LiStCd = 10,
                ActvSt_Cd = "A",
                IntFinH_Dte = calculatedCreationDate, 
                IntFinH_RcvtAffdvt_Dte = calculatedCreationDate.Date, 
                IntFinH_Affdvt_SubmCd = "FO2SSS"
            }) ;
        }

    }
}
