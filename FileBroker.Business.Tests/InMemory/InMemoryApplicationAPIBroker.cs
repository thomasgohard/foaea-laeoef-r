using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileBroker.Business.Tests.InMemory
{
    internal class InMemoryApplicationAPIBroker : IApplicationAPIBroker
    {
        public IAPIBrokerHelper ApiHelper => throw new NotImplementedException();

        public string Token { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Task<ApplicationData> GetApplication(string appl_EnfSrvCd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task<List<ApplicationData>> GetApplicationsForSin(string confirmedSIN)
        {
            throw new NotImplementedException();
        }

        public Task<List<StatsOutgoingProvincialData>> GetOutgoingProvincialStatusData(int maxRecords, string activeState, string recipientCode)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationData> SinConfirmation(string appl_EnfSrvCd, string appl_CtrlCd, SINConfirmationData confirmationData)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationData> ValidateCoreValues(ApplicationData application)
        {
            return Task.FromResult(application);
        }
    }
}
