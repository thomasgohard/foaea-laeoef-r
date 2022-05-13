using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using System;
using System.Linq;

namespace FOAEA3.Business.Areas.Application
{
    internal class InterceptionValidation : ApplicationValidation
    {
        private InterceptionApplicationData InterceptionApplication { get; }


        public InterceptionValidation(InterceptionApplicationData interceptionApplication, ApplicationEventManager eventManager,
                                      IRepositories repositories, CustomConfig config) : base(interceptionApplication, eventManager, repositories, config)
        {
            InterceptionApplication = interceptionApplication;
        }

        public InterceptionValidation(InterceptionApplicationData interceptionApplication, IRepositories repositories,
                                      CustomConfig config) : base(interceptionApplication, repositories, config)
        {
            InterceptionApplication = interceptionApplication;
        }

        public bool ValidVariationDefaultHoldbacks()
        {
            bool isValid = true;

            if (!ValidDefaultHoldbackAmount())
                isValid = false;

            if (!ValidVariationIssueDate())
                isValid = false;

            if (!ValidPaymentPeriodicCode())
                isValid = false;

            if (!ValidPeriodicPaymentAmount())
                isValid = false;

            if (!ValidLumpSumAmount())
                isValid = false;

            var intFinHdata = InterceptionApplication.IntFinH;
            var sourceSpecificData = InterceptionApplication.HldbCnd;

            decimal defHldbAmount = intFinHdata.IntFinH_DefHldbAmn_Money ?? 0.0M;
            int defHldbPercent = intFinHdata.IntFinH_DefHldbPrcnt ?? 0;

            if (!ValidHoldbackCategory(intFinHdata.HldbCtg_Cd, defHldbAmount, defHldbPercent))
                isValid = false;

            if (!ValidHoldbackTypeCode(intFinHdata.HldbTyp_Cd, defHldbAmount, defHldbPercent))
                isValid = false;

            return isValid;
        }

        public bool ValidVariationSourceSpecificHoldbacks()
        {
            bool isValid = true;

            foreach (var sourceSpecificData in InterceptionApplication.HldbCnd)
            {
                decimal mxmPerChequeAmount = sourceSpecificData.HldbCnd_MxmPerChq_Money ?? 0.0M;
                decimal sourceHoldbackAmount = sourceSpecificData.HldbCnd_SrcHldbAmn_Money ?? 0.0M;
                int sourceHoldbackPercent = sourceSpecificData.HldbCnd_SrcHldbPrcnt ?? 0;

                if (string.IsNullOrEmpty(sourceSpecificData.EnfSrv_Cd) &&
                    ((mxmPerChequeAmount > 0.0M) || !string.IsNullOrEmpty(sourceSpecificData.HldbCtg_Cd) ||
                     (sourceHoldbackPercent > 0) || (sourceHoldbackAmount > 0.0M)))
                {
                    EventManager.AddEvent(EventCode.C55201_SOURCE_ID_MUST_BE_ENTERED_WHEN_OTHER_FIELDS_ARE_FILLED);
                    isValid = false;
                }

                if (!ValidHoldbackCategory(sourceSpecificData.HldbCtg_Cd, sourceHoldbackAmount, sourceHoldbackPercent))
                    isValid = false;
            }

            return isValid;
        }

        public void CheckCreditorSurname()
        {
            var applications = Repositories.InterceptionRepository.GetSameCreditorForI01(InterceptionApplication.Appl_CtrlCd,
                                                                                         InterceptionApplication.Subm_SubmCd,
                                                                                         InterceptionApplication.Appl_Dbtr_Entrd_SIN,
                                                                                         InterceptionApplication.Appl_SIN_Cnfrmd_Ind,
                                                                                         InterceptionApplication.ActvSt_Cd);

            if (applications.Count > 0)
            {
                string errorMessage = string.Empty;
                foreach (var application in applications)
                {
                    if (!string.IsNullOrEmpty(application.Appl_Crdtr_SurNme) && !string.IsNullOrEmpty(InterceptionApplication.Appl_Crdtr_SurNme))
                    {
                        if (application.Appl_Crdtr_SurNme.Trim().ToUpper() == InterceptionApplication.Appl_Crdtr_SurNme.Trim().ToUpper())
                            errorMessage += $"{application.Appl_EnfSrv_Cd.Trim()}-{application.Subm_SubmCd.Trim()}-{application.Appl_CtrlCd.Trim()} ";
                    }
                }

                if (!string.IsNullOrEmpty(errorMessage))
                    EventManager.AddEvent(EventCode.C50932_THERE_EXISTS_ONE_OR_MORE_ACTIVE_APPLICATIONS_OF_THIS_TYPE_FOR_THE_SAME_DEBTOR___CREDITOR,
                                          eventReasonText: errorMessage.Trim());
            }

        }

