using FOAEA3.Model.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IAccessAuditRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<int> SaveDataPageInfo(AccessAuditPage auditPage, string subject_submitter);
        Task SaveDataValue(int pageId, string key, string value);
        Task<List<AccessAuditElementTypeData>> GetAllElementAccessType();
    }
}