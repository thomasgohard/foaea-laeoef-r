using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestData.TestDB
{
    public class InMemoryGarnPeriod : IGarnPeriodRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public void UpdateGarnPeriod(string applEnfSrvCd, string applCtrlCd, decimal finTrmLumpSumAmt, decimal finTrmPerPymAmt, DateTime calcStartDate, ref decimal lumpDivertedTtl, ref decimal prdPymtDivertedTtl)
        {
            throw new NotImplementedException();
        }
    }
}
