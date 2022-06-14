using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Broker;
using FOAEA3.Resources.Helpers;
using System;
using System.Collections.Generic;

namespace FOAEA3.Common.Brokers
{
    public class InterceptionApplicationAPIBroker : IInterceptionApplicationAPIBroker
    {
        public IAPIBrokerHelper ApiHelper { get; }

        public InterceptionApplicationAPIBroker(IAPIBrokerHelper apiHelper)
        {
            ApiHelper = apiHelper;
        }

        public InterceptionApplicationData GetApplication(string dat_Appl_EnfSrvCd, string dat_Appl_CtrlCd)
        {
            string key = ApplKey.MakeKey(dat_Appl_EnfSrvCd, dat_Appl_CtrlCd);
            string apiCall = $"api/v1/interceptions/{key}";
            return ApiHelper.GetDataAsync<InterceptionApplicationData>(apiCall).Result;
        }

        public InterceptionApplicationData CreateInterceptionApplication(InterceptionApplicationData interceptionApplication)
        {
            string apiCall = "api/v1/Interceptions";
            var data = ApiHelper.PostDataAsync<InterceptionApplicationData, InterceptionApplicationData>(apiCall,
                                                                                               interceptionApplication).Result;
            return data;
        }

        public InterceptionApplicationData TransferInterceptionApplication(InterceptionApplicationData interceptionApplication, string newRecipientSubmitter, string newIssuingSubmitter)
        {
            string key = ApplKey.MakeKey(interceptionApplication.Appl_EnfSrv_Cd, interceptionApplication.Appl_CtrlCd);
            string apiCall = $"api/v1/interceptions/{key}/transfer?newRecipientSubmitter={newRecipientSubmitter}" +
                                                                 $"&newIssuingSubmitter={newIssuingSubmitter}";
            var data = ApiHelper.PutDataAsync<InterceptionApplicationData, InterceptionApplicationData>(apiCall,
                                                                                               interceptionApplication).Result;
            return data;
        }

        public InterceptionApplicationData UpdateInterceptionApplication(InterceptionApplicationData interceptionApplication)
        {
            string key = ApplKey.MakeKey(interceptionApplication.Appl_EnfSrv_Cd, interceptionApplication.Appl_CtrlCd);
            string apiCall = $"api/v1/interceptions/{key}";
            var data = ApiHelper.PutDataAsync<InterceptionApplicationData, InterceptionApplicationData>(apiCall,
                                                                                               interceptionApplication).Result;
            return data;
        }

        public InterceptionApplicationData CancelInterceptionApplication(InterceptionApplicationData interceptionApplication)
        {
            string key = ApplKey.MakeKey(interceptionApplication.Appl_EnfSrv_Cd, interceptionApplication.Appl_CtrlCd);
            string apiCall = $"api/v1/interceptions/{key}/cancel";
            var data = ApiHelper.PutDataAsync<InterceptionApplicationData, InterceptionApplicationData>(apiCall,
                                                                                               interceptionApplication).Result;
            return data;
        }

        public InterceptionApplicationData SuspendInterceptionApplication(InterceptionApplicationData interceptionApplication)
        {
            string key = ApplKey.MakeKey(interceptionApplication.Appl_EnfSrv_Cd, interceptionApplication.Appl_CtrlCd);
            string apiCall = $"api/v1/interceptions/{key}/suspend";
            var data = ApiHelper.PutDataAsync<InterceptionApplicationData, InterceptionApplicationData>(apiCall,
                                                                                               interceptionApplication).Result;
            return data;
        }

        public InterceptionApplicationData VaryInterceptionApplication(InterceptionApplicationData interceptionApplication)
        {
            string key = ApplKey.MakeKey(interceptionApplication.Appl_EnfSrv_Cd, interceptionApplication.Appl_CtrlCd);
            string apiCall = $"api/v1/interceptions/{key}/Vary";
            var data = ApiHelper.PutDataAsync<InterceptionApplicationData, InterceptionApplicationData>(apiCall,
                                                                                               interceptionApplication).Result;
            return data;
        }

        public InterceptionApplicationData ValidateFinancialCoreValues(InterceptionApplicationData application)
        {
            string apiCall = "api/v1/Interceptions/ValidateFinancialCoreValues";
            return ApiHelper.PutDataAsync<InterceptionApplicationData, InterceptionApplicationData>(apiCall, application).Result;
        }

        public List<InterceptionApplicationData> GetApplicationsForVariationAutoAccept(string enfService)
        {
            string apiCall = $"api/v1/interceptions/GetApplicationsForVariationAutoAccept?enfService={enfService}";
            return ApiHelper.GetDataAsync<List<InterceptionApplicationData>>(apiCall).Result;
        }

        public InterceptionApplicationData AcceptVariation(InterceptionApplicationData interceptionApplication)
        {
            string key = ApplKey.MakeKey(interceptionApplication.Appl_EnfSrv_Cd, interceptionApplication.Appl_CtrlCd);
            string apiCall = $"api/v1/interceptions/{key}/AcceptVariation?autoAccept=true";
            var data = ApiHelper.PutDataAsync<InterceptionApplicationData, InterceptionApplicationData>(apiCall,
                                                                                           interceptionApplication).Result;
            return data;
        }
    }
}
