using FOAEA3.Model.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IControlBatchRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<DataList<ControlBatchData>> GetFADAReadyBatchAsync(string EnfSrv_Source_Cd = "", string DAFABatchID = "");
        Task<List<BatchSimpleData>> GetReadyDivertFundsBatches(string enfSrv_Cd, string enfSrv_Loc_Cd);
        Task<(string, string, string, string)> CreateXFControlBatchAsync(ControlBatchData values);
        Task<ControlBatchData> GetControlBatchAsync(string batchId);
        Task CloseControlBatchAsync(string batchId);
        Task UpdateBatchAsync(ControlBatchData batch);
        Task UpdateBatchStateFtpProcessedAsync(string batchId, int recordCount);
        Task<bool> GetPaymentIdIsSinIndicator(string batchId);
    }
}
