using System;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IGarnPeriodRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<(decimal, decimal)> UpdateGarnPeriod(string applEnfSrvCd, string applCtrlCd, decimal finTrmLumpSumAmt, decimal finTrmPerPymAmt, DateTime calcStartDate,
                                     decimal lumpDivertedTtl, decimal prdPymtDivertedTtl);

    }
}
