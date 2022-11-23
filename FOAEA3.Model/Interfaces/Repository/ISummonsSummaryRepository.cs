using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface ISummonsSummaryRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<List<SummonsSummaryData>> GetSummonsSummaryAsync(string appl_EnfSrv_Cd = "", string appl_CtrlCd = "", string debtorId = "");
        Task<List<SummonsSummaryData>> GetAmountOwedRecordsAsync();
        Task<decimal> GetFeesOwedTotalAsync(int yearsCount, DateTime finTermsEffectiveDate, bool isFeeCumulative);
        Task CreateSummonsSummaryAsync(SummonsSummaryData summSmryData);
        Task UpdateSummonsSummaryAsync(SummonsSummaryData summSmryData);
    }
}
