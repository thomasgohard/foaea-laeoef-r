using FOAEA3.Model.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Model.Interfaces
{
    public interface IControlBatchRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public DataList<ControlBatchData> GetFADAReadyBatch(string EnfSrv_Source_Cd = "", string DAFABatchID = "");
        public void CreateXFControlBatch(ControlBatchData values, out string returnCode, out string batchID, out string reasonCode, out string reasonText);

    }
}
