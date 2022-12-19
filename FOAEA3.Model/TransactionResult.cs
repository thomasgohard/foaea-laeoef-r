using FOAEA3.Model.Enums;

namespace FOAEA3.Model
{
    public class TransactionResult
    {
        public ReturnCode ReturnCode { get; set; }
        public EventCode ReasonCode { get; set; }
    }
}
