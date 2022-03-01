using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestData.TestDB
{
    public class InMemoryDivertFunds : IDivertFundsRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public decimal GetTotalDivertedForPeriod(string appl_EnfSrv_Cd, string appl_CtrlCd, int period)
        {
            var result = 0.0M;

            var data = InMemData.GarnPeriodTestData.FindAll(m => (m.Appl_CtrlCd == appl_CtrlCd) && (m.Appl_EnfSrv_Cd == appl_EnfSrv_Cd) && (m.Period_no == period));

            if (data.Count == 1)
                result = data[0].Garn_Amt;

            return result;
        }

        public decimal GetTotalFeesDiverted(string appl_EnfSrv_Cd, string appl_CtrlCd, bool isCumulativeFees)
        {
            var lastAnniversaryDate = GetApplicationLastAnniversaryDate(appl_CtrlCd, appl_EnfSrv_Cd);

            var result = (from df in InMemData.SummDFTestData
                          join fa in InMemData.SummFAFRTestData on new { df.SummFAFR_Id } equals new { fa.SummFAFR_Id }
                          where (fa.Appl_EnfSrv_Cd == appl_EnfSrv_Cd) && (fa.Appl_CtrlCd == appl_CtrlCd) && (df.SummDF_Divert_Dte >= lastAnniversaryDate)
                          select df.SummDF_FeeAmt_Money).Sum();

            return result ?? 0.0M;
        }

        public static DateTime GetApplicationLastAnniversaryDate(string appl_CtrlCd, string appl_EnfSrv_Cd)
        {
            DateTime result;

            DateTime startDate = InMemData.SummSmryTestData.FindAll(m => (m.Appl_CtrlCd == appl_CtrlCd) && (m.Appl_EnfSrv_Cd == appl_EnfSrv_Cd))
                                                           .FirstOrDefault().Start_Dte;
            int year = DateTime.Now.Year;
            bool isLeapYear = DateTime.IsLeapYear(year);

            if ((startDate.Month == 2) && (startDate.Day == 29) && !isLeapYear)
                result = new DateTime(year, 3, 1);
            else
                result = new DateTime(year, startDate.Month, startDate.Day);

            if (result.Date > DateTime.Now.Date)
                result = result.AddYears(-1);

            return result;
        }
    }
}
