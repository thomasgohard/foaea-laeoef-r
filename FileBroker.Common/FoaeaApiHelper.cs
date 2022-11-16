using FOAEA3.Common.Brokers;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;

namespace FileBroker.Common
{
    public static class FoaeaApiHelper
    {
        public static APIBrokerList SetupFoaeaAPIs(ApiConfig apiRootData)
        {
            string token = "";
            var apiApplHelper = new APIBrokerHelper(apiRootData.FoaeaApplicationRootAPI, currentSubmitter: LoginsAPIBroker.SYSTEM_SUBMITTER,
                                                    currentUser: LoginsAPIBroker.SYSTEM_SUBJECT);
            var applicationApplicationAPIs = new ApplicationAPIBroker(apiApplHelper, token);
            var applicationEventsAPIs = new ApplicationEventAPIBroker(apiApplHelper, token);
            var productionAuditAPIs = new ProductionAuditAPIBroker(apiApplHelper, token);
            var loginAPIs = new LoginsAPIBroker(apiApplHelper, token);
            var sinsAPIs = new SinAPIBroker(apiApplHelper, token);

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
                Applications = applicationApplicationAPIs,
                ApplicationEvents = applicationEventsAPIs,
                ProductionAudits = productionAuditAPIs,
                Accounts = loginAPIs,
                Sins = sinsAPIs,

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
