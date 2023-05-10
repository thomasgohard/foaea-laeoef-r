using FOAEA3.Resources.Helpers;

namespace FileBroker.Model
{
    public class ProvincialAuditFileConfig
    {
        private string auditRootPath;
        private string auditRecipients;

        public string AuditRootPath
        {
            get => auditRootPath;
            set => auditRootPath = value.ReplaceVariablesWithEnvironmentValues();
        }
        public string AuditRecipients
        {
            get => auditRecipients;
            set => auditRecipients = value.ReplaceVariablesWithEnvironmentValues();
        }       
    }
}
