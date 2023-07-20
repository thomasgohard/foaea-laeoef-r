using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;

namespace FOAEA3.Common.Brokers
{
    public class InterceptionApplicationAPIBroker : IInterceptionApplicationAPIBroker, IVersionSupport
    {
        public IAPIBrokerHelper ApiHelper { get; }
        public string Token { get; set; }

        public InterceptionApplicationAPIBroker(IAPIBrokerHelper apiHelper, string token = null)
        {
            ApiHelper = apiHelper;
            Token = token;
        }

        public async Task<string> GetVersion()
        {
            string apiCall = $"api/v1/interceptions/Version";
            return await ApiHelper.GetString(apiCall, maxAttempts: 1, token: Token);
        }

        public async Task<string> GetConnection()
        {
            string apiCall = $"api/v1/interceptions/DB";
            return await ApiHelper.GetString(apiCall, maxAttempts: 1, token: Token);
        }

        public async Task<InterceptionApplicationData> GetApplication(string dat_Appl_EnfSrvCd, string dat_Appl_CtrlCd)
        {
            string key = ApplKey.MakeKey(dat_Appl_EnfSrvCd, dat_Appl_CtrlCd);
            string apiCall = $"api/v1/interceptions/{key}";
            return await ApiHelper.GetData<InterceptionApplicationData>(apiCall, token: Token);
        }

        public async Task<SummonsSummaryData> GetSummonsSummaryForApplication(string dat_Appl_EnfSrvCd, string dat_Appl_CtrlCd)
        {
            string key = ApplKey.MakeKey(dat_Appl_EnfSrvCd, dat_Appl_CtrlCd);
            string apiCall = $"api/v1/summonsSummaries/{key}";
            return await ApiHelper.GetData<SummonsSummaryData>(apiCall, token: Token);
        }

        public async Task<InterceptionApplicationData> CreateInterceptionApplication(InterceptionApplicationData interceptionApplication)
        {
            string apiCall = "api/v1/Interceptions";
            var data = await ApiHelper.PostData<InterceptionApplicationData, InterceptionApplicationData>(apiCall,
                                                                                               interceptionApplication, token: Token);
            
            if (ApiHelper.ErrorData.Any() && !data.Messages.Any())
            {
                foreach(var error in ApiHelper.ErrorData)
                    data.Messages.Add(error);
            }

            // IMessageList

            return data;
        }

        public async Task<InterceptionApplicationData> TransferInterceptionApplication(InterceptionApplicationData interceptionApplication, string newRecipientSubmitter, string newIssuingSubmitter)
        {
            string key = ApplKey.MakeKey(interceptionApplication.Appl_EnfSrv_Cd, interceptionApplication.Appl_CtrlCd);
            string apiCall = $"api/v1/interceptions/{key}/transfer?newRecipientSubmitter={newRecipientSubmitter}" +
                                                                 $"&newIssuingSubmitter={newIssuingSubmitter}";
            var data = await ApiHelper.PutData<InterceptionApplicationData, InterceptionApplicationData>(apiCall,
                                                                                               interceptionApplication, token: Token);
            return data;
        }

        public async Task<InterceptionApplicationData> UpdateInterceptionApplication(InterceptionApplicationData interceptionApplication)
        {
            string key = ApplKey.MakeKey(interceptionApplication.Appl_EnfSrv_Cd, interceptionApplication.Appl_CtrlCd);
            string apiCall = $"api/v1/interceptions/{key}";
            var data = await ApiHelper.PutData<InterceptionApplicationData, InterceptionApplicationData>(apiCall,
                                                                                               interceptionApplication, token: Token);
            return data;
        }

        public async Task<InterceptionApplicationData> CancelInterceptionApplication(InterceptionApplicationData interceptionApplication)
        {
            string key = ApplKey.MakeKey(interceptionApplication.Appl_EnfSrv_Cd, interceptionApplication.Appl_CtrlCd);
            string apiCall = $"api/v1/interceptions/{key}/cancel";
            var data = await ApiHelper.PutData<InterceptionApplicationData, InterceptionApplicationData>(apiCall,
                                                                                               interceptionApplication, token: Token);
            return data;
        }

