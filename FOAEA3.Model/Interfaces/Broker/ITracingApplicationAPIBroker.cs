using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces
{
    public interface ITracingApplicationAPIBroker
    {
        TracingApplicationData CloseTracingApplication(TracingApplicationData tracingApplication);
        TracingApplicationData CreateTracingApplication(TracingApplicationData tracingData);
        TracingApplicationData FullyServiceApplication(TracingApplicationData tracingApplication, string enfSrvCd);
        TracingApplicationData PartiallyServiceApplication(TracingApplicationData tracingApplication, string enfSrvCd);
        TracingApplicationData GetApplication(string dat_Appl_EnfSrvCd, string dat_Appl_CtrlCd);
        List<TraceCycleQuantityData> GetTraceCycleQuantityData(string enfSrvCd, string fileCycle);
        List<TraceToApplData> GetTraceToApplData();
        TracingApplicationData UpdateTracingApplication(TracingApplicationData tracingApplication);
        TracingApplicationData TransferTracingApplication(TracingApplicationData tracingApplication,
                                                                 string newRecipientSubmitter,
                                                                 string newIssuingSubmitter);
        List<TracingOutgoingFederalData> GetOutgoingFederalTracingRequests(int maxRecords, string activeState,
                                                                           int lifeState, string enfServiceCode);
        List<TracingOutgoingProvincialData> GetOutgoingProvincialTracingData(int maxRecords, string activeState,
                                                                             string recipientCode);
    }
}
