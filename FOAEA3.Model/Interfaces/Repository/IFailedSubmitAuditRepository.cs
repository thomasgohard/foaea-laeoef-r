using FOAEA3.Model.Enums;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Repository
{
    public interface IFailedSubmitAuditRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        Task AppendFiledSubmitAudit(string subject_submitter, FailedSubmitActivityAreaType activityAreaType, string error);
    }
}