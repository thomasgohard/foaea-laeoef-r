namespace FileBroker.Model
{
    public class InboundAuditData
    {
        public string EnforcementServiceCode { get; set; }
        public string ControlCode { get; set; }
        public string SourceReferenceNumber { get; set; }
        public string ApplicationMessage { get; set; }
        public bool IsWarning { get; set; }
    }
}
