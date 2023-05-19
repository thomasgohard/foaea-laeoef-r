using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface IControlBatchAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }

        Task CloseControlBatch(string batchId);
        Task<ControlBatchData> CreateControlBatch(ControlBatchData controlBatchData);
        Task<ControlBatchData> GetControlBatch(string batchId);
        Task<DateTime> GetLastUiBatchLoaded();
        Task MarkBatchAsLoaded(ControlBatchData controlBatchData);
        Task<List<BatchSimpleData>> GetReadyDivertFundsBatches(string enfSrv_Cd, string enfSrv_Loc_Cd);
    }
}
