using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers
{
    public struct APIBrokerList
    {
        public ITracingEventAPIBroker TracingEventAPIBroker { get; set; }
        public ITracingApplicationAPIBroker TracingApplicationAPIBroker { get; set; }
        public ITraceResponseAPIBroker TracingResponseAPIBroker { get; set; }

        public ILicenceDenialEventAPIBroker LicenceDenialEventAPIBroker { get; set; }
        public ILicenceDenialApplicationAPIBroker LicenceDenialApplicationAPIBroker { get; set; }
        public ILicenceDenialResponseAPIBroker LicenceDenialResponseAPIBroker { get; set; }


        public IApplicationEventAPIBroker ApplicationEventAPIBroker { get; set; }
        public IApplicationAPIBroker ApplicationAPIBroker { get; set; }

        public ISinAPIBroker SinAPIBroker { get; set; }
    }
}
