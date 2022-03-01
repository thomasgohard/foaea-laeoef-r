using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Model
{
    public class ControlBatchData
    {
        public string Batch_Id { get; set; }
        public string EnfSrv_Src_Cd { get; set; }
        public string DataEntryBatch_Id { get; set; }
        public string BatchType_Cd { get; set; }
        public DateTime Batch_Post_Dte { get; set; }
        public DateTime? Batch_Compl_Dte { get; set; }
        public string Medium_Cd { get; set; }
        public int? SourceRecCnt { get; set; }
        public int? DoJRecCnt { get; set; }
        public decimal? SourceTtlAmt_Money { get; set; }
        public decimal? DoJTtlAmt_Money { get; set; }
        public short BatchLiSt_Cd { get; set; }
        public int? Batch_Reas_Cd { get; set; }
        public byte Batch_Pend_Ind { get; set; }
        public decimal? PendTtlAmt_Money { get; set; }
        public decimal? FeesTtlAmt_Money { get; set; }
    }
}
