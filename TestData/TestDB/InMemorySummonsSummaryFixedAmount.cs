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

        public async Task CreateSummonsSummaryFixedAmountAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, DateTime fixedAmountRecalcDate)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task DeleteSummSmryFixedAmountRecalcDateAsync(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<SummonsSummaryFixedAmountData> GetSummonsSummaryFixedAmountAsync(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task UpdateSummonsSummaryFixedAmountAsync(SummonsSummaryFixedAmountData summSmryFixedAmount)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }
    }
}
