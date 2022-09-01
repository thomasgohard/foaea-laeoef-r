using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    public class InMemorySummonsSummaryFixedAmount : ISummonsSummaryFixedAmountRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public  Task CreateSummonsSummaryFixedAmountAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, DateTime fixedAmountRecalcDate)
        {
            throw new NotImplementedException();
        }

        public  Task DeleteSummSmryFixedAmountRecalcDateAsync(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public  Task<SummonsSummaryFixedAmountData> GetSummonsSummaryFixedAmountAsync(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public  Task UpdateSummonsSummaryFixedAmountAsync(SummonsSummaryFixedAmountData summSmryFixedAmount)
        {
            throw new NotImplementedException();
        }
    }
}
