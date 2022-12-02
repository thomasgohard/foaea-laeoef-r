using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers
{
    public struct APIBrokerList
    {
        public ILoginsAPIBroker Accounts { get; set; }
        public ITracingEventAPIBroker TracingEvents { get; set; }
        public ITracingApplicationAPIBroker TracingApplications { get; set; }
        public ITraceResponseAPIBroker TracingResponses { get; set; }

        public IInterceptionApplicationAPIBroker InterceptionApplications { get; set; }

        public ILicenceDenialEventAPIBroker LicenceDenialEvents { get; set; }
        public ILicenceDenialApplicationAPIBroker LicenceDenialApplications { get; set; }
        public ILicenceDenialTerminationApplicationAPIBroker LicenceDenialTerminationApplications { get; set; }
        public ILicenceDenialResponseAPIBroker LicenceDenialResponses { get; set; }


        public IApplicationEventAPIBroker ApplicationEvents { get; set; }
        public IApplicationAPIBroker Applications { get; set; }

        public ISinAPIBroker Sins { get; set; }

        public IFinancialEventAPIBroker FinancialEvents { get; set; }

        public IProductionAuditAPIBroker ProductionAudits { get; set; }
    }
}
