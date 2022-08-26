using System.Threading.Tasks;

namespace FileBroker.Model.Interfaces
{
    public interface ILoadInboundAuditRepository
    {
        Task MarkAsCompletedAsync(string applEnfSrvCd, string applCtrlCd, string sourceRefNumber, string inboundFileName);
        Task<bool> AlreadyExistsAsync(string applEnfSrvCd, string applCtrlCd, string sourceRefNumber, string inboundFileName);
        Task AddNewAsync(string applEnfSrvCd, string applCtrlCd, string sourceRefNumber, string inboundFileName);
    }
}
