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

        public Task<InterceptionApplicationData> AcceptVariation(InterceptionApplicationData interceptionApplication)
        {
            throw new NotImplementedException();
        }

        public Task AutoAcceptVariations(string enfService)
        {
            throw new NotImplementedException();
        }

        public Task<InterceptionApplicationData> CancelInterceptionApplication(InterceptionApplicationData interceptionApplication)
        {
            throw new NotImplementedException();
        }

        public Task<InterceptionApplicationData> CreateInterceptionApplication(InterceptionApplicationData interceptionApplication)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeleteEISOhistoryForSIN(string oldSIN)
        {
            throw new NotImplementedException();
        }

        public Task<ElectronicSummonsDocumentPdfData> ESDPDF_Create(ElectronicSummonsDocumentPdfData newPdf)
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

        public Task<string> FixDebtorIdForSin(string newSIN)
        {
            throw new NotImplementedException();
        }

        public Task<InterceptionApplicationData> GetApplication(string dat_Appl_EnfSrvCd, string dat_Appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task<List<InterceptionApplicationData>> GetApplicationsForVariationAutoAccept(string enfService)
        {
            throw new NotImplementedException();
        }

        public Task<List<EIoutgoingFederalData>> GetEIexchangeOutData(string enfSrv)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProcessEISOOUTHistoryData>> GetEISOvalidApplications()
        {
            throw new NotImplementedException();
        }

        public Task<List<PaymentPeriodData>> GetPaymentPeriods()
        {
            throw new NotImplementedException();
        }

        public Task<SummonsSummaryData> GetSummonsSummaryForApplication(string dat_Appl_EnfSrvCd, string dat_Appl_CtrlCd)
        {
            throw new NotImplementedException();
        }

        public Task<InterceptionApplicationData> SuspendInterceptionApplication(InterceptionApplicationData interceptionApplication)
        {
            throw new NotImplementedException();
        }

        public Task<InterceptionApplicationData> TransferInterceptionApplication(InterceptionApplicationData interceptionApplication, string newRecipientSubmitter, string newIssuingSubmitter)
        {
            throw new NotImplementedException();
        }

        public Task<InterceptionApplicationData> UpdateInterceptionApplication(InterceptionApplicationData interceptionApplication)
        {
            throw new NotImplementedException();
        }

        public Task<InterceptionApplicationData> ValidateFinancialCoreValues(InterceptionApplicationData application)
        {
            return Task.FromResult(application);
        }

        public Task<InterceptionApplicationData> VaryInterceptionApplication(InterceptionApplicationData interceptionApplication)
        {
            throw new NotImplementedException();
        }
    }
}
