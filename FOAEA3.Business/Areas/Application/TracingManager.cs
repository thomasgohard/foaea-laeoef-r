using DBHelper;
using FOAEA3.Business.Security;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Repository;
using FOAEA3.Resources.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Business.Areas.Application
{
    internal partial class TracingManager : ApplicationManager
    {
        public TracingApplicationData TracingApplication { get; }
        private DateTime ReinstateEffectiveDate { get; set; }
        private EventCode BFEventReasonCode { get; set; }
        private int BFEvent_Id { get; set; }

        public TracingManager(TracingApplicationData tracing, IRepositories repositories, IFoaeaConfigurationHelper config) :
            base(tracing, repositories, config, new TracingValidation(tracing, repositories, config, null))
        {

            TracingApplication = tracing;

            // add Tracing specific valid state changes
            StateEngine.ValidStateChange.Add(ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7, new List<ApplicationState> {
                                             ApplicationState.SIN_CONFIRMED_4,
                                             ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7,
                                             ApplicationState.APPLICATION_REJECTED_9,
                                             ApplicationState.APPLICATION_ACCEPTED_10,
                                             ApplicationState.MANUALLY_TERMINATED_14
                                            });
            StateEngine.ValidStateChange[ApplicationState.APPLICATION_ACCEPTED_10].Add(ApplicationState.APPLICATION_REINSTATED_11);
            StateEngine.ValidStateChange[ApplicationState.PARTIALLY_SERVICED_12].Add(ApplicationState.APPLICATION_REINSTATED_11);
            //            StateEngine.ValidStateChange[ApplicationState.PENDING_ACCEPTANCE_SWEARING_6].Add(ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7);
            StateEngine.ValidStateChange[ApplicationState.SIN_CONFIRMED_4].Add(ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7);

        }

        public TracingManager(IRepositories repositories, IFoaeaConfigurationHelper config) :
            this(new TracingApplicationData(), repositories, config)
        {
        }

        public override async Task<bool> LoadApplicationAsync(string enfService, string controlCode)
        {
            // get data from Appl
            bool isSuccess = await base.LoadApplicationAsync(enfService, controlCode);

            if (isSuccess)
            {
                // get additional data from Trace table, if available
                TracingApplicationData data = await DB.TracingTable.GetTracingDataAsync(enfService, controlCode);

                if (data != null)
                    TracingApplication.Merge(data);
            }

            if (!isSuccess)
            {
                // add to audit
            }

            return isSuccess;
        }

        public override async Task<bool> CreateApplicationAsync()
        {
            if (!IsValidCategory("T01"))
                return false;

            bool success = await base.CreateApplicationAsync();

            if (!success)
            {
                var failedSubmitterManager = new FailedSubmitAuditManager(DB, TracingApplication);
                await failedSubmitterManager.AddToFailedSubmitAuditAsync(FailedSubmitActivityAreaType.T01);
            }

            // TODO: check and write Tracing specific info, if any were included

            return success;

        }

        public override async Task UpdateApplicationAsync()
        {
            // TODO: check and update Tracing specific info, if any were included
            if (!string.IsNullOrEmpty(TracingApplication.FamPro_Cd))
            {
                // affidavit data has been passed -- need to either create it or update it
                if (!await AffidavitExistsAsync())
                {
                    await ValidateAndProcessNewAffidavitAsync();

                    // ignore all changes other than Creditor

                    string newAppl_Crdtr_FrstNme = TracingApplication.Appl_Crdtr_FrstNme;
                    string newAppl_Crdtr_MddleNme = TracingApplication.Appl_Crdtr_MddleNme;
                    string newAppl_Crdtr_SurNme = TracingApplication.Appl_Crdtr_SurNme;

                    await LoadApplicationAsync(Appl_EnfSrv_Cd, Appl_CtrlCd);

                    TracingApplication.Appl_Crdtr_FrstNme = newAppl_Crdtr_FrstNme;
                    TracingApplication.Appl_Crdtr_MddleNme = newAppl_Crdtr_MddleNme;
                    TracingApplication.Appl_Crdtr_SurNme = newAppl_Crdtr_SurNme;

                    MakeUpperCase();

                    await base.UpdateApplicationNoValidationAsync();
                }
                else
                {
                    await DB.TracingTable.UpdateTracingDataAsync(TracingApplication);

                    MakeUpperCase();

                    await base.UpdateApplicationAsync();
                }

            }

        }

        public async Task<bool> ReinstateApplicationAsync(string enfService, string controlCode, string lastUpdateUser,
                                         DateTime eventEffectiveDate, EventCode bfEventCode, int bfEventId)
        {
            ReinstateEffectiveDate = eventEffectiveDate;
            BFEventReasonCode = bfEventCode;
            BFEvent_Id = bfEventId;

            await LoadApplicationAsync(enfService, controlCode);

            TracingApplication.Appl_LastUpdate_Usr = lastUpdateUser;
            TracingApplication.Appl_LastUpdate_Dte = DateTime.Now;

            if (!IsValidCategory("T01"))
                return false;

            TracingApplication.Appl_Reactv_Dte = ReinstateEffectiveDate;

            int traceCycle = TracingApplication.Trace_Cycl_Qty;

            await SetNewStateTo(ApplicationState.APPLICATION_REINSTATED_11);

            TracingApplication.Trace_Cycl_Qty = traceCycle + 1;
            TracingApplication.Trace_LiSt_Cd = 80;
            TracingApplication.Trace_LstCyclStr_Dte = DateTime.Now;

            MakeUpperCase();

            await UpdateApplicationAsync();

            return true;
        }

        public async Task<bool> RejectApplicationAsync(string enfService, string controlCode, string lastUpdateUser)
        {
            bool result = true;

            await LoadApplicationAsync(enfService, controlCode);

            TracingApplication.Appl_LastUpdate_Usr = lastUpdateUser;
            TracingApplication.Appl_LastUpdate_Dte = DateTime.Now;

            if (!IsValidCategory("T01"))
                return false;

            await SetNewStateTo(ApplicationState.APPLICATION_REJECTED_9);

            await UpdateApplicationAsync();

            return result;
        }

        public async Task<bool> CertifyAffidavitAsync(string lastUpdateUser)
        {
            TracingApplication.Appl_LastUpdate_Dte = DateTime.Now;
            TracingApplication.Appl_LastUpdate_Usr = lastUpdateUser;

            if (TracingApplication.AppLiSt_Cd != ApplicationState.PENDING_ACCEPTANCE_SWEARING_6)
            {
                TracingApplication.Messages.AddError($"CertifyL02 is not available for state {TracingApplication.AppLiSt_Cd}.  It should be from state 6 (legal authorization pending) only.");
                return false;
            }

            if (!lastUpdateUser.IsInternalUser())
            {
                TracingApplication.Subm_Affdvt_SubmCd = lastUpdateUser;
                TracingApplication.Appl_RecvAffdvt_Dte = DateTime.Now;

                await SetNewStateTo(ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7);

                MakeUpperCase();
                await UpdateApplicationNoValidationAsync();

                await EventManager.SaveEventsAsync();

                if (TracingApplication.Medium_Cd != "FTP")
                    TracingApplication.Messages.AddInformation(EventCode.C50620_VALID_APPLICATION);
            }
            else
            {
                // TODO: send message back that this is not allowed?
            }

            return true;

        }

        public async Task RejectAffidavitAsync(string lastUpdateUser)
        {
            TracingApplication.Appl_LastUpdate_Dte = DateTime.Now;
            TracingApplication.Appl_LastUpdate_Usr = lastUpdateUser;

            await SetNewStateTo(ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7);

            MakeUpperCase();
            await UpdateApplicationNoValidationAsync();

            EventManager.AddEvent(EventCode.C51019_REJECTED_AFFIDAVIT);

            await EventManager.SaveEventsAsync();

            TracingApplication.Messages.AddInformation(EventCode.C50763_AFFIDAVIT_REJECTED_BY_FOAEA);
        }

        public async Task FullyServiceApplicationAsync(string enfSrvCode)
        {
            if (TracingApplication.AppLiSt_Cd == ApplicationState.MANUALLY_TERMINATED_14)
            {
                var traceResponseDB = DB.TraceResponseTable;
                await traceResponseDB.DeleteCancelledApplicationTraceResponseDataAsync(Appl_EnfSrv_Cd, Appl_CtrlCd, enfSrvCode);
            }
            else
            {
                await SetNewStateTo(ApplicationState.FULLY_SERVICED_13);

                MakeUpperCase();
                await UpdateApplicationAsync();
            }
        }

        public async Task PartiallyServiceApplicationAsync(string enfSrvCode)
        {
            if (TracingApplication.AppLiSt_Cd == ApplicationState.MANUALLY_TERMINATED_14)
            {
                var traceResponseDB = DB.TraceResponseTable;
                await traceResponseDB.DeleteCancelledApplicationTraceResponseDataAsync(Appl_EnfSrv_Cd, Appl_CtrlCd, enfSrvCode);
            }
            else
            {
                await SetNewStateTo(ApplicationState.PARTIALLY_SERVICED_12);

                MakeUpperCase();
                await UpdateApplicationAsync();
            }
        }

        public async Task TerminateApplicationAsync(string enfSrvCode)
        {

            var traceResponseDB = DB.TraceResponseTable;
            await traceResponseDB.DeleteCancelledApplicationTraceResponseDataAsync(Appl_EnfSrv_Cd, Appl_CtrlCd, enfSrvCode);

            await SetNewStateTo(ApplicationState.MANUALLY_TERMINATED_14);

            MakeUpperCase();
            await UpdateApplicationAsync();

        }

        public async Task<List<TraceCycleQuantityData>> GetTraceCycleQuantityDataAsync(string enfSrv_Cd, string cycle)
        {
            var tracingDB = DB.TracingTable;
            return await tracingDB.GetTraceCycleQuantityDataAsync(enfSrv_Cd, cycle);
        }

        public async Task<List<TraceToApplData>> GetTraceToApplDataAsync()
        {
            var tracingDB = DB.TracingTable;
            return await tracingDB.GetTraceToApplDataAsync();
        }

        private async Task SetTracingForReinstateAsync(DateTime traceDate, DateTime bfDate, EventCode eventReasonCode)
        {

            TracingApplication.AppLiSt_Cd = ApplicationState.APPLICATION_ACCEPTED_10;
            TracingApplication.Messages.AddInformation(EventCode.C50780_APPLICATION_ACCEPTED);

            EventManager.AddEvent(EventCode.C50780_APPLICATION_ACCEPTED, queue: EventQueue.EventTrace);
            EventManager.AddEvent(EventCode.C50780_APPLICATION_ACCEPTED);

            EventManager.AddEvent(eventReasonCode, appState: ApplicationState.APPLICATION_REINSTATED_11, effectiveDateTime: traceDate);
            EventManager.AddBFEvent(eventReasonCode, appState: ApplicationState.APPLICATION_REINSTATED_11, effectiveTimestamp: bfDate);

            await Validation.AddDuplicateCreditorWarningEventsAsync();

        }

        private async Task ValidateAndProcessNewAffidavitAsync()
        {

            if (!TracingApplication.Appl_LastUpdate_Usr.IsInternalUser())
            {
                TracingApplication.Subm_Affdvt_SubmCd = null;
            }

            if (TracingApplication.AppLiSt_Cd.In(ApplicationState.INVALID_APPLICATION_1, ApplicationState.SIN_CONFIRMATION_PENDING_3,
                                                 ApplicationState.SIN_NOT_CONFIRMED_5, ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7))
            {
                TracingApplication.Trace_LiSt_Cd = 80;
                await DB.TracingTable.CreateTracingDataAsync(TracingApplication);

                if (TracingApplication.AppLiSt_Cd == ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7)
                {
                    await Process_04_SinConfirmed();
                }
            }

        }

        public override async Task ProcessBringForwardsAsync(ApplicationEventData bfEvent)
        {
            bool closeEvent = false;

            TimeSpan diff = TimeSpan.Zero;
            if (TracingApplication.Appl_LastUpdate_Dte.HasValue)
                diff = TracingApplication.Appl_LastUpdate_Dte.Value - DateTime.Now;

            if ((TracingApplication.ActvSt_Cd != "A") &&
                ((!bfEvent.Event_Reas_Cd.HasValue) || (
                 (bfEvent.Event_Reas_Cd.NotIn(EventCode.C50806_SCHEDULED_TO_BE_REINSTATED__QUARTERLY_TRACING,
                                              EventCode.C50680_CHANGE_OR_SUPPLY_ADDITIONAL_DEBTOR_INFORMATION_SEE_SIN_VERIFICATION_RESULTS_PAGE_IN_FOAEA_FOR_SPECIFIC_DETAILS,
                                              EventCode.C50600_INVALID_APPLICATION)))) &&
                ((diff.Equals(TimeSpan.Zero)) || (Math.Abs(diff.TotalHours) > 24)))
            {
                await EventManager.SaveEventAsync(bfEvent, ApplicationState.MANUALLY_TERMINATED_14, "I");
                closeEvent = false;
            }
            else
            {
                if (bfEvent.Event_Reas_Cd.HasValue)
                {
                    var currentLifeState = TracingApplication.AppLiSt_Cd;

                    switch (bfEvent.Event_Reas_Cd)
                    {
                        case EventCode.C50528_BF_10_DAYS_FROM_RECEIPT_OF_APPLICATION:
                            if (currentLifeState < ApplicationState.APPLICATION_REJECTED_9)
                            {
                                var DaysElapsed = Math.Abs((DateTime.Now - TracingApplication.Appl_Lgl_Dte).TotalDays);

                                if (currentLifeState.In(ApplicationState.INVALID_APPLICATION_1, ApplicationState.SIN_NOT_CONFIRMED_5) &&
                                   (DaysElapsed > 40))
                                {
                                    await RejectApplicationAsync(Appl_EnfSrv_Cd, Appl_CtrlCd, DB.CurrentSubmitter);
                                    EventManager.AddEvent(EventCode.C50760_APPLICATION_REJECTED_AS_CONDITIONS_NOT_MET_IN_TIMEFRAME);
                                }
                                else if (DaysElapsed > 90)
                                {
                                    await RejectApplicationAsync(Appl_EnfSrv_Cd, Appl_CtrlCd, DB.CurrentSubmitter);
                                    EventManager.AddEvent(EventCode.C50760_APPLICATION_REJECTED_AS_CONDITIONS_NOT_MET_IN_TIMEFRAME);
                                }
                                else
                                {
                                    AddNewBFevent();
                                }
                            }
                            break;

                        case EventCode.C50804_SCHEDULED_TO_BE_REINSTATED:
                            if (currentLifeState == ApplicationState.APPLICATION_REINSTATED_11)
                            {
                                await ReinstateApplicationAsync(Appl_EnfSrv_Cd, Appl_CtrlCd, DB.CurrentSubmitter,
                                                     bfEvent.Event_Effctv_Dte, bfEvent.Event_Reas_Cd.Value, bfEvent.Event_Id);
                                await SetNewStateTo(ApplicationState.APPLICATION_REINSTATED_11);
                            }
                            break;

                        case EventCode.C50806_SCHEDULED_TO_BE_REINSTATED__QUARTERLY_TRACING:
                            if (currentLifeState.In(ApplicationState.PARTIALLY_SERVICED_12, ApplicationState.EXPIRED_15))
                            {
                                // TODO: if it is at state 15, how can it re-instate?
                                await ReinstateApplicationAsync(Appl_EnfSrv_Cd, Appl_CtrlCd, DB.CurrentSubmitter,
                                                     bfEvent.Event_Effctv_Dte, bfEvent.Event_Reas_Cd.Value, bfEvent.Event_Id);
                                await SetNewStateTo(ApplicationState.APPLICATION_REINSTATED_11);
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
            }

            if (closeEvent)
            {
                await EventManager.SaveEventAsync(bfEvent, TracingApplication.AppLiSt_Cd, "C");
            }

            await EventManager.SaveEventsAsync();
        }

        private void AddNewBFevent()
        {
            DateTime nextBFdate = DateTime.Now;
            EventCode eventReasonCode = EventCode.UNDEFINED;

            if (TracingApplication.AppLiSt_Cd.In(ApplicationState.INVALID_APPLICATION_1, ApplicationState.SIN_NOT_CONFIRMED_5))
            {
                eventReasonCode = EventCode.C50902_AWAITING_AN_ACTION_ON_THIS_APPLICATION;
                nextBFdate = nextBFdate.AddDays(10);
            }
            else if (TracingApplication.AppLiSt_Cd.In(ApplicationState.PENDING_ACCEPTANCE_SWEARING_6, ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7))
            {
                switch (TracingApplication.AppLiSt_Cd)
                {
                    case ApplicationState.PENDING_ACCEPTANCE_SWEARING_6:
                        eventReasonCode = EventCode.C51042_REQUIRES_LEGAL_AUTHORIZATION;
                        break;
                    case ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7:
                        eventReasonCode = EventCode.C54004_AWAITING_AFFIDAVIT_TO_BE_ENTERED;
                        break;
                }
                nextBFdate = nextBFdate.AddDays(20);
            }
            else
            {
                eventReasonCode = EventCode.C50902_AWAITING_AN_ACTION_ON_THIS_APPLICATION;
                nextBFdate = nextBFdate.AddDays(10);

                EventManager.AddEvent(EventCode.C51100_STATE_OUT_OF_SYNC, queue: EventQueue.EventAM);
            }

            EventManager.AddEvent(eventReasonCode);
            EventManager.AddBFEvent(EventCode.C50520_INVALID_STATUS_CODE, effectiveTimestamp: nextBFdate);
        }

        public async Task<DataList<TracingApplicationData>> GetApplicationsWaitingForAffidavitAsync()
        {
            return await DB.TracingTable.GetApplicationsWaitingForAffidavitAsync();
        }

        private async Task<bool> AffidavitExistsAsync()
        {
            return await DB.TracingTable.TracingDataExistsAsync(Appl_EnfSrv_Cd, Appl_CtrlCd);
        }

        public async Task<DataList<TraceResponseData>> GetTraceResultsAsync(bool checkCycle = false)
        {
            return await DB.TraceResponseTable.GetTraceResponseForApplicationAsync(Appl_EnfSrv_Cd, Appl_CtrlCd, checkCycle);
        }

        public async Task<List<ApplicationEventData>> GetRequestedTRCINTracingEventsAsync(string enfSrv_Cd, string cycle)
        {
            return await EventManager.GetRequestedTRCINTracingEventsAsync(enfSrv_Cd, cycle);
        }

        public async Task<List<ApplicationEventDetailData>> GetActiveTracingEventDetailsAsync(string enfSrv_Cd, string cycle)
        {
            return await EventDetailManager.GetActiveTracingEventDetailsAsync(enfSrv_Cd, cycle);
        }

        public async Task ApplySINconfirmation()
        {
            await SetNewStateTo(ApplicationState.SIN_CONFIRMED_4);

            var sinManager = new ApplicationSINManager(TracingApplication, this);
            await sinManager.UpdateSINChangeHistoryAsync();

            foreach (var eventItem in EventManager.Events)
                eventItem.Subm_Update_SubmCd = "SYSTEM";

            await UpdateApplicationNoValidationAsync();

        }

        public async Task ApplySINfailedAsync()
        {
            await SetNewStateTo(ApplicationState.SIN_NOT_CONFIRMED_5);

            foreach (var eventItem in EventManager.Events)
                eventItem.Subm_Update_SubmCd = "SYSTEM";

            await UpdateApplicationNoValidationAsync();
        }

        public async Task CreateResponseDataAsync(List<TraceResponseData> responseData)
        {
            var responsesDB = DB.TraceResponseTable;
            await responsesDB.InsertBulkDataAsync(responseData);
        }

        public async Task MarkResponsesAsViewedAsync(string enfService)
        {
            var responsesDB = DB.TraceResponseTable;
            await responsesDB.MarkResponsesAsViewedAsync(enfService);
        }

        public async Task<List<TracingOutgoingFederalData>> GetFederalOutgoingDataAsync(int maxRecords,
                                                                       string activeState,
                                                                       ApplicationState lifeState,
                                                                       string enfServiceCode)
        {
            var tracingDB = DB.TracingTable;
            return await tracingDB.GetFederalOutgoingDataAsync(maxRecords, activeState, lifeState, enfServiceCode);
        }

        public async Task<List<TracingOutgoingProvincialData>> GetProvincialOutgoingDataAsync(int maxRecords,
                                                                             string activeState,
                                                                             string recipientCode,
                                                                             bool isXML = true)
        {
            var tracingDB = DB.TracingTable;
            var data = await tracingDB.GetProvincialOutgoingDataAsync(maxRecords, activeState, recipientCode, isXML);
            return data;
        }
    }

}
