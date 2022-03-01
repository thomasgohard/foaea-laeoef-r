using System;

namespace FOAEA3.Model.Interfaces
{
    public interface IGarnPeriodRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public void UpdateGarnPeriod(string applEnfSrvCd, string applCtrlCd, decimal finTrmLumpSumAmt, decimal finTrmPerPymAmt, DateTime calcStartDate,
                                     ref decimal lumpDivertedTtl, ref decimal prdPymtDivertedTtl);

    }
}
