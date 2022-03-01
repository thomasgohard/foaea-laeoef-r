using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestData.Data
{
    public static class SummFAFRTestData
    {
        public static void SetupSummFAFRTestData(int periodCount = 1)
        {
            InMemData.SummFAFRTestData.Clear();

            //DateTime now = DateTime.Now;

            //InMemData.SummFAFRTestData.Add(new SummFAFR_Data {
            //    SummFAFR_Id = 1,
            //    Appl_EnfSrv_Cd = "ON01",
            //    Appl_CtrlCd = "00002",
            //    Batch_Id = "00000001",
            //    SummFAFR_OrigFA_Ind = 1,
            //    SummFAFRLiSt_Cd = 1,
            //    SummFAFR_Reas_Cd = null,
            //    EnfSrv_Src_Cd = "RC00",
            //    EnfSrv_Loc_Cd = "01",
            //    EnfSrv_SubLoc_Cd = null,
            //    SummFAFR_FA_Pym_Id = "123123123",
            //    SummFAFR_MultiSumm_Ind = 0,
            //    SummFAFR_Post_Dte = now,
            //    SummFAFR_FA_Payable_Dte = now.AddDays(-1).Date,
            //    SummFAFR_AvailDbtrAmt_Money = 10.0M,
            //    SummFAFR_AvailAmt_Money = 10.0M,
            //    SummFAFR_Comments = "ABC0001"
            //});
        }
    }
}
