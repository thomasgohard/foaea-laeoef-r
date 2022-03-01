using FOAEA3.Model.Enums;
using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces
{
    public interface IAccessAuditRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        int SaveDataPageInfo(AccessAuditPage auditPage, string subject_submitter);
        void SaveDataValue(int pageId, string key, string value);
        List<AccessAuditElementTypeData> GetAllElementAccessType();
    }
}