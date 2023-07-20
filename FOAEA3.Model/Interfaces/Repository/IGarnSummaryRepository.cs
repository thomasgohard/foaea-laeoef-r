using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IGarnSummaryRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<List<GarnSummaryData>> GetGarnSummary(string appl_EnfSrv_Cd, string enfOfficeCd, int fiscalMonth, int fiscalYear);
        Task CreateGarnSummary(GarnSummaryData garnSummaryData);
        Task UpdateGarnSummary(GarnSummaryData garnSummaryData);
    }
}
