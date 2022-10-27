using DBHelper;
using FOAEA3.Business.Security;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Business.Areas.Application
{
    internal partial class LicenceDenialManager : ApplicationManager
    {
        public LicenceDenialApplicationData LicenceDenialApplication { get; }
        private bool AffidavitExists() => !String.IsNullOrEmpty(LicenceDenialApplication.Appl_Crdtr_FrstNme);

        public LicenceDenialManager(LicenceDenialApplicationData licenceDenial, IRepositories repositories, CustomConfig config) : base(licenceDenial, repositories, config)
        {
            LicenceDenialApplication = licenceDenial;

            // add Licence Denial specific state changes
            StateEngine.ValidStateChange.Add(ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7, new List<ApplicationState> {
                            ApplicationState.SIN_CONFIRMED_4,
                            ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7,
                            ApplicationState.APPLICATION_REJECTED_9,
                            ApplicationState.APPLICATION_ACCEPTED_10,
                            ApplicationState.MANUALLY_TERMINATED_14
                        });

            //            StateEngine.ValidStateChange[ApplicationState.PENDING_ACCEPTANCE_SWEARING_6].Add(ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7);
            StateEngine.ValidStateChange[ApplicationState.SIN_CONFIRMED_4].Add(ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7);
        }

        public async Task<List<LicenceDenialOutgoingProvincialData>> GetProvincialOutgoingDataAsync(int maxRecords, string activeState, string recipientCode, bool isXML)
        {
            var licenceDenialDB = DB.LicenceDenialTable;
            var data = await licenceDenialDB.GetProvincialOutgoingDataAsync(maxRecords, activeState, recipientCode, isXML);
            return data;
        }

        public LicenceDenialManager(IRepositories repositories, CustomConfig config) : this(new LicenceDenialApplicationData(), repositories, config)
        {

        }

        public override async Task<bool> LoadApplicationAsync(string enfService, string controlCode)
        {
            // get data from Appl
            bool isSuccess = await base.LoadApplicationAsync(enfService, controlCode);

            if (isSuccess)
            {
                // get additional data from LicSusp table 
                var licenceDenialDB = DB.LicenceDenialTable;
                var data = await licenceDenialDB.GetLicenceDenialDataAsync(enfService, appl_L01_CtrlCd: controlCode);

                if (data != null)
                    LicenceDenialApplication.Merge(data);
            }

            return isSuccess;
        }

        public override async Task<bool> CreateApplicationAsync()
        {
            if (!IsValidCategory("L01"))
                return false;

            bool success = await base.CreateApplicationAsync();

            if (!success)
            {
                var failedSubmitterManager = new FailedSubmitAuditManager(DB, LicenceDenialApplication);
                await failedSubmitterManager.AddToFailedSubmitAuditAsync(FailedSubmitActivityAreaType.L01);
            }

            await DB.LicenceDenialTable.CreateLicenceDenialDataAsync(LicenceDenialApplication);

            return success;
        }

        public async Task<List<LicenceSuspensionHistoryData>> GetLicenceSuspensionHistoryAsync()
        {
            return await DB.LicenceDenialTable.GetLicenceSuspensionHistoryAsync(LicenceDenialApplication.Appl_EnfSrv_Cd, LicenceDenialApplication.Appl_CtrlCd);
        }

        public async Task CancelApplicationAsync()
        {
            LicenceDenialApplication.Appl_LastUpdate_Dte = DateTime.Now;
            LicenceDenialApplication.Appl_LastUpdate_Usr = DB.CurrentSubmitter;

            await SetNewStateTo(ApplicationState.MANUALLY_TERMINATED_14);

            MakeUpperCase();
            await UpdateApplicationNoValidationAsync();

            await DB.LicenceDenialTable.UpdateLicenceDenialDataAsync(LicenceDenialApplication);

            EventManager.AddEvent(EventCode.C50843_APPLICATION_CANCELLED);

            await EventManager.SaveEventsAsync();
        }

        public override async Task UpdateApplicationAsync()
        {
            var current = new LicenceDenialManager(DB, config)
            {
                CurrentUser = this.CurrentUser
            };
            await current.LoadApplicationAsync(Appl_EnfSrv_Cd, Appl_CtrlCd);

            // keep these stored values
            LicenceDenialApplication.Appl_Create_Dte = current.LicenceDenialApplication.Appl_Create_Dte;
            LicenceDenialApplication.Appl_Create_Usr = current.LicenceDenialApplication.Appl_Create_Usr;

            await base.UpdateApplicationAsync();
        }

        public async Task<bool> ProcessLicenceDenialResponseAsync(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            if (!await LoadApplicationAsync(appl_EnfSrv_Cd, appl_CtrlCd))
            {
                LicenceDenialApplication.Messages.AddError(SystemMessage.APPLICATION_NOT_FOUND);
                return false;
            }

            if (!IsValidCategory("L01"))
                return false;

            if (LicenceDenialApplication.AppLiSt_Cd.NotIn(ApplicationState.APPLICATION_ACCEPTED_10, ApplicationState.PARTIALLY_SERVICED_12))
            {
                LicenceDenialApplication.Messages.AddError("Invalid State for the current application.  Valid states allowed are 10 and 12.");
                return false;
            }

            LicenceDenialApplication.Appl_LastUpdate_Dte = DateTime.Now;
            LicenceDenialApplication.Appl_LastUpdate_Usr = DB.CurrentSubmitter;

            await SetNewStateTo(ApplicationState.PARTIALLY_SERVICED_12);

            await UpdateApplicationNoValidationAsync();

            await DB.LicenceDenialTable.UpdateLicenceDenialDataAsync(LicenceDenialApplication);

            await EventManager.SaveEventsAsync();

            return true;
        }

        public async Task<List<ApplicationEventData>> GetRequestedLICINLicenceDenialEventsAsync(string enfSrv_Cd, string appl_EnfSrv_Cd,
                                                                               string appl_CtrlCd)
        {
            return await EventManager.GetRequestedLICINLicenceDenialEventsAsync(enfSrv_Cd, appl_EnfSrv_Cd, appl_CtrlCd);
        }

        public async Task<List<ApplicationEventDetailData>> GetRequestedLICINLicenceDenialEventDetailsAsync(string enfSrv_Cd, string appl_EnfSrv_Cd,
                                                                               string appl_CtrlCd)
        {
            return await EventDetailManager.GetRequestedLICINLicenceDenialEventDetailsAsync(enfSrv_Cd, appl_EnfSrv_Cd, appl_CtrlCd);
        }

        public async Task<List<LicenceDenialOutgoingFederalData>> GetFederalOutgoingDataAsync(int maxRecords,
                                                                      string activeState,
                                                                      ApplicationState lifeState,
                                                                      string enfServiceCode)
        {
            var licenceDenialDB = DB.LicenceDenialTable;
            return await licenceDenialDB.GetFederalOutgoingDataAsync(maxRecords, activeState, lifeState, enfServiceCode);
        }

        public async Task CreateResponseDataAsync(List<LicenceDenialResponseData> responseData)
        {
            var responsesDB = DB.LicenceDenialResponseTable;
            await responsesDB.InsertBulkDataAsync(responseData);
        }

        public async Task MarkResponsesAsViewedAsync(string enfService)
        {
            var responsesDB = DB.LicenceDenialResponseTable;
            await responsesDB.MarkResponsesAsViewedAsync(enfService);
        }

        public override async Task ProcessBringForwardsAsync(ApplicationEventData bfEvent)
        {
            bool closeEvent = false;

            TimeSpan diff;
            if (LicenceDenialApplication.Appl_LastUpdate_Dte.HasValue)
                diff = LicenceDenialApplication.Appl_LastUpdate_Dte.Value - DateTime.Now;
            else
                diff = TimeSpan.Zero;

            if ((LicenceDenialApplication.ActvSt_Cd != "A") &&
                ((!bfEvent.Event_Reas_Cd.HasValue) || (
                 (bfEvent.Event_Reas_Cd.NotIn(EventCode.C50806_SCHEDULED_TO_BE_REINSTATED__QUARTERLY_TRACING,
                                              EventCode.C50680_CHANGE_OR_SUPPLY_ADDITIONAL_DEBTOR_INFORMATION_SEE_SIN_VERIFICATION_RESULTS_PAGE_IN_FOAEA_FOR_SPECIFIC_DETAILS,
                                              EventCode.C50600_INVALID_APPLICATION)))) &&
                ((diff.Equals(TimeSpan.Zero)) || (Math.Abs(diff.TotalHours) > 24)))
            {
                bfEvent.AppLiSt_Cd = ApplicationState.MANUALLY_TERMINATED_14;
                bfEvent.ActvSt_Cd = "I";

                await EventManager.SaveEventAsync(bfEvent);

                closeEvent = false;
            }
            else
            {
                if (bfEvent.Event_Reas_Cd.HasValue)
                    switch (bfEvent.Event_Reas_Cd)
                    {
                        case EventCode.C50528_BF_10_DAYS_FROM_RECEIPT_OF_APPLICATION:
                            if (LicenceDenialApplication.AppLiSt_Cd >= ApplicationState.APPLICATION_REJECTED_9)
                            {
                                // no action required
                            }
                            else
                            {
                                var DaysElapsed = (DateTime.Now - LicenceDenialApplication.Appl_Lgl_Dte).TotalDays;
                                if (LicenceDenialApplication.AppLiSt_Cd.In(ApplicationState.INVALID_APPLICATION_1, ApplicationState.SIN_NOT_CONFIRMED_5) &&
                                   (Math.Abs(DaysElapsed) > 40))
                                {
                                    // Reject the application according to the Application category code
                                    // .RejectT01(app.Appl.Item(0).Appl_EnfSrv_Cd, app.Appl.Item(0).Appl_CtrlCd, Appl_LastUpdate_Subm)
                                    EventManager.AddEvent(EventCode.C50760_APPLICATION_REJECTED_AS_CONDITIONS_NOT_MET_IN_TIMEFRAME);
                                }
                                else if ((LicenceDenialApplication.AppLiSt_Cd < ApplicationState.APPLICATION_REJECTED_9)
                                    && (DaysElapsed > 90))
                                {
                                    // Reject the application according to the Application category code
                                    EventManager.AddEvent(EventCode.C50760_APPLICATION_REJECTED_AS_CONDITIONS_NOT_MET_IN_TIMEFRAME);
                                }
                                else
                                {
                                    if (LicenceDenialApplication.AppLiSt_Cd.In(ApplicationState.INVALID_APPLICATION_1, ApplicationState.SIN_NOT_CONFIRMED_5))
                                    {
                                        //Event_Reas_Cd = 50902 'Awaiting an action on this application
                                        //dteDateForNextBF = DateAdd(DateInterval.Day, 10, dteDateForNextBF)
                                    }
                                    else if (LicenceDenialApplication.AppLiSt_Cd.In(ApplicationState.PENDING_ACCEPTANCE_SWEARING_6, ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7))
                                    {
                                        //Select Case Current_LifeState
                                        //    Case 6
                                        //        Event_Reas_Cd = 51042 'Requires legal authorization
                                        //    Case 7
                                        //        Event_Reas_Cd = 54004 'Awaiting affidavit to be entered
                                        //End Select
                                        //dteDateForNextBF = DateAdd(DateInterval.Day, 20, dteDateForNextBF)
                                    }
                                    else
                                    {
                                        //Event_Reas_Cd = 50902 'Awaiting an action on this application
                                        //dteDateForNextBF = DateAdd(DateInterval.Day, 10, dteDateForNextBF)
                                        //.CreateEvent("EvntAm", Subm_SubmCd, Appl_EnfSrv_Cd, Appl_CtrlCd, Now, Subm_Recpt_SubmCd, #12:00:00 AM#, 51100, "", "N", #12:00:00 AM#, "A", AppList_Cd, "")
                                    }
                                    //.CreateEvent("EvntSubm", Subm_SubmCd, Appl_EnfSrv_Cd, Appl_CtrlCd, Now, Subm_Recpt_SubmCd, #12:00:00 AM#, Event_Reas_Cd, "", "N", #12:00:00 AM#, "A", AppList_Cd, Appl_LastUpdate_Subm)
                                    //.CreateEvent("EvntBF", Subm_SubmCd, Appl_EnfSrv_Cd, Appl_CtrlCd, Now, Subm_Recpt_SubmCd, #12:00:00 AM#, 50528, "", "N", dteDateForNextBF, "A", AppList_Cd, "")

                                }
                            }
                            break;

                        case EventCode.C54001_BF_EVENT:
                        case EventCode.C54002_TELEPHONE_EVENT:
                            EventManager.AddEvent(bfEvent.Event_Reas_Cd.Value);
                            break;

                        default:
                            EventManager.AddEvent(EventCode.C54003_UNKNOWN_EVNTBF, queue: EventQueue.EventSYS);
                            break;
                    }
            }

            if (closeEvent)
            {
                bfEvent.AppLiSt_Cd = LicenceDenialApplication.AppLiSt_Cd;
                bfEvent.ActvSt_Cd = "C";

                await EventManager.SaveEventAsync(bfEvent);
            }

            await EventManager.SaveEventsAsync();
        }

        public async Task<List<LicenceDenialToApplData>> GetLicenceDenialToApplDataAsync(string federalSource)
        {
            var licenceDenialDB = DB.LicenceDenialTable;
            return await licenceDenialDB.GetLicenceDenialToApplDataAsync(federalSource);
        }
    }
}
