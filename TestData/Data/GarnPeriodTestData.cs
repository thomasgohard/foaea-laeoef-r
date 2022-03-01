using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestData.Data
{
    public static class GarnPeriodTestData
    {
        public static void SetupGarnPeriodTestData(int periodCount = 1)
        {
            InMemData.GarnPeriodTestData.Clear();

            //InMemData.GarnPeriodTestData.Add(new GarnPeriodData
            //{
            //    Appl_EnfSrv_Cd = "ON01",
            //    Appl_CtrlCd = "00002",
            //    Garn_Amt = 0.0M,
            //    Period_no = 1,
            //    SummFaFr_Id = 0
            //});
        }
    }
}
