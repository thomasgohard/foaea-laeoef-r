using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using System.Threading.Tasks;

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

        public async Task AddToFailedSubmitAuditAsync(FailedSubmitActivityAreaType activityType)
        {
            string subject_submitter = $"{Repositories.CurrentUser} ({Repositories.CurrentSubmitter})";

            foreach (var errorInfo in Application.Messages.GetMessagesForType(MessageType.Error))
                await Repositories.FailedSubmitAuditRepository.AppendFiledSubmitAuditAsync(subject_submitter, activityType, errorInfo.Description);

        }

    }
}
