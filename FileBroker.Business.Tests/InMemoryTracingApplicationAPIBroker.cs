using FileBroker.Model;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using System.Collections.Generic;

namespace FileBroker.Business.Tests
{
    public class InMemoryTracingApplicationAPIBroker : ITracingApplicationAPIBroker
    {
        public int Count { get; set; }
        public MessageDataList LastMessages { get; set; }

        public TracingApplicationData CloseTracingApplication(TracingApplicationData tracingApplication)
        {
            throw new System.NotImplementedException();
        }

        public TracingApplicationData CreateTracingApplication(TracingApplicationData tracingData)
        {
            throw new System.NotImplementedException();
        }

        public TracingApplicationData FullyServiceApplication(TracingApplicationData tracingApplication, string enfSrvCd)
        {
            throw new System.NotImplementedException();
        }

        public TracingApplicationData PartiallyServiceApplication(TracingApplicationData tracingApplication, string enfSrvCd)
        {
            throw new System.NotImplementedException();
        }

        public TracingApplicationData GetApplication(string dat_Appl_EnfSrvCd, string dat_Appl_CtrlCd)
        {
            throw new System.NotImplementedException();
        }

        public List<TraceCycleQuantityData> GetTraceCycleQuantityData(string enfSrvCd, string fileCycle)
        {
            throw new System.NotImplementedException();
        }

        public List<TraceToApplData> GetTraceToApplData()
        {
            throw new System.NotImplementedException();
        }

        public MessageDataList ProcessApplicationRequest(TracingMessageData tracingMessageData)
        {
            var result = new MessageDataList();

            string controlCode = tracingMessageData.TracingApplication.Appl_CtrlCd;

            result.Add(new MessageData(EventCode.UNDEFINED, "Appl_CtrlCd", controlCode, MessageType.Information));

            Count++;
            LastMessages = result;

            return result;
        }

        public TracingApplicationData UpdateTracingApplication(TracingApplicationData tracingApplication)
        {
            throw new System.NotImplementedException();
        }

        public List<TracingOutgoingFederalData> GetOutgoingFederalTracingRequests(int maxRecords, string activeState, int lifeState, string enfServiceCode)
        {
            throw new System.NotImplementedException();
        }

        public List<TracingOutgoingProvincialData> GetOutgoingProvincialTracingData(int maxRecords, string activeState, int lifeState, string enfServiceCode)
        {
            throw new System.NotImplementedException();
        }

        public List<TracingOutgoingProvincialData> GetOutgoingProvincialTracingData(int maxRecords, string activeState, string recipientCode)
        {
            throw new System.NotImplementedException();
        }
    }
}
