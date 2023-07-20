using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Model.Interfaces.Broker
{
    public interface IInterceptionApplicationAPIBroker
    {
        IAPIBrokerHelper ApiHelper { get; }
        string Token { get; set; }


        Task<InterceptionApplicationData> GetApplication(string dat_Appl_EnfSrvCd, string dat_Appl_CtrlCd);
        Task<SummonsSummaryData> GetSummonsSummaryForApplication(string dat_Appl_EnfSrvCd, string dat_Appl_CtrlCd);
        Task<InterceptionApplicationData> CreateInterceptionApplication(InterceptionApplicationData interceptionApplication);
        Task<InterceptionApplicationData> UpdateInterceptionApplication(InterceptionApplicationData interceptionApplication);
        Task<InterceptionApplicationData> CancelInterceptionApplication(InterceptionApplicationData interceptionApplication);
        Task<InterceptionApplicationData> SuspendInterceptionApplication(InterceptionApplicationData interceptionApplication);
        Task<InterceptionApplicationData> VaryInterceptionApplication(InterceptionApplicationData interceptionApplication);
        Task<InterceptionApplicationData> TransferInterceptionApplication(InterceptionApplicationData interceptionApplication,
                                                                             string newRecipientSubmitter,
                                                                             string newIssuingSubmitter);

        Task<InterceptionApplicationData> ValidateFinancialCoreValues(InterceptionApplicationData application);

        Task AutoAcceptVariations(string enfService);
        Task<InterceptionApplicationData> AcceptVariation(InterceptionApplicationData interceptionApplication);
        Task<List<PaymentPeriodData>> GetPaymentPeriods();

        Task<bool> ESD_CheckIfAlreadyLoaded(string fileName);
        Task<ElectronicSummonsDocumentZipData> ESD_Create(int processId, string fileName, DateTime dateReceived);
        Task<ElectronicSummonsDocumentPdfData> ESDPDF_Create(ElectronicSummonsDocumentPdfData newPdf);

        Task<List<ProcessEISOOUTHistoryData>> GetEISOvalidApplications();
        Task<List<EIoutgoingFederalData>> GetEIexchangeOutData(string enfSrv);
    }
}
