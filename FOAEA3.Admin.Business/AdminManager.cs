using FOAEA3.Business.Areas.Application;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FOAEA3.Admin.Business
{
    internal class AdminManager
    {
        private readonly IRepositories DB;
        private readonly CustomConfig config;
        public string LastError { get; set; }

        public AdminManager(IRepositories repositories, CustomConfig config)
        {
            this.config = config;
            DB = repositories;
            LastError = string.Empty;
        }


        public async Task<bool> ManuallyConfirmSINAsync(string enfService, string controlCode, string sin,
                                                        ClaimsPrincipal user)
        {
            // manually move application from state 3 to state 6 by adding SIN confirmation information to database
            // and processing the state as per normal
            bool success = true;

            try
            {
                var applicationData = new ApplicationData();

                var applManager = new ApplicationManager(applicationData, DB, config);
                await applManager.SetCurrentUserAsync(user);

                if (!await applManager.LoadApplicationAsync(enfService, controlCode))
                    throw new Exception($"Application {enfService}-{controlCode} does not exists or could not be loaded.");

                if (applManager.GetState() != ApplicationState.SIN_CONFIRMATION_PENDING_3)
                    throw new Exception($"Application is at invalid state {applManager.GetState()}. It must be at state {ApplicationState.SIN_CONFIRMATION_PENDING_3}!");

                if (!ValidationHelper.IsValidSinNumberMod10(sin))
                    throw new Exception("Invalid SIN!");

                await CloseActiveSinEventAndDetailsForApplicationAsync(applManager);

                await CreateSinResultsAsync(enfService, controlCode, sin);

                switch (applManager.GetCategory())
                {
                    case "I01":
                        break;

                    case "T01":
                        var tracingManager = new TracingManager(DB, config);
                        await tracingManager.LoadApplicationAsync(enfService, controlCode);
                        tracingManager.TracingApplication.Appl_Dbtr_Cnfrmd_SIN = sin;
                        tracingManager.TracingApplication.Appl_SIN_Cnfrmd_Ind = 1;
                        tracingManager.TracingApplication.Appl_LastUpdate_Usr = "SYSTEM";
                        tracingManager.TracingApplication.Appl_LastUpdate_Dte = DateTime.Now;

                        await tracingManager.ApplySINconfirmation();

                        break;

                    case "L01":
                        break;

                    case "L03":
                        break;
                }
            }
            catch (Exception e)
            {
                LastError = e.Message;
                success = false;
            }

            return success;
        }

        private async Task CloseActiveSinEventAndDetailsForApplicationAsync(ApplicationManager applManager)
        {
            var sinEvents = await applManager.EventManager.GetApplicationEventsForQueueAsync(EventQueue.EventSIN);
            var mostRecentEventSIN = sinEvents.OrderByDescending(m => m.Event_TimeStamp)
                                              .First(m => (m.Event_Reas_Cd == EventCode.C50640_SIN_SENT_TO_HRD_FOR_VALIDATION));
            mostRecentEventSIN.ActvSt_Cd = "C";
            mostRecentEventSIN.Event_Compl_Dte = DateTime.Now;
            await applManager.EventManager.SaveEventAsync(mostRecentEventSIN);

            var mostRecentEventSINdetails = await DB.ApplicationEventDetailTable.GetEventSINDetailDataForEventIDAsync(mostRecentEventSIN.Event_Id);
            mostRecentEventSINdetails.ActvSt_Cd = "C";
            mostRecentEventSINdetails.Event_Compl_Dte = DateTime.Now;
            await applManager.EventDetailManager.SaveEventDetailAsync(mostRecentEventSINdetails);

        }

        private async Task CreateSinResultsAsync(string enfService, string controlCode, string sin)
        {
            var sinResult = new SINResultData
            {
                Appl_EnfSrv_Cd = enfService,
                Appl_CtrlCd = controlCode,
                SVR_TimeStamp = DateTime.Now,
                SVR_TolCd = "1",
                SVR_SIN = sin,
                SVR_DOB_TolCd = 1,
                SVR_GvnNme_TolCd = 1,
                SVR_MddlNme_TolCd = 1,
                SVR_SurNme_TolCd = 1,
                SVR_MotherNme_TolCd = 2,
                SVR_Gendr_TolCd = 1,
                ValStat_Cd = 0,
                ActvSt_Cd = "C"
            };
            await DB.SINResultTable.CreateSINResultsAsync(sinResult);
        }


    }
}
