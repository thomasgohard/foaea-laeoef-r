using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces
{
    public interface IGarnSummaryRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        List<GarnSummaryData> GetGarnSummary(string appl_EnfSrv_Cd, string enfOfficeCd, int fiscalMonth, int fiscalYear);
        void CreateGarnSummary(GarnSummaryData garnSummaryData);
        void UpdateGarnSummary(GarnSummaryData garnSummaryData);
    }
}