        public bool ValidNewFinancialTerms(InterceptionApplicationData currentInterceptionApplication)
        {
            var newFinancialTerms = InterceptionApplication.IntFinH;
            var newSourceSpecificHoldbacks = InterceptionApplication.HldbCnd;

            var currentFinancialTerms = currentInterceptionApplication.IntFinH;

            var holdbackDate = newFinancialTerms.IntFinH_Dte;

            foreach (var holdback in newSourceSpecificHoldbacks)
            {
                if (holdback.IntFinH_Dte != holdbackDate)
                {
                    InterceptionApplication.Messages.AddError("Source specific holdback has different IntFinH Date than IntFinH row");
                    return false;
                }
            }

            if (newFinancialTerms.IntFinH_Dte < currentFinancialTerms.IntFinH_Dte)
            {
                InterceptionApplication.Messages.AddError("New holdback is dated earlier than the active holdback currently in the system");
                return false;
                // TODO: ask Eric if it is ok for new intfinh to be dated prior to non-active ("I") intfinh?
            }

            if (newFinancialTerms.IntFinH_Dte == currentFinancialTerms.IntFinH_Dte)
            {
                InterceptionApplication.Messages.AddError("New holdback is a duplicate to existing one");
                return false;
                // TODO: ask Eric if it is ok for a new variation to be posted with the same date as an existing active variation, but with different terms?
            }

            return true;

        }



        private bool ValidVariationIssueDate()
        {

            bool isValid = InterceptionApplication.IntFinH.IntFinH_VarIss_Dte.HasValue &&
                           (InterceptionApplication.IntFinH.IntFinH_VarIss_Dte.Value > DateTime.Now);

            if (!isValid)
                EventManager.AddEvent(EventCode.C55101_INVALID_VARIATION_ISSUE_DATE);

            return isValid;
        }

        private bool HasPeriodPaymentAmount()
        {
            return InterceptionApplication.IntFinH.IntFinH_PerPym_Money.HasValue && (InterceptionApplication.IntFinH.IntFinH_PerPym_Money.Value > 0.0M);
        }

        private bool ValidLumpSumAmount()
        {
            bool isValid = true;

            decimal maxTotalMoney = InterceptionApplication.IntFinH.IntFinH_MxmTtl_Money ?? 0.0M;

            decimal maxAmntPeriodic = CalculateMaxAmountPeriodicForPeriodCode(InterceptionApplication.IntFinH.PymPr_Cd);

            decimal maxAmnt = maxAmntPeriodic * InterceptionApplication.IntFinH.IntFinH_LmpSum_Money;

            if (maxTotalMoney > maxAmnt)
            {
                EventManager.AddEvent(EventCode.C55105_MAXIMUM_PERIODIC_TOTAL_INCONSISTENT_WITH_PERIODIC_PAYMENTS);
                isValid = false;
            }

            return isValid;
        }

