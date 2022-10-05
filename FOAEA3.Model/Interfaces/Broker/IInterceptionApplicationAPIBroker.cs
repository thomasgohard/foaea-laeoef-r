using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface IInterceptionApplicationAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }

        Task<InterceptionApplicationData> GetApplicationAsync(string dat_Appl_EnfSrvCd, string dat_Appl_CtrlCd, string token);
        Task<InterceptionApplicationData> CreateInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication, string token);
        Task<InterceptionApplicationData> UpdateInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication, string token);
        Task<InterceptionApplicationData> CancelInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication, string token);
        Task<InterceptionApplicationData> SuspendInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication, string token);
        Task<InterceptionApplicationData> VaryInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication, string token);
        Task<InterceptionApplicationData> TransferInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication,
                                                                 string newRecipientSubmitter,
                                                                 string newIssuingSubmitter, string token);

        Task<InterceptionApplicationData> ValidateFinancialCoreValuesAsync(InterceptionApplicationData application, string token);

        Task<List<InterceptionApplicationData>> GetApplicationsForVariationAutoAcceptAsync(string enfService, string token);
        Task<InterceptionApplicationData> AcceptVariationAsync(InterceptionApplicationData interceptionApplication, string token);
    }
}
