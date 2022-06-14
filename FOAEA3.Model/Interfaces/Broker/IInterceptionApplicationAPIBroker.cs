using FOAEA3.Model.Enums;
using System;
using System.Collections.Generic;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface IInterceptionApplicationAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        InterceptionApplicationData GetApplication(string dat_Appl_EnfSrvCd, string dat_Appl_CtrlCd);
        InterceptionApplicationData CreateInterceptionApplication(InterceptionApplicationData interceptionApplication);
        InterceptionApplicationData UpdateInterceptionApplication(InterceptionApplicationData interceptionApplication);
        InterceptionApplicationData CancelInterceptionApplication(InterceptionApplicationData interceptionApplication);
        InterceptionApplicationData SuspendInterceptionApplication(InterceptionApplicationData interceptionApplication);
        InterceptionApplicationData VaryInterceptionApplication(InterceptionApplicationData interceptionApplication);
        InterceptionApplicationData TransferInterceptionApplication(InterceptionApplicationData interceptionApplication,
                                                                 string newRecipientSubmitter,
                                                                 string newIssuingSubmitter);

        InterceptionApplicationData ValidateFinancialCoreValues(InterceptionApplicationData application);

        List<InterceptionApplicationData> GetApplicationsForVariationAutoAccept(string enfService);
        InterceptionApplicationData AcceptVariation(InterceptionApplicationData interceptionApplication);
    }
}
