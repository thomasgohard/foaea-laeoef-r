using System;

namespace FOAEA3.Model
{
    public class LicenceSuspensionHistoryData
    {
        public string EnforcementServiceCode { get; set; }
        public string ControlCode { get; set; }
        public string FromName { get; set; }
        public string ConvertedResponseDate { get; set; }
        public DateTime ResponseDate { get; set; }
        public short ResponseCode { get; set; }
        public string LicRspSource_RefNo { get; set; }
        public string ResponseDescription_E { get; set; }
        public string ResponseDescription_F { get; set; }
    }
}
