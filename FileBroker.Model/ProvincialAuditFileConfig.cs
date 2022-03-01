using FOAEA3.Resources.Helpers;

namespace FileBroker.Model
{
    public class ProvincialAuditFileConfig
    {
        private string auditRootPath;
        private string auditRecipients;
        private string[] frenchAuditProvinceCodes;
        private string mailServer;

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
        public string[] FrenchAuditProvinceCodes
        {
            get => frenchAuditProvinceCodes;
            set => frenchAuditProvinceCodes = value;
        }

        public string MailServer
        {
            get => mailServer;
            set => mailServer = value.ReplaceVariablesWithEnvironmentValues();
        }
    }
}
