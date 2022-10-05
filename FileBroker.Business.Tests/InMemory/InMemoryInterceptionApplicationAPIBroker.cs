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

        public Task<InterceptionApplicationData> AcceptVariationAsync(InterceptionApplicationData interceptionApplication, string token)
        {
            throw new NotImplementedException();
        }

        public Task<InterceptionApplicationData> CancelInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication, string token)
        {
            throw new NotImplementedException();
        }

        public Task<InterceptionApplicationData> CreateInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication, string token)
        {
            throw new NotImplementedException();
        }

        public Task<InterceptionApplicationData> GetApplicationAsync(string dat_Appl_EnfSrvCd, string dat_Appl_CtrlCd, string token)
        {
            throw new NotImplementedException();
        }

        public Task<List<InterceptionApplicationData>> GetApplicationsForVariationAutoAcceptAsync(string enfService, string token)
        {
            throw new NotImplementedException();
        }

        public Task<InterceptionApplicationData> SuspendInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication, string token)
        {
            throw new NotImplementedException();
        }

        public Task<InterceptionApplicationData> TransferInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication, string newRecipientSubmitter, string newIssuingSubmitter, string token)
        {
            throw new NotImplementedException();
        }

        public Task<InterceptionApplicationData> UpdateInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication, string token)
        {
            throw new NotImplementedException();
        }

        public Task<InterceptionApplicationData> ValidateFinancialCoreValuesAsync(InterceptionApplicationData application, string token)
        {
            return Task.FromResult(application);
        }

        public Task<InterceptionApplicationData> VaryInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication, string token)
        {
            throw new NotImplementedException();
        }
    }
}
