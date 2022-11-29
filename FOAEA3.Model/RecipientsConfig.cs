using FOAEA3.Resources.Helpers;

namespace FOAEA3.Model
{
    public class RecipientsConfig
    {
        private string emailRecipients;
        private string exGratiaRecipients;
        private string systemErrorRecipients;

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
        public string SystemErrorRecipients
        {
            get => systemErrorRecipients;
            set => systemErrorRecipients = value.ReplaceVariablesWithEnvironmentValues();
        }
    }
}
