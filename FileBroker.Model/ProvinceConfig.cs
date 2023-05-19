namespace FileBroker.Model
{
    public class ProvinceConfig
    {
        private string[] frenchAuditProvinceCodes;
        private string[] autoSwearEnfSrvCodes;
        private string[] autoAcceptEnfSrvCodes;
        private string[] esdSiteListEnfSrvCodes;

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
