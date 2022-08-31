using FOAEA3.Model.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface IAccessAuditRepository
    {
        string CurrentSubmitter { get; set; }
        string UserId { get; set; }

        Task<int> SaveDataPageInfoAsync(AccessAuditPage auditPage, string subject_submitter);
        Task SaveDataValueAsync(int pageId, string key, string value);
        Task<List<AccessAuditElementTypeData>> GetAllElementAccessTypeAsync();
    }
}