using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface ISummonsSummaryRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<List<SummonsSummaryData>> GetSummonsSummary(string appl_EnfSrv_Cd = "", string appl_CtrlCd = "", string debtorId = "");
        Task<List<SummonsSummaryData>> GetAmountOwedRecords();
        Task<List<SummonsSummaryData>> GetFixedAmountRecalcDateRecords();
        Task<decimal> GetFeesOwedTotal(int yearsCount, DateTime finTermsEffectiveDate, bool isFeeCumulative);
        Task CreateSummonsSummary(SummonsSummaryData summSmryData);
        Task UpdateSummonsSummary(SummonsSummaryData summSmryData);
    }
}
