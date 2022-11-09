using BackendProcesses.Business;
using DBHelper;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Resources.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.Business.Areas.Application
{
    internal partial class InterceptionManager : ApplicationManager
    {
        private async Task SendDebtorLetterAsync()
        {
            // TODO: this checks will currently fail in the old code, but will work here... should it?
            var events = await EventManager.GetEventBFAsync(InterceptionApplication.Subm_SubmCd, Appl_CtrlCd,
                                                 EventCode.C54005_CREATE_A_DEBTOR_LETTER_EVENT_IN_EVNTDBTR, "A");

            if (events.Count == 0)
                EventManager.AddBFEvent(EventCode.C54005_CREATE_A_DEBTOR_LETTER_EVENT_IN_EVNTDBTR,
                    effectiveTimestamp: DateTime.Now, appState: ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7);

        }

        private static string GetDebtorID(string justiceID)
        {
            if (!string.IsNullOrEmpty(justiceID))
                return justiceID.Trim()[..7];
            else
                return null;
        }

        private static string DebtorPrefix(string name)
        {
            name = name.Trim();
            name = name.Replace("'", "");
            if (name.Length >= 3)
                return name.Substring(0, 3);
            else
                return name.PadRight(3, '-');
        }

        private async Task<string> GenerateDebtorIDAsync(string debtorLastName)
        {
            return await DB.InterceptionTable.GetDebtorIDAsync(DebtorPrefix(debtorLastName)); ;
        }

        private async Task<string> NextJusticeIDAsync(string justiceID)
        {
            string justiceSuffix = justiceID.Substring(justiceID.Length - 1, 1);
            string debtorID = justiceID.Substring(0, 7);
            if (string.IsNullOrEmpty(justiceID))
                justiceSuffix = "A";
            else
                justiceSuffix = GetNextSuffixLetter(justiceSuffix);

            if (justiceSuffix == "@")
                justiceSuffix = "A";

            string newJusticeNumber = debtorID.Trim() + justiceSuffix.Trim();

            bool isAlreadyUser = await DB.InterceptionTable.IsAlreadyUsedJusticeNumberAsync(newJusticeNumber);

            if (isAlreadyUser)
                justiceSuffix = await NextJusticeIDAsync(justiceSuffix);

            nextJusticeID_callCount++;
            if (nextJusticeID_callCount > 26)
                throw new Exception("Infinite loop!");

            return justiceSuffix;
        }

        private static string GetNextSuffixLetter(string previousJusticeSuffix)
        {
            const string JUSTICE_ID_SUFFIX = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            int position = JUSTICE_ID_SUFFIX.IndexOf(previousJusticeSuffix);
            if ((position >= 25) || (position == -1) || (position == JUSTICE_ID_SUFFIX.Length - 1))
                return "@";
            else
                return JUSTICE_ID_SUFFIX.Substring(position + 1, 1);
        }

        private bool IsESD_MEP(string enforcementServiceCode)
        {
            return config.ESDsites.Contains(enforcementServiceCode);
        }

        private async Task ProcessSummSmryBFNAsync(string debtorId, EventCode eventBFNreasonCode)
        {
            var summSmryData = await DBfinance.SummonsSummaryRepository.GetSummonsSummaryAsync(debtorId: debtorId);

            foreach (var summSmryItem in summSmryData)
                if ((summSmryItem.Appl_EnfSrv_Cd != Appl_EnfSrv_Cd) && (summSmryItem.Appl_CtrlCd != Appl_CtrlCd))
                    await ProcessBringForwardNotificationAsync(summSmryItem.Appl_EnfSrv_Cd, summSmryItem.Appl_CtrlCd, eventBFNreasonCode);
        }

        private async Task ProcessBringForwardNotificationAsync(string applEnfSrvCode, string applControlCode, EventCode eventBFNreasonCode)
        {
            var thisApplicationManager = new InterceptionManager(DB, DBfinance, config)
            {
                CurrentUser = this.CurrentUser
            };

            var thisApplication = thisApplicationManager.InterceptionApplication;
            var applEventManager = new ApplicationEventManager(thisApplication, DB);
            var applEventDetailManager = new ApplicationEventDetailManager(thisApplication, DB);

            var eventBFNs = (await applEventManager.GetApplicationEventsForQueueAsync(EventQueue.EventBFN))
                                .Where(m => m.ActvSt_Cd == "A");
            var eventBFNDetails = (await applEventDetailManager.GetApplicationEventDetailsForQueueAsync(EventQueue.EventBFN_dtl))
                                    .Where(m => m.ActvSt_Cd == "A");

            foreach (var thisEventBFN in eventBFNs)
            {
                EventCode thisEventCode = EventCode.UNDEFINED;
                if (thisEventBFN.Event_Reas_Cd.HasValue)
                    thisEventCode = thisEventBFN.Event_Reas_Cd.Value;

                switch (thisEventCode)
                {
                    case EventCode.C56003_CANCELLED_OR_COMPLETED_BFN:
                        eventBFNreasonCode = EventCode.C56002_NEW_BFN_FOR_EXISTING_DEBTOR;
                        thisEventBFN.ActvSt_Cd = "C";
                        thisEventBFN.Event_Compl_Dte = DateTime.Now;
                        break;
                    case EventCode.C56001_NEW_BFN_FOR_NEW_DEBTOR:
                        eventBFNreasonCode = EventCode.UNDEFINED;
                        break;
                    case EventCode.C56002_NEW_BFN_FOR_EXISTING_DEBTOR:
                        eventBFNreasonCode = EventCode.UNDEFINED;
                        break;
                    case EventCode.UNDEFINED:
                        eventBFNreasonCode = EventCode.C56001_NEW_BFN_FOR_NEW_DEBTOR;
                        break;
                }

            }

            foreach (var thisEventBFNdetail in eventBFNDetails)
            {
                EventCode thisEventDetailCode = EventCode.UNDEFINED;
                if (thisEventBFNdetail.Event_Reas_Cd.HasValue)
                    thisEventDetailCode = thisEventBFNdetail.Event_Reas_Cd.Value;

                if (thisEventDetailCode == EventCode.C56003_CANCELLED_OR_COMPLETED_BFN)
                {
                    thisEventBFNdetail.ActvSt_Cd = "C";
                    thisEventBFNdetail.Event_Compl_Dte = DateTime.Now;
                    thisEventBFNdetail.Event_Reas_Text = $"Application {thisApplication.Appl_EnfSrv_Cd}-{thisApplication.Appl_CtrlCd} became active, STOP BLOCK not sent";
                }
            }

            await applEventManager.SaveEventsAsync();
            await applEventDetailManager.SaveEventDetailsAsync();
        }

        private async Task<DateTime> RecalculateFixedAmountRecalcDateAfterVariationAsync(InterceptionFinancialHoldbackData newFinTerms, DateTime variationCalcDate)
        {
            DateTime fixedAmountRecalcDate = variationCalcDate;
            DateTime currDateTime = variationCalcDate;
            DateTime ctrlFaDtePayable = currDateTime;

            var summSmry = await DBfinance.SummonsSummaryRepository.GetSummonsSummaryAsync(Appl_EnfSrv_Cd, Appl_CtrlCd);
            var workActSummons = await DBfinance.ActiveSummonsRepository.GetActiveSummonsCoreAsync(ctrlFaDtePayable, Appl_EnfSrv_Cd, Appl_CtrlCd);

            if (workActSummons is not null)
            {
                var activeSummonData = GetActiveSummonsForVariation(ctrlFaDtePayable, newFinTerms, summSmry.FirstOrDefault());

                if (activeSummonData is not null)
                {
                    DateTime calcFaDtePayable;

                    // if payable date prior to variation, use system date as payable date

                    if ((ctrlFaDtePayable > activeSummonData.Start_Dte) &&
                        (activeSummonData.IntFinH_VarIss_Dte.HasValue) &&
                        (ctrlFaDtePayable < activeSummonData.IntFinH_VarIss_Dte))
                        calcFaDtePayable = currDateTime;
                    else
                        calcFaDtePayable = ctrlFaDtePayable;

                    // if summon has been varied, use variation issue date of latest variation
                    // to determine current period

                    DateTime calcStartDate = activeSummonData.IntFinH_VarIss_Dte ?? activeSummonData.Start_Dte;

                    string fixedAmountPrdFreqCd = activeSummonData.IntFinH_DefHldbAmn_Period ?? string.Empty;

                    // adjust calcStartDate so that it matches the SummSmry StartDate (as if the periods
                    // had been calculated from that start date)

                    if (InterceptionApplication.Appl_RecvAffdvt_Dte is null)
                    {
                        await AddSystemErrorAsync(DB, InterceptionApplication.Messages, config.EmailRecipients,
                                       $"Appl_RecvAffdvt_Dte is null for {Appl_EnfSrv_Cd}-{Appl_CtrlCd}. Cannot recalculate fixed amount recalc date after variation!");
                        return fixedAmountRecalcDate;
                    }

                    DateTime acceptedDate = InterceptionApplication.Appl_RecvAffdvt_Dte.Value.Date;
                    calcStartDate = AmountOwedProcess.AdjustCalcAcceptedDateBasedOnAcceptedDateAndPeriod(calcStartDate, acceptedDate, fixedAmountPrdFreqCd);

                    int currentPeriod = AmountOwedProcess.CalculatePeriodCount(fixedAmountPrdFreqCd, calcFaDtePayable.Date, calcStartDate.Date);

                    DateTime? newDate = AmountOwedProcess.CalculateRecalcDateFromStartDateForPeriodCount(fixedAmountPrdFreqCd, currentPeriod, calcStartDate);

                    if (newDate.HasValue)
                        fixedAmountRecalcDate = newDate.Value;

                }

            }

            return fixedAmountRecalcDate;

        }

        private ActiveSummonsData GetActiveSummonsForVariation(DateTime ctrlFaDtePayable, InterceptionFinancialHoldbackData intFinHdata, SummonsSummaryData summSmryData)
        {
            ActiveSummonsData activeSummons = null;

            var appl = InterceptionApplication;

            if ((ctrlFaDtePayable >= summSmryData.Start_Dte) && (ctrlFaDtePayable <= summSmryData.End_Dte))
            {
                DateTime thisVarEnterDte = default;
                if (!intFinHdata.IntFinH_VarIss_Dte.HasValue)
                    if (intFinHdata.IntFinH_RcvtAffdvt_Dte.HasValue)
                        thisVarEnterDte = intFinHdata.IntFinH_RcvtAffdvt_Dte.Value;

                string hldbTypeCode = "T";
                if (!string.IsNullOrEmpty(intFinHdata.HldbTyp_Cd))
                    hldbTypeCode = intFinHdata.HldbTyp_Cd;

                decimal mxmTtl_Money = intFinHdata.IntFinH_MxmTtl_Money ?? 0M;
                decimal perPym_Money = intFinHdata.IntFinH_PerPym_Money ?? 0M;
                byte cmlPrPym_Ind = intFinHdata.IntFinH_CmlPrPym_Ind ?? 0;
                int defHldbPrcnt = intFinHdata.IntFinH_DefHldbPrcnt ?? 0;
                decimal defHldbAmn_Money = intFinHdata.IntFinH_DefHldbAmn_Money ?? 0M;
                DateTime varIss_Dte = intFinHdata.IntFinH_VarIss_Dte ?? default;
                string pymPrCd = intFinHdata.PymPr_Cd ?? string.Empty;
                string defHldbAmnPrCd = intFinHdata.IntFinH_DefHldbAmn_Period ?? string.Empty;

                activeSummons = new ActiveSummonsData
                {
                    Subm_SubmCd = appl.Subm_SubmCd,
                    Appl_JusticeNr = appl.Appl_JusticeNr,
                    IntFinH_LmpSum_Money = intFinHdata.IntFinH_LmpSum_Money,
                    IntFinH_PerPym_Money = perPym_Money,
                    IntFinH_MxmTtl_Money = mxmTtl_Money,
                    PymPr_Cd = pymPrCd,
                    IntFinH_CmlPrPym_Ind = cmlPrPym_Ind,
                    HldbCtg_Cd = intFinHdata.HldbCtg_Cd,
                    IntFinH_DefHldbPrcnt = defHldbPrcnt,
                    IntFinH_DefHldbAmn_Money = defHldbAmn_Money,
                    IntFinH_DefHldbAmn_Period = defHldbAmnPrCd,
                    HldbTyp_Cd = hldbTypeCode,
                    Start_Dte = summSmryData.Start_Dte,
                    FeeDivertedTtl_Money = summSmryData.FeeDivertedTtl_Money,
                    LmpSumDivertedTtl_Money = summSmryData.LmpSumDivertedTtl_Money,
                    PerPymDivertedTtl_Money = summSmryData.PerPymDivertedTtl_Money,
                    HldbAmtTtl_Money = summSmryData.HldbAmtTtl_Money,
                    Appl_TotalAmnt = summSmryData.Appl_TotalAmnt,
                    IntFinH_Dte = intFinHdata.IntFinH_Dte,
                    End_Dte = summSmryData.End_Dte,
                    Appl_RecvAffdvt_Dte = summSmryData.Start_Dte,
                    IntFinH_VarIss_Dte = varIss_Dte,
                    LmpSumOwedTtl_Money = summSmryData.LmpSumOwedTtl_Money,
                    PerPymOwedTtl_Money = summSmryData.PerPymOwedTtl_Money,
                    Appl_EnfSrv_Cd = intFinHdata.Appl_EnfSrv_Cd,
                    VarEnterDte = thisVarEnterDte,
                    Appl_CtrlCd = intFinHdata.Appl_CtrlCd
                };

                if (!intFinHdata.IntFinH_VarIss_Dte.HasValue)
                    activeSummons.IntFinH_VarIss_Dte = null;

            }

            return activeSummons;
        }
        private async Task IncrementGarnSmryAsync(bool isNewApplication = false)
        {

            var garnSmryDB = DBfinance.GarnSummaryRepository;

            if (isNewApplication || InterceptionApplication.AppLiSt_Cd.In(ApplicationState.SIN_NOT_CONFIRMED_5,
                                                                          ApplicationState.APPLICATION_REJECTED_9,
                                                                          ApplicationState.APPLICATION_SUSPENDED_35))
            {
                int fiscalMonthCounter = DateTime.Now.GetFiscalMonth();
                int fiscalYear = DateTime.Now.GetFiscalYear();
                string enfOfficeCode = InterceptionApplication.Subm_SubmCd.Substring(3, 1);

                var garnSummaryData = await garnSmryDB.GetGarnSummaryAsync(Appl_EnfSrv_Cd, enfOfficeCode, fiscalMonthCounter, fiscalYear);

                GarnSummaryData thisGarnSmryData;

                if (garnSummaryData.Count == 0)
                {
                    int totalActiveSummonsCount = await DB.InterceptionTable.GetTotalActiveSummonsAsync(Appl_EnfSrv_Cd, enfOfficeCode);
                    thisGarnSmryData = new GarnSummaryData
                    {
                        EnfSrv_Cd = Appl_EnfSrv_Cd,
                        EnfOff_Cd = enfOfficeCode,
                        AcctYear = fiscalYear,
                        AcctMonth = fiscalMonthCounter,
                        Ttl_ActiveSummons_Count = totalActiveSummonsCount,
                        Mth_ActiveSummons_Count = 0,
                        Mth_ActionedSummons_Count = 0,
                        Mth_LumpSumActive_Amount = 0,
                        Mth_PeriodicActive_Amount = 0,
                        Mth_FeesActive_Amount = 0,
                        Mth_FeesDiverted_Amount = 0,
                        Mth_LumpSumDiverted_Amount = 0,
                        Mth_PeriodicDiverted_Amount = 0,
                        Mth_FeesOwed_Amount = 0,
                        Mth_FeesRemitted_Amount = 0,
                        Mth_FeesCollected_Amount = 0,
                        Mth_FeesDisbursed_Amount = 0,
                        Mth_Uncollected_Amount = 0,
                        Mth_FeesSatisfied_Count = 0,
                        Mth_FeesUnsatisfied_Count = 0,
                        Mth_Garnisheed_Amount = 0,
                        Mth_DivertActions_Count = 0,
                        Mth_Variation1_Count = 0,
                        Mth_Variation2_Count = 0,
                        Mth_Variation3_Count = 0,
                        Mth_Variations_Count = 0,
                        Mth_SummonsReceived_Count = 0,
                        Mth_SummonsCancelled_Count = 0,
                        Mth_SummonsRejected_Count = 0,
                        Mth_SummonsSatisfied_Count = 0,
                        Mth_SummonsExpired_Count = 0,
                        Mth_SummonsSuspended_Count = 0,
                        Mth_SummonsArchived_Count = 0,
                        Mth_SummonsSIN_Count = 0,
                        Mth_Action_Count = 0,
                        Mth_FAAvailable_Amount = 0,
                        Mth_FA_Count = 0,
                        Mth_CRAction_Count = 0,
                        Mth_CRFee_Amount = 0,
                        Mth_CRPaid_Amount = 0
                    };

                    await garnSmryDB.CreateGarnSummaryAsync(thisGarnSmryData);
                }
                else
                    thisGarnSmryData = garnSummaryData.First();

                if (isNewApplication)
                {
                    if (thisGarnSmryData.Mth_SummonsReceived_Count.HasValue)
                        thisGarnSmryData.Mth_SummonsReceived_Count++;
                    else
                        thisGarnSmryData.Mth_SummonsReceived_Count = 1;
                }

                switch (InterceptionApplication.AppLiSt_Cd)
                {
                    case ApplicationState.SIN_NOT_CONFIRMED_5:
                        if (thisGarnSmryData.Mth_SummonsSIN_Count.HasValue)
                            thisGarnSmryData.Mth_SummonsSIN_Count++;
                        else
                            thisGarnSmryData.Mth_SummonsSIN_Count = 1;
                        break;

                    case ApplicationState.APPLICATION_REJECTED_9:
                        if (thisGarnSmryData.Mth_SummonsRejected_Count.HasValue)
                            thisGarnSmryData.Mth_SummonsRejected_Count++;
                        else
                            thisGarnSmryData.Mth_SummonsRejected_Count = 1;
                        break;

                    case ApplicationState.APPLICATION_SUSPENDED_35:
                        if (thisGarnSmryData.Mth_SummonsSuspended_Count.HasValue)
                            thisGarnSmryData.Mth_SummonsSuspended_Count++;
                        else
                            thisGarnSmryData.Mth_SummonsSuspended_Count = 1;
                        break;
                }

                await garnSmryDB.UpdateGarnSummaryAsync(thisGarnSmryData);

            }

        }

        private async Task<DateTime> ChangeStateForFinancialTermsAsync(string oldState, string newState, short intFinH_LifeStateCode)
        {

            var interceptionDB = DB.InterceptionTable;

            var defaultHoldback = InterceptionApplication.IntFinH ?? await interceptionDB.GetInterceptionFinancialTermsAsync(Appl_EnfSrv_Cd, Appl_CtrlCd, oldState);

            DateTime intFinH_Date = defaultHoldback.IntFinH_Dte;

            defaultHoldback.ActvSt_Cd = newState;

            defaultHoldback.IntFinH_LiStCd = intFinH_LifeStateCode;

            if (newState == "A")
            {
                defaultHoldback.IntFinH_RcvtAffdvt_Dte = DateTime.Now;
                defaultHoldback.IntFinH_Affdvt_SubmCd = "FO2SSS";
            }

            var sourceSpecificHoldbacks = InterceptionApplication.HldbCnd ?? await interceptionDB.GetHoldbackConditionsAsync(Appl_EnfSrv_Cd, Appl_CtrlCd, intFinH_Date, oldState);

            foreach (var sourceSpecificHoldback in sourceSpecificHoldbacks)
            {
                sourceSpecificHoldback.ActvSt_Cd = newState;
                sourceSpecificHoldback.HldbCnd_LiStCd = 2; // valid
            }

            await interceptionDB.UpdateInterceptionFinancialTermsAsync(defaultHoldback);
            await interceptionDB.UpdateHoldbackConditionsAsync(sourceSpecificHoldbacks);

            return intFinH_Date;

        }

        private async Task DeletePendingFinancialTermsAsync()
        {

            var interceptionDB = DB.InterceptionTable;

            var defaultHoldback = await interceptionDB.GetInterceptionFinancialTermsAsync(Appl_EnfSrv_Cd, Appl_CtrlCd, "P");

            DateTime intFinH_Date = defaultHoldback.IntFinH_Dte;

            var sourceSpecificHoldbacks = await interceptionDB.GetHoldbackConditionsAsync(Appl_EnfSrv_Cd, Appl_CtrlCd, intFinH_Date, "P");

            await interceptionDB.DeleteHoldbackConditionsAsync(sourceSpecificHoldbacks);
            await interceptionDB.DeleteInterceptionFinancialTermsAsync(defaultHoldback);
        }

        private async Task<DateTime> GetLastVariationDateAsync()
        {
            var interceptionDB = DB.InterceptionTable;

            var defaultHoldback = await interceptionDB.GetInterceptionFinancialTermsAsync(Appl_EnfSrv_Cd, Appl_CtrlCd, "P");

            return defaultHoldback.IntFinH_Dte;
        }

        private async Task CreateSummonsSummaryAsync(string debtorID, string justiceSuffix, DateTime startDate)
        {
            var summSummary = new SummonsSummaryData
            {
                Appl_EnfSrv_Cd = Appl_EnfSrv_Cd,
                Appl_CtrlCd = Appl_CtrlCd,
                Dbtr_Id = debtorID,
                Appl_JusticeNrSfx = justiceSuffix,
                Start_Dte = startDate,
                End_Dte = startDate.AddYears(5).AddDays(-1),
                SummSmry_Recalc_Dte = startDate
            };

            var existingData = await DBfinance.SummonsSummaryRepository.GetSummonsSummaryAsync(Appl_EnfSrv_Cd, Appl_CtrlCd);

            if (!existingData.Any())
                await DBfinance.SummonsSummaryRepository.CreateSummonsSummaryAsync(summSummary);
            else
                // TODO: should we get a warning that instead of creating we are updating an existing summsmry?
                await DBfinance.SummonsSummaryRepository.UpdateSummonsSummaryAsync(summSummary);
        }

        private async Task NotifyMatchingActiveApplicationsAsync(EventCode eventCode)
        {
            var matchedApplications = await DB.InterceptionTable.FindMatchingActiveApplicationsAsync(Appl_EnfSrv_Cd, Appl_CtrlCd,
                                                                                                InterceptionApplication.Appl_Dbtr_Cnfrmd_SIN,
                                                                                                InterceptionApplication.Appl_Crdtr_FrstNme,
                                                                                                InterceptionApplication.Appl_Crdtr_SurNme);

            if (matchedApplications.Any())
            {
                string newApplicationKey = $"{Appl_EnfSrv_Cd}-{InterceptionApplication.Subm_SubmCd}-{Appl_CtrlCd}";
                foreach (var matchedApplication in matchedApplications)
                {
                    EventManager.AddEvent(eventCode, eventReasonText: newApplicationKey, appState: matchedApplication.AppLiSt_Cd,
                                          submCd: matchedApplication.Subm_SubmCd, recipientSubm: matchedApplication.Subm_Recpt_SubmCd,
                                          enfSrv: matchedApplication.Appl_EnfSrv_Cd, controlCode: matchedApplication.Appl_CtrlCd);
                }
            }
        }

        private async Task StopBlockFundsAsync(ApplicationState requestedState, ApplicationState previousState)
        {
            string debtorID = GetDebtorID(InterceptionApplication.Appl_JusticeNr);
            int summSmryCount = 0;

            var summSmryInfoForDebtor = await DBfinance.SummonsSummaryRepository.GetSummonsSummaryAsync(debtorId: debtorID);

            foreach (var summSmryInfo in summSmryInfoForDebtor)
            {
                if (summSmryInfo.ActualEnd_Dte is null)
                    summSmryCount++;
            }

            summSmryCount--;

            if (summSmryCount <= 0)
            {
                var effective10daysFromNow = DateTime.Now.AddDays(10);
                EventManager.AddEvent(EventCode.C56003_CANCELLED_OR_COMPLETED_BFN, queue: EventQueue.EventBFN,
                                      effectiveDateTime: effective10daysFromNow);
            }

            var summSmryForCurrentAppl = (await DBfinance.SummonsSummaryRepository.GetSummonsSummaryAsync(Appl_EnfSrv_Cd, Appl_CtrlCd))
                                            .FirstOrDefault();
            if ((summSmryForCurrentAppl is null) && (previousState >= ApplicationState.APPLICATION_ACCEPTED_10))
            {
                await AddSystemErrorAsync(DB, InterceptionApplication.Messages, config.SystemErrorRecipients,
                               $"Could not find summSmry record for {Appl_EnfSrv_Cd}-{Appl_CtrlCd} in StopBlockFunds!");
                return;
            }

            if (summSmryForCurrentAppl is not null)
            {
                switch (requestedState)
                {
                    case ApplicationState.FULLY_SERVICED_13:
                    case ApplicationState.MANUALLY_TERMINATED_14:
                        summSmryForCurrentAppl.ActualEnd_Dte = DateTime.Now;
                        break;
                    case ApplicationState.EXPIRED_15:
                        summSmryForCurrentAppl.ActualEnd_Dte = summSmryForCurrentAppl.End_Dte;
                        break;
                    default:
                        // Throw New Exception("Invalid State for the current application.")
                        break;
                }

                await DBfinance.SummonsSummaryRepository.UpdateSummonsSummaryAsync(summSmryForCurrentAppl);
            }

            await DB.InterceptionTable.EISOHistoryDeleteBySINAsync(InterceptionApplication.Appl_Dbtr_Cnfrmd_SIN, false);

        }

    }
}