        private bool ValidPaymentPeriodicCode()
        {
            bool isValid = true;

            if (!string.IsNullOrEmpty(InterceptionApplication.IntFinH.PymPr_Cd))
            {
                if (!ValidPaymentPeriod(InterceptionApplication.IntFinH.PymPr_Cd))
                {
                    EventManager.AddEvent(EventCode.C55102_INVALID_PERIODIC_INDICATOR);
                    isValid = false;
                }
                if (!HasPeriodPaymentAmount())
                {
                    EventManager.AddEvent(EventCode.C55103_PERIODIC_INDICATOR_INCONSISTENT_WITH_SUMMONS_PAYMENT_AMOUNT);
                    isValid = false;
                }
            }
            else if (HasPeriodPaymentAmount())
            {
                EventManager.AddEvent(EventCode.C55103_PERIODIC_INDICATOR_INCONSISTENT_WITH_SUMMONS_PAYMENT_AMOUNT);
                isValid = false;
            }

            return isValid;
        }
        private decimal CalculateMaxAmountPeriodicForPeriodCode(string paymentPeriodicCode)
        {
            decimal maxAmntPeriodic = 0.0M;

            if (InterceptionApplication.IntFinH.IntFinH_PerPym_Money is null)
            {
                ApplicationManager.AddSystemError(Repositories, InterceptionApplication.Messages, config.SystemErrorRecipients,
                                                  $"CalculateMaxAmountPeriodicForPeriodCode for {InterceptionApplication.Appl_EnfSrv_Cd}-{InterceptionApplication.Appl_CtrlCd}" +
                                                  $" (with periodic code {paymentPeriodicCode}) was called even though IntFinH_PerPym_Money is null!");
                return 0.0M;
            }

            if (!string.IsNullOrEmpty(paymentPeriodicCode))
            {
                switch (paymentPeriodicCode)
                {
                    case PaymentPeriodicCode.WEEKLY:
                        maxAmntPeriodic = InterceptionApplication.IntFinH.IntFinH_PerPym_Money.Value * 261;
                        break;
                    case PaymentPeriodicCode.BIWEEKLY:
                        maxAmntPeriodic = InterceptionApplication.IntFinH.IntFinH_PerPym_Money.Value * 131;
                        break;
                    case PaymentPeriodicCode.MONTHLY:
                        maxAmntPeriodic = InterceptionApplication.IntFinH.IntFinH_PerPym_Money.Value * 60;
                        break;
                    case PaymentPeriodicCode.QUARTERLY:
                        maxAmntPeriodic = InterceptionApplication.IntFinH.IntFinH_PerPym_Money.Value * 20;
                        break;
                    case PaymentPeriodicCode.SEMI_ANNUALLY:
                        maxAmntPeriodic = InterceptionApplication.IntFinH.IntFinH_PerPym_Money.Value * 10;
                        break;
                    case PaymentPeriodicCode.ANNUALLY:
                        maxAmntPeriodic = InterceptionApplication.IntFinH.IntFinH_PerPym_Money.Value * 5;
                        break;
                    case PaymentPeriodicCode.SEMI_MONTHLY:
                        maxAmntPeriodic = InterceptionApplication.IntFinH.IntFinH_PerPym_Money.Value * 120;
                        break;
                }
            }

            return maxAmntPeriodic;

        }

        private bool ValidHoldbackCategory(string categoryCode, decimal holdbackAmount, int holdbackPercent)
        {
            bool isValid = true;

            switch (categoryCode)
            {
                case HoldbackCategoryCode.NO_HOLDBACK:
                    if (holdbackAmount > 0.0M)
                    {
                        EventManager.AddEvent(EventCode.C55107_HOLDBACK_AMOUNT_INCONSISTENT_WITH_OTHER_HOLDBACK_FIELDS);
                        isValid = false;
                    }
                    if (holdbackPercent > 0)
                    {
                        EventManager.AddEvent(EventCode.C55106_HOLDBACK_PERC_INCONSISTENT_WITH_OTHER_HOLDBACK_FIELDS);
                        isValid = false;
                    }
                    break;
                case HoldbackCategoryCode.PERCENTAGE:
                    if (holdbackAmount > 0.0M)
                    {
                        EventManager.AddEvent(EventCode.C55107_HOLDBACK_AMOUNT_INCONSISTENT_WITH_OTHER_HOLDBACK_FIELDS);
                        isValid = false;
                    }
                    if (holdbackPercent == 0)
                    {
                        EventManager.AddEvent(EventCode.C55106_HOLDBACK_PERC_INCONSISTENT_WITH_OTHER_HOLDBACK_FIELDS);
                        isValid = false;
                    }
                    break;
                case HoldbackCategoryCode.FIXED_AMOUNT:
                    if (holdbackAmount == 0.0M)
                    {
                        EventManager.AddEvent(EventCode.C55107_HOLDBACK_AMOUNT_INCONSISTENT_WITH_OTHER_HOLDBACK_FIELDS);
                        isValid = false;
                    }
                    if (holdbackPercent > 0)
                    {
                        EventManager.AddEvent(EventCode.C55106_HOLDBACK_PERC_INCONSISTENT_WITH_OTHER_HOLDBACK_FIELDS);
                        isValid = false;
                    }
                    break;
            }

            return isValid;
        }

