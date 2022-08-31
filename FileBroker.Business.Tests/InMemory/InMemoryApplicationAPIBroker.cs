using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Broker;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileBroker.Business.Tests.InMemory
{
    internal class InMemoryApplicationAPIBroker : IApplicationAPIBroker
    {
        public async Task<ApplicationData> GetApplicationAsync(string appl_EnfSrvCd, string appl_CtrlCd)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<StatsOutgoingProvincialData>> GetOutgoingProvincialStatusDataAsync(int maxRecords, string activeState, string recipientCode)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<ApplicationData> SinConfirmationAsync(string appl_EnfSrvCd, string appl_CtrlCd, SINConfirmationData confirmationData)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<ApplicationData> ValidateCoreValuesAsync(ApplicationData application)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }
    }
}
