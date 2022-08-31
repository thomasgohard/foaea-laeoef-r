using FOAEA3.Model.Interfaces;
using System;
using System.Threading.Tasks;

namespace TestData.TestDB
{
    public class InMemoryGarnPeriod : IGarnPeriodRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public async Task<(decimal, decimal)> UpdateGarnPeriodAsync(string applEnfSrvCd, string applCtrlCd, decimal finTrmLumpSumAmt, decimal finTrmPerPymAmt, DateTime calcStartDate, decimal lumpDivertedTtl, decimal prdPymtDivertedTtl)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }
    }
}
