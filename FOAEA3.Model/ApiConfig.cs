using FOAEA3.Resources.Helpers;
using System;

namespace FOAEA3.Model
{
    public class ApiConfig
    {
        private string applicationRootAPI;
        private string interceptionRootAPI;
        private string licenceDenialRootAPI;
        private string tracingRootAPI;
        private string incomingMEPInterceptionRootAPI;
        private string incomingMEPLicenceDenialRootAPI;
        private string incomingMEPTracingRootAPI;
        private string incomingFederalInterceptionRootAPI;
        private string incomingFederalLicenceDenialRootAPI;
        private string incomingFederalTracingRootAPI;
        private string incomingFederalSINRootAPI;
        private string outgoingMEPInterceptionRootAPI;
        private string outgoingMEPLicenceDenialRootAPI;
        private string outgoingMEPTracingRootAPI;
        private string outgoingFederalInterceptionRootAPI;
        private string outgoingFederalLicenceDenialRootAPI;
        private string outgoingFederalTracingRootAPI;
        private string outgoingFederalSINRootAPI;
        private string backendProcessesRootAPI;

        public string ApplicationRootAPI
        {
            get => applicationRootAPI;
            set => applicationRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }
        public string InterceptionRootAPI
        {
            get => interceptionRootAPI;
            set => interceptionRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }
        public string LicenceDenialRootAPI
        {
            get => licenceDenialRootAPI;
            set => licenceDenialRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }
        public string TracingRootAPI
        {
            get => tracingRootAPI;
            set => tracingRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }
        public string IncomingMEPInterceptionRootAPI
        {
            get => incomingMEPInterceptionRootAPI;
            set => incomingMEPInterceptionRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }
        public string IncomingMEPLicenceDenialRootAPI
        {
            get => incomingMEPLicenceDenialRootAPI;
            set => incomingMEPLicenceDenialRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }
        public string IncomingMEPTracingRootAPI
        {
            get => incomingMEPTracingRootAPI;
            set => incomingMEPTracingRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }
        public string IncomingFederalInterceptionRootAPI
        {
            get => incomingFederalInterceptionRootAPI;
            set => incomingFederalInterceptionRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }
        public string IncomingFederalLicenceDenialRootAPI
        {
            get => incomingFederalLicenceDenialRootAPI;
            set => incomingFederalLicenceDenialRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }
        public string IncomingFederalTracingRootAPI
        {
            get => incomingFederalTracingRootAPI;
            set => incomingFederalTracingRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }
        public string IncomingFederalSINRootAPI
        {
            get => incomingFederalSINRootAPI;
            set => incomingFederalSINRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }
        public string OutgoingMEPInterceptionRootAPI
        {
            get => outgoingMEPInterceptionRootAPI;
            set => outgoingMEPInterceptionRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }
        public string OutgoingMEPLicenceDenialRootAPI
        {
            get => outgoingMEPLicenceDenialRootAPI;
            set => outgoingMEPLicenceDenialRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }
        public string OutgoingMEPTracingRootAPI
        {
            get => outgoingMEPTracingRootAPI;
            set => outgoingMEPTracingRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }
        public string OutgoingFederalInterceptionRootAPI
        {
            get => outgoingFederalInterceptionRootAPI;
            set => outgoingFederalInterceptionRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }
        public string OutgoingFederalLicenceDenialRootAPI
        {
            get => outgoingFederalLicenceDenialRootAPI;
            set => outgoingFederalLicenceDenialRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }
        public string OutgoingFederalTracingRootAPI
        {
            get => outgoingFederalTracingRootAPI;
            set => outgoingFederalTracingRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }
        public string OutgoingFederalSINRootAPI
        {
            get => outgoingFederalSINRootAPI;
            set => outgoingFederalSINRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }
        
        public string BackendProcessesRootAPI
        {
            get => backendProcessesRootAPI;
            set => backendProcessesRootAPI = value.ReplaceVariablesWithEnvironmentValues();
        }


    }
}
