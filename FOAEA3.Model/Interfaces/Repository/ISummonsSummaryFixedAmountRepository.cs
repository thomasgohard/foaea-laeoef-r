using System;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface ISummonsSummaryFixedAmountRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<SummonsSummaryFixedAmountData> GetSummonsSummaryFixedAmountAsync(string appl_EnfSrv_Cd, string appl_CtrlCd);
        Task CreateSummonsSummaryFixedAmountAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, DateTime fixedAmountRecalcDate);
        Task UpdateSummonsSummaryFixedAmountAsync(SummonsSummaryFixedAmountData summSmryFixedAmount);
        Task DeleteSummSmryFixedAmountRecalcDateAsync(string appl_EnfSrv_Cd, string appl_CtrlCd);
    }
}
