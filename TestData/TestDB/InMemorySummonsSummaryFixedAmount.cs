using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestData.TestDB
{
    public class InMemorySummonsSummaryFixedAmount : ISummonsSummaryFixedAmountRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public void CreateSummonsSummaryFixedAmount(string appl_EnfSrv_Cd, string appl_CtrlCd, DateTime fixedAmountRecalcDate)
        {
            throw new NotImplementedException();
        }

        public SummonsSummaryFixedAmountData GetSummonsSummaryFixedAmount(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public void UpdateSummonsSummaryFixedAmount(SummonsSummaryFixedAmountData summSmryFixedAmount)
        {
            throw new NotImplementedException();
        }
    }
}
