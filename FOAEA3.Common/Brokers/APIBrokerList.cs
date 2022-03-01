using FOAEA3.Model.Interfaces;

namespace FOAEA3.Common.Brokers
{
    public struct APIBrokerList
    {
        public ITracingEventAPIBroker TracingEventAPIBroker { get; set; }
        public ITracingApplicationAPIBroker TracingApplicationAPIBroker { get; set; }
        public ITraceResponseAPIBroker TraceResponseAPIBroker { get; set; }
        public IApplicationEventAPIBroker ApplicationEventAPIBroker { get; set; }
        public ISinAPIBroker SinAPIBroker { get; set; }
    }
}