        public async Task<InterceptionApplicationData> SuspendInterceptionApplication(InterceptionApplicationData interceptionApplication)
        {
            string key = ApplKey.MakeKey(interceptionApplication.Appl_EnfSrv_Cd, interceptionApplication.Appl_CtrlCd);
            string apiCall = $"api/v1/interceptions/{key}/suspend";
            var data = await ApiHelper.PutData<InterceptionApplicationData, InterceptionApplicationData>(apiCall,
                                                                                               interceptionApplication, token: Token);
            return data;
        }

        public async Task<InterceptionApplicationData> VaryInterceptionApplication(InterceptionApplicationData interceptionApplication)
        {
            string key = ApplKey.MakeKey(interceptionApplication.Appl_EnfSrv_Cd, interceptionApplication.Appl_CtrlCd);
            string apiCall = $"api/v1/interceptions/{key}/Vary";
            var data = await ApiHelper.PutData<InterceptionApplicationData, InterceptionApplicationData>(apiCall,
                                                                                               interceptionApplication, token: Token);
            return data;
        }

        public async Task<InterceptionApplicationData> ValidateFinancialCoreValues(InterceptionApplicationData application)
        {
            string apiCall = "api/v1/Interceptions/ValidateFinancialCoreValues";
            return await ApiHelper.PutData<InterceptionApplicationData, InterceptionApplicationData>(apiCall, application, token: Token);
        }

        public async Task AutoAcceptVariations(string enfService)
        {
            string apiCall = $"api/v1/interceptions/AutoAcceptVariations?enfService={enfService}";
            await ApiHelper.SendCommand(apiCall, token: Token);
            return;
        }

        public async Task<List<PaymentPeriodData>> GetPaymentPeriods()
        {
            string apiCall = "api/v1/paymentperiods";
            return await ApiHelper.GetData<List<PaymentPeriodData>>(apiCall, token: Token);
        }

        public async Task<InterceptionApplicationData> AcceptVariation(InterceptionApplicationData interceptionApplication)
        {
            string key = ApplKey.MakeKey(interceptionApplication.Appl_EnfSrv_Cd, interceptionApplication.Appl_CtrlCd);
            string apiCall = $"api/v1/interceptions/{key}/AcceptVariation?autoAccept=true";
            var data = await ApiHelper.PutData<InterceptionApplicationData, InterceptionApplicationData>(apiCall,
                                                                                           interceptionApplication, token: Token);
            return data;
        }

        public async Task<bool> ESD_CheckIfAlreadyLoaded(string fileName)
        {
            string apiCall = $"api/v1/ESDs/{fileName}";
            var data = await ApiHelper.GetData<ElectronicSummonsDocumentZipData>(apiCall, token: Token);

            return data.ZipName != null;
        }

        public async Task<ElectronicSummonsDocumentZipData> ESD_Create(int processId, string fileName, DateTime dateReceived)
        {
            var newFile = new ElectronicSummonsDocumentZipData
            {
                PrcID = processId,
                ZipName = fileName,
                DateReceived = dateReceived
            };

            string apiCall = $"api/v1/ESDs";
            var data = await ApiHelper.PostData<ElectronicSummonsDocumentZipData, ElectronicSummonsDocumentZipData>(apiCall, newFile, token: Token);

            return data;
        }

        public async Task<ElectronicSummonsDocumentPdfData> ESDPDF_Create(ElectronicSummonsDocumentPdfData newPdf)
        {
            string apiCall = $"api/v1/ESDPDFs";
            var data = await ApiHelper.PostData<ElectronicSummonsDocumentPdfData, ElectronicSummonsDocumentPdfData>(apiCall, newPdf, token: Token);

            return data;
        }

        public async Task<List<ProcessEISOOUTHistoryData>> GetEISOvalidApplications()
        {
            string apiCall = $"api/v1/EISOrequests/CRA";
            return await ApiHelper.GetData<List<ProcessEISOOUTHistoryData>>(apiCall, token: Token);
        }

        public async Task<List<EIoutgoingFederalData>> GetEIexchangeOutData(string enfSrv)
        {
            string apiCall = $"api/v1/EISOrequests/EI?enfSrv={enfSrv}";
            return await ApiHelper.GetData<List<EIoutgoingFederalData>>(apiCall, token: Token);
        }
    }
}
