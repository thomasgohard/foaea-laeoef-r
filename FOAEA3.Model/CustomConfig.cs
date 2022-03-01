using FOAEA3.Resources.Helpers;
using System.Collections.Generic;

namespace FOAEA3.Model
{
    public class CustomConfig
    {
        private string auditRecipients;
        private string emailRecipients;
        private string exGratiaRecipients;

        public string AuditRecipients
        {
            get => auditRecipients;
            set => auditRecipients = value.ReplaceVariablesWithEnvironmentValues();
        }
        public string EmailRecipients
        {
            get => emailRecipients;
            set => emailRecipients = value.ReplaceVariablesWithEnvironmentValues();
        }
        public string ExGratiaRecipients
        {
            get => exGratiaRecipients;
            set => exGratiaRecipients = value.ReplaceVariablesWithEnvironmentValues();
        }

        public List<string> ESDsites { get; set; }
    }
}
