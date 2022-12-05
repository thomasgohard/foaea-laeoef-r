using FOAEA3.Common.Brokers;
using FOAEA3.Common.Brokers.Financials;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;

namespace FileBroker.Common
{
    public static class FoaeaApiHelper
    {
        public static APIBrokerList SetupFoaeaAPIs(ApiConfig apiRootData)
        {
            string token = "";
            var apiFoaeaHelper = new APIBrokerHelper(apiRootData.FoaeaRootAPI, currentSubmitter: LoginsAPIBroker.SYSTEM_SUBMITTER,
                                                     currentUser: LoginsAPIBroker.SYSTEM_SUBJECT);
            var applicationAPIs = new ApplicationAPIBroker(apiFoaeaHelper, token);
            var applicationEventsAPIs = new ApplicationEventAPIBroker(apiFoaeaHelper, token);
            var productionAuditAPIs = new ProductionAuditAPIBroker(apiFoaeaHelper, token);
            var loginAPIs = new LoginsAPIBroker(apiFoaeaHelper, token);
            var sinsAPIs = new SinAPIBroker(apiFoaeaHelper, token);
            var financialAPIs = new FinancialAPIBroker(apiFoaeaHelper, token);

            var apiTracingHelper = new APIBrokerHelper(apiRootData.FoaeaTracingRootAPI, currentSubmitter: LoginsAPIBroker.SYSTEM_SUBMITTER,
                                                       currentUser: LoginsAPIBroker.SYSTEM_SUBJECT);
            var tracingApplicationAPIs = new TracingApplicationAPIBroker(apiTracingHelper, token);
            var tracingResponsesAPIs = new TraceResponseAPIBroker(apiTracingHelper, token);
            var tracingEventsAPIs = new TracingEventAPIBroker(apiTracingHelper, token);

            var apiInterceptionHelper = new APIBrokerHelper(apiRootData.FoaeaInterceptionRootAPI, currentSubmitter: LoginsAPIBroker.SYSTEM_SUBMITTER,
                                                                currentUser: LoginsAPIBroker.SYSTEM_SUBJECT);
            var interceptionApplicationAPIs = new InterceptionApplicationAPIBroker(apiInterceptionHelper, token);

            var apiLicenceDenialHelper = new APIBrokerHelper(apiRootData.FoaeaLicenceDenialRootAPI, currentSubmitter: LoginsAPIBroker.SYSTEM_SUBMITTER,
                                                             currentUser: LoginsAPIBroker.SYSTEM_SUBJECT);
            var licenceDenialApplicationAPIs = new LicenceDenialApplicationAPIBroker(apiLicenceDenialHelper, token);
            var licenceDenialTerminationApplicationAPIs = new LicenceDenialTerminationApplicationAPIBroker(apiLicenceDenialHelper, token);
            var licenceDenialEventAPIs = new LicenceDenialEventAPIBroker(apiLicenceDenialHelper, token);
            var licenceDenialResponsesAPIs = new LicenceDenialResponseAPIBroker(apiLicenceDenialHelper, token);

            var foaeaApis = new APIBrokerList
            {
                Applications = applicationAPIs,
                ApplicationEvents = applicationEventsAPIs,
                ProductionAudits = productionAuditAPIs,
                Accounts = loginAPIs,
                Sins = sinsAPIs,
                Financials = financialAPIs,

                TracingApplications = tracingApplicationAPIs,
                TracingResponses = tracingResponsesAPIs,
                TracingEvents = tracingEventsAPIs,

                InterceptionApplications = interceptionApplicationAPIs,

                LicenceDenialApplications = licenceDenialApplicationAPIs,
                LicenceDenialTerminationApplications = licenceDenialTerminationApplicationAPIs,
                LicenceDenialEvents = licenceDenialEventAPIs,
                LicenceDenialResponses = licenceDenialResponsesAPIs
            };

            return foaeaApis;
        }
    }
}
