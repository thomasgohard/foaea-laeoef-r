using FOAEA3.Model.Enums;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces
{
    public interface IFailedSubmitAuditRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        Task AppendFiledSubmitAuditAsync(string subject_submitter, FailedSubmitActivityAreaType activityAreaType, string error);
    }
}