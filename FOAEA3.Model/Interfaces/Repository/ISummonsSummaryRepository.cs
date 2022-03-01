using System;
using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces
{
    public interface ISummonsSummaryRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public List<SummonsSummaryData> GetSummonsSummary(string appl_EnfSrv_Cd = "", string appl_CtrlCd = "", string debtorId = "");
        public List<SummonsSummaryData> GetAmountOwedRecords();
        public decimal GetFeesOwedTotal(int yearsCount, DateTime finTermsEffectiveDate, bool isFeeCumulative);
        void CreateSummonsSummary(SummonsSummaryData summSmryData);
        public void UpdateSummonsSummary(SummonsSummaryData summSmryData);
    }
}
