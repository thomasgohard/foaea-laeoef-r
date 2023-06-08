using FOAEA3.Model.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface ISummFAFR_DERepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<SummFAFR_DE_Data> GetSummFaFrDe(int summFAFR_Id);
        Task<DataList<SummFAFR_DE_Data>> GetSummFaFrDeReadyBatches(string enfSrv_Src_Cd, string DAFABatchId);
        Task<int> CountDuplicateFundsAvailable(string debtorId, string enfsrv_src_cd, string enfsrv_loc_cd,
                                               string paymentId, DateTime payableDate, int summFaFrId = 0);
        Task<int> CountDuplicateFundsReversal(string debtorId, string enfsrv_src_cd, string enfsrv_loc_cd,
                                              string paymentId, DateTime payableDate, int summFaFrId = 0);
        Task<SummFAFR_DE_Data> CreateFaFrDe(SummFAFR_DE_Data data);
    }
}
