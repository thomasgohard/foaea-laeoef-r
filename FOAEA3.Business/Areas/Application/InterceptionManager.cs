using BackendProcesses.Business;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FOAEA3.Business.Areas.Application
{
    internal partial class InterceptionManager : ApplicationManager
    {
        private const string I01_AFFITDAVIT_DOCUMENT_CODE = "IXX";

        public InterceptionApplicationData InterceptionApplication { get; }
        private IRepositories_Finance RepositoriesFinance { get; }
        private InterceptionValidation InterceptionValidation { get; }
        private bool? AcceptedWithin30Days { get; set; } = null;
        private bool ESDReceived { get; set; } = true;

        private int nextJusticeID_callCount = 0;

        private enum VariationDocumentAction
        {
            AcceptVariationDocument,
            RejectVariationDocument
        }

        private VariationDocumentAction VariationAction { get; set; }

        public InterceptionManager(InterceptionApplicationData interception, IRepositories repositories,
                                   IRepositories_Finance repositoriesFinance, CustomConfig config) :
                                  base(interception, repositories, config, new InterceptionValidation(interception, repositories, config))
        {
            InterceptionApplication = interception;
            InterceptionValidation = Validation as InterceptionValidation;
            RepositoriesFinance = repositoriesFinance;

            // add Interception-specific state changes

            StateEngine.ValidStateChange[ApplicationState.APPLICATION_ACCEPTED_10].Add(ApplicationState.FINANCIAL_TERMS_VARIED_17);
            StateEngine.ValidStateChange[ApplicationState.APPLICATION_ACCEPTED_10].Add(ApplicationState.APPLICATION_SUSPENDED_35);

            StateEngine.ValidStateChange[ApplicationState.PARTIALLY_SERVICED_12].Add(ApplicationState.FINANCIAL_TERMS_VARIED_17);
            StateEngine.ValidStateChange[ApplicationState.PARTIALLY_SERVICED_12].Add(ApplicationState.APPLICATION_SUSPENDED_35);

            StateEngine.ValidStateChange.Add(ApplicationState.FINANCIAL_TERMS_VARIED_17, new List<ApplicationState> {
                            ApplicationState.INVALID_VARIATION_SOURCE_91,
                            ApplicationState.INVALID_VARIATION_FINTERMS_92,
                            ApplicationState.VALID_FINANCIAL_VARIATION_93
                        });

            StateEngine.ValidStateChange.Add(ApplicationState.AWAITING_DOCUMENTS_FOR_VARIATION_19, new List<ApplicationState> {
                            ApplicationState.PARTIALLY_SERVICED_12,
                            ApplicationState.MANUALLY_TERMINATED_14,
                            ApplicationState.EXPIRED_15,
                            ApplicationState.APPLICATION_SUSPENDED_35
                        });

            StateEngine.ValidStateChange.Add(ApplicationState.APPLICATION_SUSPENDED_35, new List<ApplicationState> {
                            ApplicationState.MANUALLY_TERMINATED_14,
                            ApplicationState.EXPIRED_15,
                            ApplicationState.FINANCIAL_TERMS_VARIED_17
                        });

            StateEngine.ValidStateChange.Add(ApplicationState.INVALID_VARIATION_SOURCE_91, new List<ApplicationState> {
                            ApplicationState.VALID_FINANCIAL_VARIATION_93
                        });

            StateEngine.ValidStateChange.Add(ApplicationState.INVALID_VARIATION_FINTERMS_92, new List<ApplicationState> {
                            ApplicationState.VALID_FINANCIAL_VARIATION_93
                        });

            StateEngine.ValidStateChange.Add(ApplicationState.VALID_FINANCIAL_VARIATION_93, new List<ApplicationState> {
                            ApplicationState.PARTIALLY_SERVICED_12,
                            ApplicationState.AWAITING_DOCUMENTS_FOR_VARIATION_19
                        });
        }

        public InterceptionManager(IRepositories repositories, IRepositories_Finance repositoriesFinance, CustomConfig config) :
            this(new InterceptionApplicationData(), repositories, repositoriesFinance, config)
        {

        }

        public bool LoadApplication(string enfService, string controlCode, bool loadFinancials)
        {
            bool isSuccess = base.LoadApplication(enfService, controlCode);

            if (isSuccess && loadFinancials)
            {
                var finTerms = Repositories.InterceptionRepository.GetInterceptionFinancialTerms(enfService, controlCode);
                InterceptionApplication.IntFinH = finTerms;

                if (finTerms != null)
                {
                    var holdbackConditions = Repositories.InterceptionRepository.GetHoldbackConditions(enfService, controlCode,
                                                                                                       finTerms.IntFinH_Dte);

                    InterceptionApplication.HldbCnd = holdbackConditions;
                }
            }

            return isSuccess;
        }

        public override bool LoadApplication(string enfService, string controlCode)
        {
            // get data from Appl
            return LoadApplication(enfService, controlCode, loadFinancials: true);
        }

        public override bool CreateApplication()
        {
            if (InterceptionApplication.AppCtgy_Cd != "I01")
            {
                InterceptionApplication.Messages.AddError($"Invalid category type ({InterceptionApplication.AppCtgy_Cd}) for interception.");
                return false;
            }

            if (InterceptionApplication.IntFinH is null)
            {
                InterceptionApplication.Messages.AddError("Missing financial terms");
                return false;
            }

            bool success = base.CreateApplication();

            if (success)
            {
                InterceptionApplication.IntFinH.ActvSt_Cd = "P";
                InterceptionApplication.IntFinH.IntFinH_VarIss_Dte = null;

                foreach (var sourceSpecificHoldback in InterceptionApplication.HldbCnd)
                    sourceSpecificHoldback.ActvSt_Cd = "P";

                if (InterceptionApplication.IntFinH.IntFinH_PerPym_Money.HasValue &&
                    (InterceptionApplication.IntFinH.IntFinH_PerPym_Money.Value == 0))
                    InterceptionApplication.IntFinH.IntFinH_PerPym_Money = null;

                if (InterceptionApplication.IntFinH.IntFinH_CmlPrPym_Ind.HasValue &&
                    (InterceptionApplication.IntFinH.IntFinH_CmlPrPym_Ind.Value != 1))
                    EventManager.AddEvent(EventCode.C51114_FINANCIAL_TERMS_INCLUDE_NONCUMULATIVE_PERIODIC_PAYMENTS);

                Repositories.InterceptionRepository.CreateInterceptionFinancialTerms(InterceptionApplication.IntFinH);

                Repositories.InterceptionRepository.CreateHoldbackConditions(InterceptionApplication.HldbCnd);

                InterceptionApplication.Messages.AddInformation(EventCode.C50620_VALID_APPLICATION);

                IncrementGarnSmry(isNewApplication: true);

                if (config.ESDsites.Contains(Appl_EnfSrv_Cd))
                    Repositories.InterceptionRepository.InsertESDrequired(Appl_EnfSrv_Cd, Appl_CtrlCd, ESDrequired.OriginalESDrequired);

                EventManager.SaveEvents();

            }

            return success;
        }

        public override void UpdateApplication()
        {
            base.UpdateApplication();

            Repositories.InterceptionRepository.UpdateInterceptionFinancialTerms(InterceptionApplication.IntFinH);

            Repositories.InterceptionRepository.UpdateHoldbackConditions(InterceptionApplication.HldbCnd);

        }

        public override void UpdateApplicationNoValidation()
        {
            base.UpdateApplicationNoValidation();

            Repositories.InterceptionRepository.UpdateInterceptionFinancialTerms(InterceptionApplication.IntFinH);

            Repositories.InterceptionRepository.UpdateHoldbackConditions(InterceptionApplication.HldbCnd);

        }

        public bool VaryApplication()
        {
            // ignore passed core information -- keep only new financials
            var applicationManagerCopy = new InterceptionManager(Repositories, RepositoriesFinance, config);

            if (!applicationManagerCopy.LoadApplication(Appl_EnfSrv_Cd, Appl_CtrlCd, loadFinancials: false))
            {
                EventManager.AddEvent(EventCode.C55000_INVALID_VARIATION);
                EventManager.SaveEvents();

                return false;
            }

            var summSmry = RepositoriesFinance.SummonsSummaryRepository.GetSummonsSummary(Appl_EnfSrv_Cd, Appl_CtrlCd).FirstOrDefault();
            if (summSmry.Start_Dte >= DateTime.Now)
            {
                EventManager.AddEvent(EventCode.C50881_CANNOT_VARY_TERMS_AT_THIS_TIME);
                EventManager.SaveEvents();

                return false;
            }

            if (!InterceptionValidation.ValidNewFinancialTerms(InterceptionApplication))
                return false;

            InterceptionApplication.IntFinH.ActvSt_Cd = "P";
            InterceptionApplication.IntFinH.IntFinH_LiStCd = 17;
            InterceptionApplication.IntFinH.Appl_CtrlCd = Appl_CtrlCd;
            InterceptionApplication.IntFinH.Appl_EnfSrv_Cd = Appl_EnfSrv_Cd;

            foreach (var sourceSpecific in InterceptionApplication.HldbCnd)
            {
                sourceSpecific.ActvSt_Cd = "P";
                sourceSpecific.HldbCnd_LiStCd = 17;
                sourceSpecific.Appl_CtrlCd = Appl_CtrlCd;
                sourceSpecific.Appl_EnfSrv_Cd = Appl_EnfSrv_Cd;
            }

            if (InterceptionApplication.IntFinH.IntFinH_CmlPrPym_Ind.HasValue &&
                (InterceptionApplication.IntFinH.IntFinH_CmlPrPym_Ind.Value != 1))
            {
                EventManager.AddEvent(EventCode.C51114_FINANCIAL_TERMS_INCLUDE_NONCUMULATIVE_PERIODIC_PAYMENTS);
            }

            SetNewStateTo(ApplicationState.FINANCIAL_TERMS_VARIED_17);

            if (InterceptionApplication.AppLiSt_Cd.NotIn(ApplicationState.INVALID_VARIATION_SOURCE_91,
                                                         ApplicationState.INVALID_VARIATION_FINTERMS_92))
            {
                UpdateApplicationNoValidation();

                EventManager.SaveEvents();

                InterceptionApplication.Messages.AddInformation(EventCode.C50620_VALID_APPLICATION);

                return true;
            }
            else
            {
                switch (InterceptionApplication.AppLiSt_Cd)
                {
                    case ApplicationState.INVALID_VARIATION_SOURCE_91:
                        InterceptionApplication.Messages.AddError(EventCode.C55002_INVALID_FINANCIAL_TERMS);
                        break;

                    case ApplicationState.INVALID_VARIATION_FINTERMS_92:
                        InterceptionApplication.Messages.AddError(EventCode.C55001_INVALID_SOURCE_HOLDBACK);
                        break;

                    default:
                        InterceptionApplication.Messages.AddError(EventCode.C55000_INVALID_VARIATION);
                        break;
                }

                // don't update the application in the database, but only save the events

                EventManager.SaveEvents();

                return false;
            }

        }

        public void FullyServiceApplication()
        {
            var applicationManagerCopy = new InterceptionManager(Repositories, RepositoriesFinance, config);

            if (!applicationManagerCopy.LoadApplication(Appl_EnfSrv_Cd, Appl_CtrlCd, loadFinancials: false))
            {
                // TODO: generate error that application does not exists
                return;
            }

            SetNewStateTo(ApplicationState.FULLY_SERVICED_13);

            UpdateApplicationNoValidation();

            EventManager.SaveEvents();
        }

        public bool CancelApplication()
        {
            var applicationManagerCopy = new InterceptionManager(Repositories, RepositoriesFinance, config);
            if (!applicationManagerCopy.LoadApplication(Appl_EnfSrv_Cd, Appl_CtrlCd))
            {
                InterceptionApplication.Messages.AddError($"No application was found in the database for {Appl_EnfSrv_Cd}-{Appl_CtrlCd}");
                return false;
            }

            InterceptionApplication.Appl_LastUpdate_Usr = Repositories.CurrentSubmitter;
            InterceptionApplication.Appl_LastUpdate_Dte = DateTime.Now;

            var interceptionCurrentlyInDB = applicationManagerCopy.InterceptionApplication;

            InterceptionApplication.Appl_Dbtr_Addr_Ln = interceptionCurrentlyInDB.Appl_Dbtr_Addr_Ln;
            InterceptionApplication.Appl_Dbtr_Addr_Ln1 = interceptionCurrentlyInDB.Appl_Dbtr_Addr_Ln1;
            InterceptionApplication.Appl_Dbtr_Addr_PCd = interceptionCurrentlyInDB.Appl_Dbtr_Addr_PCd;
            InterceptionApplication.Appl_Dbtr_Addr_PrvCd = interceptionCurrentlyInDB.Appl_Dbtr_Addr_PrvCd;
            InterceptionApplication.Appl_Dbtr_Addr_CityNme = interceptionCurrentlyInDB.Appl_Dbtr_Addr_CityNme;
            InterceptionApplication.Appl_Dbtr_Addr_CtryCd = interceptionCurrentlyInDB.Appl_Dbtr_Addr_CtryCd;

            if (interceptionCurrentlyInDB.ActvSt_Cd != "A")
            {
                EventManager.AddEvent(EventCode.C50841_CAN_ONLY_CANCEL_AN_ACTIVE_APPLICATION);
                return false;
            }

            SetNewStateTo(ApplicationState.MANUALLY_TERMINATED_14);

            UpdateApplicationNoValidation();

            EventManager.SaveEvents();

            InterceptionApplication.Messages.AddInformation(EventCode.C50620_VALID_APPLICATION);

            return true;
        }

        public bool AcceptGarnishee(bool isAutoAccept = false)
        {
            var applicationManagerCopy = new InterceptionManager(Repositories, RepositoriesFinance, config);

            // ignore passed core information -- keep only new financials

            if (!applicationManagerCopy.LoadApplication(Appl_EnfSrv_Cd, Appl_CtrlCd, loadFinancials: false))
            {
                return false;
            }

            var interceptionCurrentlyInDB = applicationManagerCopy.InterceptionApplication;

            AcceptedWithin30Days = true;

            if (!isAutoAccept)
            {
                var interceptionDB = Repositories.InterceptionRepository;

                bool isESDsite = IsESD_MEP(Appl_EnfSrv_Cd);
                DateTime garnisheeSummonsReceiptDate = interceptionDB.GetGarnisheeSummonsReceiptDate(Appl_EnfSrv_Cd, Appl_CtrlCd, isESDsite);

                var dateDiff = garnisheeSummonsReceiptDate - interceptionCurrentlyInDB.Appl_Lgl_Dte;
                if (dateDiff.HasValue && dateDiff.Value.Days > 30)
                {
                    AcceptedWithin30Days = false;
                    RejectInterception();

                    return false;
                }

                if (string.IsNullOrEmpty(InterceptionApplication.Appl_CommSubm_Text))
                    InterceptionApplication.Appl_CommSubm_Text = interceptionCurrentlyInDB.Appl_CommSubm_Text;

                InterceptionApplication.Appl_LastUpdate_Usr = Repositories.CurrentSubmitter;
                InterceptionApplication.Appl_LastUpdate_Dte = DateTime.Now;

                SetNewStateTo(ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7);

                InterceptionApplication.Appl_Rcptfrm_Dte = garnisheeSummonsReceiptDate;

                UpdateApplicationNoValidation();

                EventManager.SaveEvents();

                InterceptionApplication.Messages.AddInformation(EventCode.C50620_VALID_APPLICATION);

            }

            return true;

        }

        public bool AcceptVariationDocument(string comments)
        {
            var applicationManagerCopy = new InterceptionManager(Repositories, RepositoriesFinance, config);

            if (!applicationManagerCopy.LoadApplication(Appl_EnfSrv_Cd, Appl_CtrlCd, loadFinancials: false))
            {
                return false;
            }

            var interceptionCurrentlyInDB = applicationManagerCopy.InterceptionApplication;

            if (interceptionCurrentlyInDB.AppLiSt_Cd != ApplicationState.AWAITING_DOCUMENTS_FOR_VARIATION_19)
            {
                InvalidStateChange(interceptionCurrentlyInDB.AppLiSt_Cd, InterceptionApplication.AppLiSt_Cd);

                EventManager.SaveEvents();

                return false;
            }

            if (!string.IsNullOrEmpty(comments.Trim()))
                InterceptionApplication.Appl_CommSubm_Text = comments.Trim();

            InterceptionApplication.Appl_LastUpdate_Usr = Repositories.CurrentSubmitter;
            InterceptionApplication.Appl_LastUpdate_Dte = DateTime.Now;

            EventManager.AddEvent(EventCode.C51111_VARIATION_ACCEPTED);

            ChangeStateForFinancialTerms(oldState: "A", newState: "I", 12);
            ChangeStateForFinancialTerms(oldState: "P", newState: "A", 12);

            var activeFinTerms = Repositories.InterceptionRepository.GetInterceptionFinancialTerms(Appl_EnfSrv_Cd, Appl_CtrlCd, "A");

            VariationAction = VariationDocumentAction.AcceptVariationDocument;

            SetNewStateTo(ApplicationState.PARTIALLY_SERVICED_12);

            // refresh the amount owed values in SummSmry

            var amountOwedProcess = new AmountOwedProcess(Repositories, RepositoriesFinance);
            var (summSmryNewData, _) = amountOwedProcess.CalculateAndUpdateAmountOwedForVariation(Appl_EnfSrv_Cd, Appl_CtrlCd);

            // update application
            decimal preBalance = summSmryNewData.PreBalance;

            Repositories.InterceptionRepository.InsertBalanceSnapshot(Appl_EnfSrv_Cd, Appl_CtrlCd, preBalance,
                                                                      BalanceSnapshotChangeType.VARIATION_ACCEPTED,
                                                                      intFinH_Date: activeFinTerms.IntFinH_Dte);
            UpdateApplicationNoValidation();

            if (!string.IsNullOrEmpty(activeFinTerms.IntFinH_DefHldbAmn_Period))
            {
                var fixedAmountData = RepositoriesFinance.SummonsSummaryFixedAmountRepository.GetSummonsSummaryFixedAmount(Appl_EnfSrv_Cd, Appl_CtrlCd);
                if (fixedAmountData is null)
                {
                    var newFixedAmountRecalcDateTime = RecalculateFixedAmountRecalcDateAfterVariation(activeFinTerms, DateTime.Now);
                    RepositoriesFinance.SummonsSummaryFixedAmountRepository.CreateSummonsSummaryFixedAmount(Appl_EnfSrv_Cd, Appl_CtrlCd,
                                                                                                            newFixedAmountRecalcDateTime);
                }
            }
            else 
            {
                // RepositoriesFinance.SummonsSummaryFixedAmountRepository.DeleteSummSmryFixedAmountRecalcDate(Appl_EnfSrv_Cd, Appl_CtrlCd);
            }

            EventManager.SaveEvents();

            InterceptionApplication.Messages.AddInformation(EventCode.C50620_VALID_APPLICATION);

            return true;
        }

        private DateTime RecalculateFixedAmountRecalcDateAfterVariation(InterceptionFinancialHoldbackData newFinTerms, DateTime variationCalcDate)
        {
            DateTime fixedAmountRecalcDate = variationCalcDate;
            DateTime currDateTime = variationCalcDate;
            DateTime ctrlFaDtePayable = currDateTime;

            var summSmry = RepositoriesFinance.SummonsSummaryRepository.GetSummonsSummary(Appl_EnfSrv_Cd, Appl_CtrlCd);
            var workActSummons = RepositoriesFinance.ActiveSummonsRepository.GetActiveSummonsCore(ctrlFaDtePayable, Appl_EnfSrv_Cd, Appl_CtrlCd);

            if (workActSummons is not null)
            {
                var activeSummons = GetActiveSummonsForVariation(ctrlFaDtePayable, newFinTerms, summSmry.FirstOrDefault());
            }

            return fixedAmountRecalcDate;

        }
        /*


        If workActSummons.SummSmrySelectActSummons.Rows.Count > 0 Then

            ' get most recent active summon

            Dim activeSummonData As Common.ActiveSummonsesForDebtorData.ActiveSummonsesForDebtorRow = Nothing

            Dim activeSummons As Common.ActiveSummonsesForDebtorData = Nothing
            ' -- removed (need to use "in memory" intFinH): activeSummons = .GetActiveSummonsesForDebtor(Nothing, ctrlFaDtePayable, applEnfSrvCd, applCtrlCd)

            activeSummons = GetActiveSummonsForVariation(ctrlFaDtePayable, applicationData, intFinH, summSmry)

            If (activeSummons.ActiveSummonsesForDebtor.Rows.Count > 0) Then
                activeSummonData = activeSummons.ActiveSummonsesForDebtor.Item(0)
            End If

            If Not activeSummonData Is Nothing Then

                ' if payable date prior to variation, use system date as payable date

                Dim calcFaDtePayable As DateTime
                If (ctrlFaDtePayable > activeSummonData.Start_Dte) AndAlso
                   (Not activeSummonData.IsIntFinH_VarIss_DteNull) AndAlso
                   (ctrlFaDtePayable < activeSummonData.IntFinH_VarIss_Dte) Then
                    calcFaDtePayable = currDateTime
                Else
                    calcFaDtePayable = ctrlFaDtePayable
                End If

                ' if summon has been varied, use variation issue date of latest variation
                ' to determine current period

                Dim calcStartDate As DateTime
                If activeSummonData.IsIntFinH_VarIss_DteNull Then
                    calcStartDate = activeSummonData.Start_Dte
                Else
                    calcStartDate = activeSummonData.IntFinH_VarIss_Dte
                End If

                Dim finTermStartDate As DateTime = activeSummonData.Start_Dte
                Dim fixedAmountPrdFreqCd As String = String.Empty

                If Not activeSummonData.IsIntFinH_DefHldbAmn_PeriodNull Then
                    fixedAmountPrdFreqCd = activeSummonData.IntFinH_DefHldbAmn_Period
                End If

                ' DS 2011-03-25 FEAT 18
                ' adjust calcStartDate so that it matches the SummSmry StartDate (as if the periods
                ' had been calculated from that start date)
                Dim acceptedDate As DateTime = applicationData.Appl.Item(0).Appl_RecvAffdvt_Dte ' finTermStartDate.AddDays(-35)
                acceptedDate = acceptedDate.Date  ' Remove the timestamp and return only the date
                calcStartDate = AdjustStartDateBasedOnAcceptedDate(calcStartDate, acceptedDate, fixedAmountPrdFreqCd)

                ' determine current period

                Dim currentPeriod As Integer = CalculatePeriod(activeSummonData, calcFaDtePayable,
                                                               calcStartDate, fixedAmountPrdFreqCd)


                ' calculate summsmry recalc date

                FixedAmountRecalcDate = CalculateSummSmryRecalcDate(fixedAmountPrdFreqCd,
                                                                                     currentPeriod,
                                                                                     calcStartDate)


            End If

        End If

        Return FixedAmountRecalcDate

    End Function
         
         
         
         */

        private ActiveSummonsCoreData GetActiveSummonsForVariation(DateTime ctrlFaDtePayable, InterceptionFinancialHoldbackData intFinHdata, SummonsSummaryData summSmryData)
        {
            ActiveSummonsCoreData activeSummons = null;

            var appl = InterceptionApplication;

            if ((ctrlFaDtePayable >= summSmryData.Start_Dte) && (ctrlFaDtePayable <= summSmryData.End_Dte))
            {
                DateTime thisVarEnterDte;
                if (!intFinHdata.IntFinH_VarIss_Dte.HasValue)
                    if (intFinHdata.IntFinH_RcvtAffdvt_Dte.HasValue)
                        thisVarEnterDte = intFinHdata.IntFinH_RcvtAffdvt_Dte.Value;

                string hldbTypeCode = "T";
                if (!string.IsNullOrEmpty(intFinHdata.HldbTyp_Cd))
                    hldbTypeCode = intFinHdata.HldbTyp_Cd;

                activeSummons = new ActiveSummonsCoreData
                {

                };

            }

            return activeSummons;
        }

        /*
         
    Private Function GetActiveSummonsForVariation(ByVal ctrlFaDtePayable As DateTime,
                                                  ByVal applicationData As Common.ApplicationData,
                                                  ByVal intFinHData As Common.DefaultHoldbackData,
                                                  ByVal summSmryData As Common.SummSmryRecalcData) As Common.ActiveSummonsesForDebtorData

        Dim activeSummons As New Common.ActiveSummonsesForDebtorData

        ' ctrlFaDtePayable is between start and end date of SummSmry
        If ctrlFaDtePayable >= summSmry.Start_Dte And ctrlFaDtePayable <= summSmry.End_Dte Then

            With intFinH

                Dim thisVarEnterDte As DateTime
                If .IsIntFinH_VarIss_DteNull Then
                    If Not .IsIntFinH_RcvtAffdvt_DteNull Then
                        thisVarEnterDte = .IntFinH_RcvtAffdvt_Dte
                    End If
                End If

                Dim hldbTyp_Cd As String = "T"
                If Not .IsHldbTyp_CdNull Then
                    hldbTyp_Cd = .HldbTyp_Cd
                End If

                Dim mxmTtl_Money As Decimal = 0
                If Not .IsIntFinH_MxmTtl_MoneyNull Then
                    mxmTtl_Money = .IntFinH_MxmTtl_Money
                End If

                Dim perPym_Money As Decimal = 0
                If Not .IsIntFinH_PerPym_MoneyNull Then
                    perPym_Money = .IntFinH_PerPym_Money
                End If

                Dim cmlPrPym_Ind As Byte = 0
                If Not .IsIntFinH_CmlPrPym_IndNull Then
                    cmlPrPym_Ind = .IntFinH_CmlPrPym_Ind
                End If

                Dim defHldbPrcnt As Integer = 0
                If Not .IsIntFinH_DefHldbPrcntNull Then
                    defHldbPrcnt = .IntFinH_DefHldbPrcnt
                End If

                Dim defHldbAmn_Money As Decimal = 0
                If Not .IsIntFinH_DefHldbAmn_MoneyNull Then
                    defHldbAmn_Money = .IntFinH_DefHldbAmn_Money
                End If

                Dim varIss_Dte As DateTime
                If Not .IsIntFinH_VarIss_DteNull Then
                    varIss_Dte = .IntFinH_VarIss_Dte
                End If

                Dim PymPrCd As String = String.Empty
                If Not .IsPymPr_CdNull Then
                    PymPrCd = .PymPr_Cd
                End If

                Dim defHldbAmnPrCd As String = String.Empty
                If Not .IsIntFinH_DefHldbAmn_PeriodNull Then
                    defHldbAmnPrCd = .IntFinH_DefHldbAmn_Period
                End If

                activeSummons.ActiveSummonsesForDebtor.AddActiveSummonsesForDebtorRow(
                            Subm_SubmCd:=appl.Subm_SubmCd,
                            Appl_JusticeNr:=appl.Appl_JusticeNr,
                            IntFinH_LmpSum_Money:= .IntFinH_LmpSum_Money,
                            IntFinH_PerPym_Money:=perPym_Money,
                            IntFinH_MxmTtl_Money:=mxmTtl_Money,
                            PymPr_Cd:=PymPrCd,
                            IntFinH_CmlPrPym_Ind:=cmlPrPym_Ind,
                            HldbCtg_Cd:= .HldbCtg_Cd,
                            IntFinH_DefHldbPrcnt:=defHldbPrcnt,
                            IntFinH_DefHldbAmn_Money:=defHldbAmn_Money,
                            IntFinH_DefHldbAmn_Period:=defHldbAmnPrCd,
                            HldbTyp_Cd:=hldbTyp_Cd,
                            Start_Dte:=summSmry.Start_Dte,
                            FeeDivertedTtl_Money:=summSmry.FeeDivertedTtl_Money,
                            LmpSumDivertedTtl_Money:=summSmry.LmpSumDivertedTtl_Money,
                            PerPymDivertedTtl_Money:=summSmry.PerPymDivertedTtl_Money,
                            HldbAmtTtl_Money:=summSmry.HldbAmtTtl_Money,
                            Appl_TotalAmnt:=summSmry.Appl_TotalAmnt,
                            IntFinH_Dte:= .IntFinH_Dte,
                            End_Dte:=summSmry.End_Dte,
                            Appl_RecvAffdvt_Dte:=summSmry.Start_Dte,
                            IntFinH_VarIss_Dte:=varIss_Dte,
                            LmpSumOwedTtl_Money:=summSmry.LmpSumOwedTtl_Money,
                            PerPymOwedTtl_Money:=summSmry.PerPymOwedTtl_Money,
                            Appl_Enfsrv_cd:= .Appl_EnfSrv_Cd,
                            VarEnterDte:=thisVarEnterDte,
                            Appl_CtrlCd:= .Appl_CtrlCd
                        )

                'If (Not .IsIntFinH_VarIss_DteNull) Or (.IsIntFinH_RcvtAffdvt_DteNull) Then
                '    activeSummons.ActiveSummonsesForDebtor.Item(0).SetVarEnterDteNull()
                'End If

                If .IsIntFinH_VarIss_DteNull Then
                    activeSummons.ActiveSummonsesForDebtor.Item(0).SetIntFinH_VarIss_DteNull()
                End If

            End With

        End If

        Return activeSummons

    End Function
         
         */

        private void RejectInterception()
        {
            InterceptionApplication.Appl_LastUpdate_Usr = Repositories.CurrentSubmitter;
            InterceptionApplication.Appl_LastUpdate_Dte = DateTime.Now;

            SetNewStateTo(ApplicationState.APPLICATION_REJECTED_9);

            UpdateApplicationNoValidation();

            IncrementGarnSmry();

            EventManager.SaveEvents();

            InterceptionApplication.Messages.AddInformation(EventCode.C50620_VALID_APPLICATION);
        }

        private void IncrementGarnSmry(bool isNewApplication = false)
        {

            var garnSmryDB = RepositoriesFinance.GarnSummaryRepository;

            if (isNewApplication || InterceptionApplication.AppLiSt_Cd.In(ApplicationState.SIN_NOT_CONFIRMED_5,
                                                                          ApplicationState.APPLICATION_REJECTED_9,
                                                                          ApplicationState.APPLICATION_SUSPENDED_35))
            {
                int fiscalMonthCounter = DateTimeHelper.GetFiscalMonth(DateTime.Now);
                int fiscalYear = DateTimeHelper.GetFiscalYear(DateTime.Now);
                string enfOfficeCode = InterceptionApplication.Subm_SubmCd.Substring(3, 1);

                var garnSummaryData = garnSmryDB.GetGarnSummary(Appl_EnfSrv_Cd, enfOfficeCode, fiscalMonthCounter, fiscalYear);

                GarnSummaryData thisGarnSmryData;

                if (garnSummaryData.Count == 0)
                {
                    int totalActiveSummonsCount = Repositories.InterceptionRepository.GetTotalActiveSummons(Appl_EnfSrv_Cd, enfOfficeCode);
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

                    garnSmryDB.CreateGarnSummary(thisGarnSmryData);
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

                garnSmryDB.UpdateGarnSummary(thisGarnSmryData);

            }

        }

        private DateTime ChangeStateForFinancialTerms(string oldState, string newState, short intFinH_LifeStateCode)
        {

            var interceptionDB = Repositories.InterceptionRepository;

            var defaultHoldback = interceptionDB.GetInterceptionFinancialTerms(Appl_EnfSrv_Cd, Appl_CtrlCd, oldState);

            DateTime intFinH_Date = defaultHoldback.IntFinH_Dte;

            defaultHoldback.ActvSt_Cd = newState;

            defaultHoldback.IntFinH_LiStCd = intFinH_LifeStateCode;

            var sourceSpecificHoldbacks = interceptionDB.GetHoldbackConditions(Appl_EnfSrv_Cd, Appl_CtrlCd, intFinH_Date, oldState);

            foreach (var sourceSpecificHoldback in sourceSpecificHoldbacks)
            {
                sourceSpecificHoldback.ActvSt_Cd = newState;
                sourceSpecificHoldback.HldbCnd_LiStCd = 2; // valid
            }

            interceptionDB.UpdateInterceptionFinancialTerms(defaultHoldback);
            interceptionDB.UpdateHoldbackConditions(sourceSpecificHoldbacks);

            return intFinH_Date;

        }

        private void CreateSummonsSummary(string debtorID, string justiceSuffix, DateTime startDate)
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

            var existingData = RepositoriesFinance.SummonsSummaryRepository.GetSummonsSummary(Appl_EnfSrv_Cd, Appl_CtrlCd);

            if (existingData.Any())
                RepositoriesFinance.SummonsSummaryRepository.CreateSummonsSummary(summSummary);
            else
                // TODO: should we get a warning that instead of creating we are updating an existing summsmry?
                RepositoriesFinance.SummonsSummaryRepository.UpdateSummonsSummary(summSummary);
        }

        private void NotifyMatchingActiveApplications(EventCode eventCode)
        {
            var matchedApplications = Repositories.InterceptionRepository.FindMatchingActiveApplications(Appl_EnfSrv_Cd, Appl_CtrlCd,
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

        private void StopBlockFunds(ApplicationState fromState)
        {
            string debtorID = GetDebtorID(InterceptionApplication.Appl_JusticeNr);
            int summSmryCount = 0;

            var summSmryInfoForDebtor = RepositoriesFinance.SummonsSummaryRepository.GetSummonsSummary(debtorId: debtorID);

            foreach (var summSmryInfo in summSmryInfoForDebtor)
            {
                if (summSmryInfo.ActualEnd_Dte.HasValue)
                    summSmryCount++;
            }

            summSmryCount--;

            if (summSmryCount <= 0)
                EventManager.AddEvent(EventCode.C56003_CANCELLED_OR_COMPLETED_BFN, queue: EventQueue.EventBFN);

            var summSmryForCurrentAppl = RepositoriesFinance.SummonsSummaryRepository.GetSummonsSummary(Appl_EnfSrv_Cd, Appl_CtrlCd).FirstOrDefault();

            switch (fromState)
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

            RepositoriesFinance.SummonsSummaryRepository.UpdateSummonsSummary(summSmryForCurrentAppl);

            Repositories.InterceptionRepository.EISOHistoryDeleteBySIN(InterceptionApplication.Appl_Dbtr_Cnfrmd_SIN, false);

        }

        public override void ProcessBringForwards(ApplicationEventData bfEvent)
        {
            bool closeEvent = false;

            TimeSpan diff;
            if (InterceptionApplication.Appl_LastUpdate_Dte.HasValue)
                diff = InterceptionApplication.Appl_LastUpdate_Dte.Value - DateTime.Now;
            else
                diff = TimeSpan.Zero;

            if ((InterceptionApplication.ActvSt_Cd != "A") &&
                ((!bfEvent.Event_Reas_Cd.HasValue) || (
                 (bfEvent.Event_Reas_Cd.NotIn(EventCode.C50806_SCHEDULED_TO_BE_REINSTATED__QUARTERLY_TRACING,
                                              EventCode.C50680_CHANGE_OR_SUPPLY_ADDITIONAL_DEBTOR_INFORMATION_SEE_SIN_VERIFICATION_RESULTS_PAGE_IN_FOAEA_FOR_SPECIFIC_DETAILS,
                                              EventCode.C50600_INVALID_APPLICATION)))) &&
                ((diff.Equals(TimeSpan.Zero)) || (Math.Abs(diff.TotalHours) > 24)))
            {
                bfEvent.AppLiSt_Cd = ApplicationState.MANUALLY_TERMINATED_14;
                bfEvent.ActvSt_Cd = "I";

                EventManager.SaveEvent(bfEvent);
            }
            else
            {
                if (bfEvent.Event_Reas_Cd.HasValue)
                {
                    var dbNotification = Repositories.NotificationRepository;
                    switch (bfEvent.Event_Reas_Cd)
                    {
                        case EventCode.C54005_CREATE_A_DEBTOR_LETTER_EVENT_IN_EVNTDBTR:
                            if (InterceptionApplication.Appl_Rcptfrm_Dte.AddDays(20) < DateTime.Now)
                            {
                                dbNotification.SendEmail("Debtor Letter create date exceeds 20 day range",
                                    "", // System.Configuration.ConfigurationManager.AppSettings("EmailRecipients")
                                    $"Debtor letter created for I01 Application {Appl_EnfSrv_Cd} {Appl_CtrlCd} more than 20 days after Garnishee Summons Receipt Date. \n" +
                                    $"Debtor name: {InterceptionApplication.Appl_Dbtr_SurNme}, {InterceptionApplication.Appl_Dbtr_FrstNme}\n" +
                                    $"Justice ID: {InterceptionApplication.Appl_JusticeNr}\n" +
                                    $"Garnishee Summons Receipt Date: {InterceptionApplication.Appl_Rcptfrm_Dte.ToShortDateString()}\n" +
                                    $"Debtor letter create date: {DateTime.Now.ToShortDateString()}"
                                    );
                            }
                            EventManager.AddEvent(EventCode.UNDEFINED, queue: EventQueue.EventDbtr); // ???
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
                bfEvent.AppLiSt_Cd = InterceptionApplication.AppLiSt_Cd;
                bfEvent.ActvSt_Cd = "C";

                EventManager.SaveEvent(bfEvent);
            }

            EventManager.SaveEvents();
        }
    }
}
