using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestData.Data
{
    public static class SummDFTestData
    {
        public static void SetupSummDFTestData(int periodCount = 1)
        {
            InMemData.SummDFTestData.Clear();

            //DateTime now = DateTime.Now;

            //InMemData.SummDFTestData.Add(new SummDF_Data
            //{
            //    SummDF_Id = 1,
            //    SummFAFR_Id = 1,
            //    Batch_Id = "00000002",
            //    IntFinH_Dte = now.AddDays(-35),
            //    SummDF_Divert_Dte = now,
            //    SummDF_CurrPerCnt = 1,
            //    SummDF_ProRatePerc = 100.0M,
            //    SummDF_MultiSumm_Ind = 0,
            //    SummDF_LmpSumDueAmt_Money = 0.0M,
            //    SummDF_PerPymDueAmt_Money = 0.0M,
            //    SummDF_DivertedDbtrAmt_Money = 0.0M,
            //    SummDF_DivOrigDbtrAmt_Money = 0.0M,
            //    SummDF_DivertedAmt_Money = 0.0M,
            //    SummDF_DivOrigAmt_Money = 0.0M,
            //    SummDF_HldbAmt_Money = 0.0M,
            //    SummDF_FeeAmt_Money = 0.0M,
            //    SummDF_LmpSumAmt_Money = 0.0M,
            //    SummDF_PerPymAmt_Money = 0.0M,
            //    SummDF_MiscAmt_Money = 0.0M
            //});
        }
    }
}
