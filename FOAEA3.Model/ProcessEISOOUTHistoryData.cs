namespace FOAEA3.Model
{
    public class ProcessEISOOUTHistoryData
    {
        public string TRANS_TYPE_CD { get; set; }
        public string ACCT_NBR { get; set; }
        public string TRANS_AMT { get; set; }
        public string RQST_RFND_EID { get; set; }
        public string OUTPUT_DEST_CD { get; set; }
        public string XREF_ACCT_NBR { get; set; }
        public string FOA_DELETE_IND { get; set; }
        public string FOA_RECOUP_PRCNT { get; set; }
        public string BLANK_AREA { get; set; }
        public int? PAYMENT_RECEIVED { get; set; }
    }
}
