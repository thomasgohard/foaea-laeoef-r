using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IGarnSummaryRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<List<GarnSummaryData>> GetGarnSummaryAsync(string appl_EnfSrv_Cd, string enfOfficeCd, int fiscalMonth, int fiscalYear);
        Task CreateGarnSummaryAsync(GarnSummaryData garnSummaryData);
        Task UpdateGarnSummaryAsync(GarnSummaryData garnSummaryData);
    }
}
