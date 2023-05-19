using FOAEA3.Resources.Helpers;

namespace FOAEA3.Model
{
    public class ApiConfig
    {
        private string foaeaRootAPI;
        private string foaeaInterceptionRootAPI;
        private string foaeaLicenceDenialRootAPI;
        private string foaeaTracingRootAPI;
        private string fileBrokerMEPInterceptionRootAPI;
        private string fileBrokerMEPLicenceDenialRootAPI;
        private string fileBrokerMEPTracingRootAPI;
        private string fileBrokerFederalInterceptionRootAPI;
        private string fileBrokerFederalLicenceDenialRootAPI;
        private string fileBrokerFederalTracingRootAPI;
        private string fileBrokerFederalSINRootAPI;
        private string backendProcessesRootAPI;
        private string fileBrokerAccountRootAPI;

        public string FoaeaRootAPI
        {
            get => foaeaRootAPI;
            set => foaeaRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }
        public string FoaeaInterceptionRootAPI
        {
            get => foaeaInterceptionRootAPI;
            set => foaeaInterceptionRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }
        public string FoaeaLicenceDenialRootAPI
        {
            get => foaeaLicenceDenialRootAPI;
            set => foaeaLicenceDenialRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }
        public string FoaeaTracingRootAPI
        {
            get => foaeaTracingRootAPI;
            set => foaeaTracingRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }
        public string FileBrokerMEPInterceptionRootAPI
        {
            get => fileBrokerMEPInterceptionRootAPI;
            set => fileBrokerMEPInterceptionRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }
        public string FileBrokerMEPLicenceDenialRootAPI
        {
            get => fileBrokerMEPLicenceDenialRootAPI;
            set => fileBrokerMEPLicenceDenialRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }
        public string FileBrokerMEPTracingRootAPI
        {
            get => fileBrokerMEPTracingRootAPI;
            set => fileBrokerMEPTracingRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }
        public string FileBrokerFederalInterceptionRootAPI
        {
            get => fileBrokerFederalInterceptionRootAPI;
            set => fileBrokerFederalInterceptionRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }
        public string FileBrokerFederalLicenceDenialRootAPI
        {
            get => fileBrokerFederalLicenceDenialRootAPI;
            set => fileBrokerFederalLicenceDenialRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }
        public string FileBrokerFederalTracingRootAPI
        {
            get => fileBrokerFederalTracingRootAPI;
            set => fileBrokerFederalTracingRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }
        public string FileBrokerFederalSINRootAPI
        {
            get => fileBrokerFederalSINRootAPI;
            set => fileBrokerFederalSINRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }

        public string BackendProcessesRootAPI
        {
            get => backendProcessesRootAPI;
            set => backendProcessesRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }

        public string FileBrokerAccountRootAPI
        {
            get => fileBrokerAccountRootAPI;
            set => fileBrokerAccountRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }

    }
}
