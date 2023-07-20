using System;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface ISummonsSummaryFixedAmountRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<SummonsSummaryFixedAmountData> GetSummonsSummaryFixedAmount(string appl_EnfSrv_Cd, string appl_CtrlCd);
        Task CreateSummonsSummaryFixedAmount(string appl_EnfSrv_Cd, string appl_CtrlCd, DateTime fixedAmountRecalcDate);
        Task UpdateSummonsSummaryFixedAmount(SummonsSummaryFixedAmountData summSmryFixedAmount);
        Task DeleteSummSmryFixedAmountRecalcDate(string appl_EnfSrv_Cd, string appl_CtrlCd);
    }
}
