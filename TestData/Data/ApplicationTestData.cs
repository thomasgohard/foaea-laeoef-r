using System;
using System.Collections.Generic;
using System.Text;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using TestData.Helpers;

namespace TestData.Data
{
    public static class ApplicationTestData
    {
        public static void SetupApplicationTestData(int periodCount=1, string periodicPaymentCode = "C")
        {
            InMemData.ApplicationTestData.Clear();

            var calculatedCreationDate = PeriodHelper.AdjustNowForPeriod(DateTime.Now, periodCount, periodicPaymentCode);
            var calculatedStartDate = calculatedCreationDate.AddDays(35);

            if (calculatedStartDate > DateTime.Now)
                throw new Exception($"Invalid period count ({periodCount}) for periodic code {periodicPaymentCode}");

            InMemData.ApplicationTestData.Add(new ApplicationData
            {
                Appl_EnfSrv_Cd = "ON01",
                Appl_CtrlCd = "00002",
                AppLiSt_Cd = ApplicationState.APPLICATION_ACCEPTED_10,
                AppCtgy_Cd = "I01",
                ActvSt_Cd = "A",
                Appl_JusticeNr = "ABC0001A",
                Appl_Lgl_Dte = calculatedCreationDate.Date,
                Appl_Rcptfrm_Dte = calculatedCreationDate.Date,
                Appl_RecvAffdvt_Dte = calculatedCreationDate.Date,
                Appl_Create_Dte = calculatedCreationDate
            });


        }
    }
}