        private bool ValidDefaultHoldbackAmount()
        {
            bool isValid = true;

            decimal defHldbAmount = InterceptionApplication.IntFinH.IntFinH_DefHldbAmn_Money ?? 0.0M;

            string defHldbAmountPr = InterceptionApplication.IntFinH.IntFinH_DefHldbAmn_Period ?? string.Empty;

            if ((defHldbAmount > 0.0M) && string.IsNullOrEmpty(defHldbAmountPr))
            {
                EventManager.AddEvent(EventCode.C55107_HOLDBACK_AMOUNT_INCONSISTENT_WITH_OTHER_HOLDBACK_FIELDS);
                isValid = false;
            }

            return isValid;
        }

        private bool ValidPeriodicPaymentAmount()
        {
            bool isValid = true;

            decimal maxTotalMoney = InterceptionApplication.IntFinH.IntFinH_MxmTtl_Money ?? 0.0M;

            if ((maxTotalMoney > 0.0M) && InterceptionApplication.IntFinH.IntFinH_PerPym_Money.HasValue &&
                                          (InterceptionApplication.IntFinH.IntFinH_PerPym_Money.Value < 0.0M))
            {
                EventManager.AddEvent(EventCode.C55104_MAXIMUM_PERIODIC_AMOUNT_INCONSISTENT_WITH_OTHER_FIELDS);
                isValid = false;
            }

            return isValid;
        }


        private bool ValidPaymentPeriod(string paymentPeriodCode)
        {
            var paymentPeriods = Repositories.InterceptionRepository.GetPaymentPeriods();
            return paymentPeriods.Any(m => (m.PymPr_Cd == paymentPeriodCode.ToUpper()) && (m.ActvSt_Cd == "A"));
        }

        private bool ValidHoldbackType(string holdbackTypeCode)
        {
            var holdbackTypes = Repositories.InterceptionRepository.GetHoldbackTypes();
            return holdbackTypes.Any(m => (m.HldbTyp_Cd == holdbackTypeCode.ToUpper()) && (m.ActvSt_Cd == "A"));
        }

        private bool ValidHoldbackTypeCode(string holdbackTypeCode, decimal defHldbAmount, int defHldbPercent)
        {
            bool isValid = true;

            if (!string.IsNullOrEmpty(holdbackTypeCode))
            {
                if (!ValidHoldbackType(holdbackTypeCode))
                {
                    EventManager.AddEvent(EventCode.C55108_INVALID_HOLDBACK_CATEGORY_CODES);
                    isValid = false;
                }
            }

            const string PER_TRANSACTION = "P";
            if ((holdbackTypeCode == PER_TRANSACTION) && (InterceptionApplication.IntFinH.HldbCtg_Cd.In(HoldbackCategoryCode.NO_HOLDBACK,
                                                                                    HoldbackCategoryCode.PERCENTAGE) ||
                                                         string.IsNullOrEmpty(InterceptionApplication.IntFinH.IntFinH_DefHldbAmn_Period) ||
                                                         (defHldbAmount <= 0.0M) ||
                                                         ((defHldbAmount > 0.0M) && (defHldbPercent > 0))))
            {
                EventManager.AddEvent(EventCode.C55109_HOLDBACK_TYPE_CODE_INCONSISTENT_WITH_OTHER_HOLDBACK_FIELDS);
                isValid = false;
            }

            return isValid;
        }

    }
}
