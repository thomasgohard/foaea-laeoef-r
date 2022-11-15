using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface IInterceptionApplicationAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }


        Task<InterceptionApplicationData> GetApplicationAsync(string dat_Appl_EnfSrvCd, string dat_Appl_CtrlCd);
        Task<InterceptionApplicationData> CreateInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication);
        Task<InterceptionApplicationData> UpdateInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication);
        Task<InterceptionApplicationData> CancelInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication);
        Task<InterceptionApplicationData> SuspendInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication);
        Task<InterceptionApplicationData> VaryInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication);
        Task<InterceptionApplicationData> TransferInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication,
                                                                             string newRecipientSubmitter,
                                                                             string newIssuingSubmitter);

        Task<InterceptionApplicationData> ValidateFinancialCoreValuesAsync(InterceptionApplicationData application);

        Task<List<InterceptionApplicationData>> GetApplicationsForVariationAutoAcceptAsync(string enfService);
        Task<InterceptionApplicationData> AcceptVariationAsync(InterceptionApplicationData interceptionApplication);

        Task<bool> ESD_CheckIfAlreadyLoaded(string fileName);
    }
}
