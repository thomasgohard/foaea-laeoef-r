using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Model.Interfaces
{
    public interface ISummonsSummaryFixedAmountRepository
    {
        public string CurrentSubmitter { get; set;  }
        public string UserId { get; set; }

        public SummonsSummaryFixedAmountData GetSummonsSummaryFixedAmount(string appl_EnfSrv_Cd, string appl_CtrlCd);
        public void CreateSummonsSummaryFixedAmount(string appl_EnfSrv_Cd, string appl_CtrlCd, DateTime fixedAmountRecalcDate);
        public void UpdateSummonsSummaryFixedAmount(SummonsSummaryFixedAmountData summSmryFixedAmount);
        void DeleteSummSmryFixedAmountRecalcDate(string appl_EnfSrv_Cd, string appl_CtrlCd);
    }
}
