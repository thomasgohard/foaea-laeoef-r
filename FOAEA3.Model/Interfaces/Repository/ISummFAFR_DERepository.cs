using FOAEA3.Model.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Model.Interfaces
{
    public interface ISummFAFR_DERepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        public DataList<SummFAFR_DE_Data> GetSummFaFrDe(int summFAFR_Id);
        public DataList<SummFAFR_DE_Data> GetSummFaFrDeReadyBatches(string enfSrv_Src_Cd, string DAFABatchId);
    }
}
