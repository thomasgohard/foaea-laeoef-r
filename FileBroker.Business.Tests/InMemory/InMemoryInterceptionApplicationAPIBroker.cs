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

        public string Token { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Task<InterceptionApplicationData> AcceptVariationAsync(InterceptionApplicationData interceptionApplication)
        {
            throw new NotImplementedException();
        }

        public Task<InterceptionApplicationData> CancelInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication)
        {
            throw new NotImplementedException();
        }

        public Task<InterceptionApplicationData> CreateInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ESD_CheckIfAlreadyLoaded(string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<ElectronicSummonsDocumentZipData> ESD_Create(int processId, string fileName, DateTime dateReceived)
        {
            throw new NotImplementedException();
        }

        public Task<InterceptionApplicationData> GetApplicationAsync(string dat_Appl_EnfSrvCd, string dat_Appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task<List<InterceptionApplicationData>> GetApplicationsForVariationAutoAcceptAsync(string enfService)
        {
            throw new NotImplementedException();
        }

        public Task<InterceptionApplicationData> SuspendInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication)
        {
            throw new NotImplementedException();
        }

        public Task<InterceptionApplicationData> TransferInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication, string newRecipientSubmitter, string newIssuingSubmitter)
        {
            throw new NotImplementedException();
        }

        public Task<InterceptionApplicationData> UpdateInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication)
        {
            throw new NotImplementedException();
        }

        public Task<InterceptionApplicationData> ValidateFinancialCoreValuesAsync(InterceptionApplicationData application)
        {
            return Task.FromResult(application);
        }

        public Task<InterceptionApplicationData> VaryInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication)
        {
            throw new NotImplementedException();
        }
    }
}
