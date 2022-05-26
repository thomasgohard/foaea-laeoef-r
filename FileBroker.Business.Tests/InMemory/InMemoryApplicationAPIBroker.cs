using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Broker;
using System;
using System.Collections.Generic;

namespace FileBroker.Business.Tests.InMemory
{
    internal class InMemoryApplicationAPIBroker : IApplicationAPIBroker
    {
        public ApplicationData GetApplication(string appl_EnfSrvCd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public List<StatsOutgoingProvincialData> GetOutgoingProvincialStatusData(int maxRecords, string activeState, string recipientCode)
        {
            throw new NotImplementedException();
        }

        public ApplicationData SinConfirmation(string appl_EnfSrvCd, string appl_CtrlCd, SINConfirmationData confirmationData)
        {
            throw new NotImplementedException();
        }

        public ApplicationData ValidateCoreValues(ApplicationData application)
        {
            throw new NotImplementedException();
        }
    }
}
