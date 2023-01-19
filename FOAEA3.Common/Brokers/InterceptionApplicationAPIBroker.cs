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

        public async Task<string> GetVersionAsync()
        {
            string apiCall = $"api/v1/interceptions/Version";
            return await ApiHelper.GetStringAsync(apiCall, maxAttempts: 1, token: Token);
        }

        public async Task<string> GetConnectionAsync()
        {
            string apiCall = $"api/v1/interceptions/DB";
            return await ApiHelper.GetStringAsync(apiCall, maxAttempts: 1, token: Token);
        }

        public async Task<InterceptionApplicationData> GetApplicationAsync(string dat_Appl_EnfSrvCd, string dat_Appl_CtrlCd)
        {
            string key = ApplKey.MakeKey(dat_Appl_EnfSrvCd, dat_Appl_CtrlCd);
            string apiCall = $"api/v1/interceptions/{key}";
            return await ApiHelper.GetDataAsync<InterceptionApplicationData>(apiCall, token: Token);
        }

        public async Task<InterceptionApplicationData> CreateInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication)
        {
            string apiCall = "api/v1/Interceptions";
            var data = await ApiHelper.PostDataAsync<InterceptionApplicationData, InterceptionApplicationData>(apiCall,
                                                                                               interceptionApplication, token: Token);
            
            if (ApiHelper.ErrorData.Any() && !data.Messages.Any())
            {
                foreach(var error in ApiHelper.ErrorData)
                    data.Messages.Add(error);
            }

            // IMessageList

            return data;
        }

        public async Task<InterceptionApplicationData> TransferInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication, string newRecipientSubmitter, string newIssuingSubmitter)
        {
            string key = ApplKey.MakeKey(interceptionApplication.Appl_EnfSrv_Cd, interceptionApplication.Appl_CtrlCd);
            string apiCall = $"api/v1/interceptions/{key}/transfer?newRecipientSubmitter={newRecipientSubmitter}" +
                                                                 $"&newIssuingSubmitter={newIssuingSubmitter}";
            var data = await ApiHelper.PutDataAsync<InterceptionApplicationData, InterceptionApplicationData>(apiCall,
                                                                                               interceptionApplication, token: Token);
            return data;
        }

        public async Task<InterceptionApplicationData> UpdateInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication)
        {
            string key = ApplKey.MakeKey(interceptionApplication.Appl_EnfSrv_Cd, interceptionApplication.Appl_CtrlCd);
            string apiCall = $"api/v1/interceptions/{key}";
            var data = await ApiHelper.PutDataAsync<InterceptionApplicationData, InterceptionApplicationData>(apiCall,
                                                                                               interceptionApplication, token: Token);
            return data;
        }

        public async Task<InterceptionApplicationData> CancelInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication)
        {
            string key = ApplKey.MakeKey(interceptionApplication.Appl_EnfSrv_Cd, interceptionApplication.Appl_CtrlCd);
            string apiCall = $"api/v1/interceptions/{key}/cancel";
            var data = await ApiHelper.PutDataAsync<InterceptionApplicationData, InterceptionApplicationData>(apiCall,
                                                                                               interceptionApplication, token: Token);
            return data;
        }

        public async Task<InterceptionApplicationData> SuspendInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication)
        {
            string key = ApplKey.MakeKey(interceptionApplication.Appl_EnfSrv_Cd, interceptionApplication.Appl_CtrlCd);
            string apiCall = $"api/v1/interceptions/{key}/suspend";
            var data = await ApiHelper.PutDataAsync<InterceptionApplicationData, InterceptionApplicationData>(apiCall,
                                                                                               interceptionApplication, token: Token);
            return data;
        }

        public async Task<InterceptionApplicationData> VaryInterceptionApplicationAsync(InterceptionApplicationData interceptionApplication)
        {
            string key = ApplKey.MakeKey(interceptionApplication.Appl_EnfSrv_Cd, interceptionApplication.Appl_CtrlCd);
            string apiCall = $"api/v1/interceptions/{key}/Vary";
            var data = await ApiHelper.PutDataAsync<InterceptionApplicationData, InterceptionApplicationData>(apiCall,
                                                                                               interceptionApplication, token: Token);
            return data;
        }

        public async Task<InterceptionApplicationData> ValidateFinancialCoreValuesAsync(InterceptionApplicationData application)
        {
            string apiCall = "api/v1/Interceptions/ValidateFinancialCoreValues";
            return await ApiHelper.PutDataAsync<InterceptionApplicationData, InterceptionApplicationData>(apiCall, application, token: Token);
        }

        public async Task AutoAcceptVariationsAsync(string enfService)
        {
            string apiCall = $"api/v1/interceptions/AutoAcceptVariations?enfService={enfService}";
            await ApiHelper.SendCommandAsync(apiCall, token: Token);
            return;
        }

        public async Task<InterceptionApplicationData> AcceptVariationAsync(InterceptionApplicationData interceptionApplication)
        {
            string key = ApplKey.MakeKey(interceptionApplication.Appl_EnfSrv_Cd, interceptionApplication.Appl_CtrlCd);
            string apiCall = $"api/v1/interceptions/{key}/AcceptVariation?autoAccept=true";
            var data = await ApiHelper.PutDataAsync<InterceptionApplicationData, InterceptionApplicationData>(apiCall,
                                                                                           interceptionApplication, token: Token);
            return data;
        }

        public async Task<bool> ESD_CheckIfAlreadyLoaded(string fileName)
        {
            string apiCall = $"api/v1/ESDs/{fileName}";
            var data = await ApiHelper.GetDataAsync<ElectronicSummonsDocumentZipData>(apiCall, token: Token);

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
            var data = await ApiHelper.PostDataAsync<ElectronicSummonsDocumentZipData, ElectronicSummonsDocumentZipData>(apiCall, newFile, token: Token);

            return data;
        }

        public async Task<ElectronicSummonsDocumentPdfData> ESDPDF_Create(ElectronicSummonsDocumentPdfData newPdf)
        {
            string apiCall = $"api/v1/ESDPDFs";
            var data = await ApiHelper.PostDataAsync<ElectronicSummonsDocumentPdfData, ElectronicSummonsDocumentPdfData>(apiCall, newPdf, token: Token);

            return data;
        }

        public async Task<List<ProcessEISOOUTHistoryData>> GetEISOvalidApplications()
        {
            string apiCall = $"api/v1/EISOrequests/CRA";
            return await ApiHelper.GetDataAsync<List<ProcessEISOOUTHistoryData>>(apiCall, token: Token);
        }

        public async Task<List<EIoutgoingFederalData>> GetEIexchangeOutData(string enfSrv)
        {
            string apiCall = $"api/v1/EISOrequests/EI?enfSrv={enfSrv}";
            return await ApiHelper.GetDataAsync<List<EIoutgoingFederalData>>(apiCall, token: Token);
        }
    }
}
