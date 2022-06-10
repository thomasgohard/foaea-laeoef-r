using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;
using System;
using System.Collections.Generic;

namespace FileBroker.Business.Tests.InMemory
{
    public class InMemoryInterceptionApplicationAPIBroker : IInterceptionApplicationAPIBroker
    {
        public IAPIBrokerHelper ApiHelper => throw new NotImplementedException();

        public InterceptionApplicationData AcceptVariation(InterceptionApplicationData interceptionApplication)
        {
            throw new NotImplementedException();
        }

        public InterceptionApplicationData CreateInterceptionApplication(InterceptionApplicationData interceptionApplication)
        {
            throw new NotImplementedException();
        }

        public InterceptionApplicationData GetApplication(string dat_Appl_EnfSrvCd, string dat_Appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public List<InterceptionApplicationData> GetApplicationsForVariationAutoAccept(string enfService)
        {
            throw new NotImplementedException();
        }

        public InterceptionApplicationData TransferInterceptionApplication(InterceptionApplicationData interceptionApplication, string newRecipientSubmitter, string newIssuingSubmitter)
        {
            throw new NotImplementedException();
        }

        public InterceptionApplicationData UpdateInterceptionApplication(InterceptionApplicationData interceptionApplication)
        {
            throw new NotImplementedException();
        }

        public InterceptionApplicationData ValidateFinancialCoreValues(InterceptionApplicationData application)
        {
            throw new NotImplementedException();
        }

        public InterceptionApplicationData VaryInterceptionApplication(InterceptionApplicationData interceptionApplication)
        {
            throw new NotImplementedException();
        }
    }
}
