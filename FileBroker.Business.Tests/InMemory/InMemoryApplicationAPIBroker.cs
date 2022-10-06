using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Broker;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileBroker.Business.Tests.InMemory
{
    internal class InMemoryApplicationAPIBroker : IApplicationAPIBroker
    {
        public Task<ApplicationData> GetApplicationAsync(string appl_EnfSrvCd, string appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task<List<StatsOutgoingProvincialData>> GetOutgoingProvincialStatusDataAsync(int maxRecords, string activeState, string recipientCode)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationData> SinConfirmationAsync(string appl_EnfSrvCd, string appl_CtrlCd, SINConfirmationData confirmationData)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationData> ValidateCoreValuesAsync(ApplicationData application)
        {
            return Task.FromResult(application);
        }
    }
}
