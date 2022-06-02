using FOAEA3.Resources.Helpers;

namespace FileBroker.Model
{
    public class ProvincialAuditFileConfig
    {
        private string auditRootPath;
        private string auditRecipients;
        private string[] frenchAuditProvinceCodes;
        private string[] autoSwearEnfSrvCodes;
        private string[] autoAcceptEnfSrvCodes;
        private string[] esdSiteListEnfSrvCodes;

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

        public string[] AutoSwearEnfSrvCodes
        {
            get => autoSwearEnfSrvCodes;
            set => autoSwearEnfSrvCodes = value;
        }

        public string[] AutoAcceptEnfSrvCodes
        {
            get => autoAcceptEnfSrvCodes;
            set => autoAcceptEnfSrvCodes = value;
        }

        public string[] ESDSiteListEnfSrvCodes
        {
            get => esdSiteListEnfSrvCodes;
            set => esdSiteListEnfSrvCodes = value;
        }
    }
}
