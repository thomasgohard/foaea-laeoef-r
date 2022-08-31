using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileBroker.Business.Tests.InMemory
{
    public class InMemoryInterceptionApplicationAPIBroker : IInterceptionApplicationAPIBroker
    {
        public IAPIBrokerHelper ApiHelper => throw new NotImplementedException();

        public async Task<InterceptionApplicationData> AcceptVariationAsync(InterceptionApplicationData interceptionApplication)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<InterceptionApplicationData> CancelInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<InterceptionApplicationData> CreateInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<InterceptionApplicationData> GetApplicationAsync(string dat_Appl_EnfSrvCd, string dat_Appl_CtrlCd)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<List<InterceptionApplicationData>> GetApplicationsForVariationAutoAcceptAsync(string enfService)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<InterceptionApplicationData> SuspendInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<InterceptionApplicationData> TransferInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication, string newRecipientSubmitter, string newIssuingSubmitter)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<InterceptionApplicationData> UpdateInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }

        public async Task<InterceptionApplicationData> ValidateFinancialCoreValuesAsync(InterceptionApplicationData application)
        {
            return await Task.Run(() => { return application; });
        }

        public async Task<InterceptionApplicationData> VaryInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication)
        {
            await Task.Run(() => { });
            throw new NotImplementedException();
        }
    }
}
