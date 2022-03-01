using FOAEA3.Model.Enums;

namespace FOAEA3.Model.Interfaces
{
    public interface IFailedSubmitAuditRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        void AppendFiledSubmitAudit(string subject_submitter, FailedSubmitActivityAreaType activityAreaType, string error);
    }
}