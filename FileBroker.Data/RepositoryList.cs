using FileBroker.Model.Interfaces;

namespace FileBroker.Data
{
    public struct RepositoryList
    {
        public IFlatFileSpecificationRepository FlatFileSpecs { get; set; }
        public IFileTableRepository FileTable { get; set; }
        public IFileAuditRepository FileAudit { get; set; }
        public IProcessParameterRepository ProcessParameterTable { get; set; }
        public IOutboundAuditRepository OutboundAuditDB { get; set; }
        public IErrorTrackingRepository ErrorTrackingDB { get; set; }
        public IMailServiceRepository MailServiceDB { get; set; }
    }
}
