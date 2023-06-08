using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    public class InMemorySummonsSummaryFixedAmount : ISummonsSummaryFixedAmountRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public Task CreateSummonsSummaryFixedAmount(string appl_EnfSrv_Cd, string appl_CtrlCd, DateTime fixedAmountRecalcDate)
        {
            throw new NotImplementedException();
        }

        public Task DeleteSummSmryFixedAmountRecalcDate(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task<SummonsSummaryFixedAmountData> GetSummonsSummaryFixedAmount(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task UpdateSummonsSummaryFixedAmount(SummonsSummaryFixedAmountData summSmryFixedAmount)
        {
            throw new NotImplementedException();
        }
    }
}
