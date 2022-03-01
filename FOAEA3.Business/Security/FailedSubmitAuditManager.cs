using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;

namespace FOAEA3.Business.Security
{
    public class FailedSubmitAuditManager
    {
        private readonly IRepositories Repositories;
        private readonly ApplicationData Application;

        public FailedSubmitAuditManager(IRepositories repositories, ApplicationData application)
        {
            Repositories = repositories;
            Application = application;
        }

        public void AddToFailedSubmitAudit(FailedSubmitActivityAreaType activityType)
        {
            string subject_submitter = $"{Repositories.CurrentUser} ({Repositories.CurrentSubmitter})";

            foreach (var errorInfo in Application.Messages.GetMessagesForType(MessageType.Error))
                Repositories.FailedSubmitAuditRepository.AppendFiledSubmitAudit(subject_submitter, activityType, errorInfo.Description);

        }

    }
}
