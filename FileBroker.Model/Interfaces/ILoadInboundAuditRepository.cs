namespace FileBroker.Model.Interfaces
{
    public interface ILoadInboundAuditRepository
    {
        void MarkAsCompleted(string applEnfSrvCd, string applCtrlCd, string sourceRefNumber, string inboundFileName);
        bool AlreadyExists(string applEnfSrvCd, string applCtrlCd, string sourceRefNumber, string inboundFileName);
        void AddNew(string applEnfSrvCd, string applCtrlCd, string sourceRefNumber, string inboundFileName);
    }
}
