using DBHelper;
using FOAEA3.Business.BackendProcesses;
using FOAEA3.Common.Helpers;
using FOAEA3.Data.DB;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.Business.Areas.Application
{
    internal partial class InterceptionManager : ApplicationManager
    {
        private const string I01_AFFITDAVIT_DOCUMENT_CODE = "IXX";
        private const string AUTO_REJECT_EXPIRED_TIME_FOR_VARIATION = "BFEventsProcessing Case 50896";

        public InterceptionApplicationData InterceptionApplication { get; }
        private IRepositories_Finance DBfinance { get; }
        private InterceptionValidation InterceptionValidation { get; }
        public bool? AcceptedWithin30Days { get; set; } = null;
        public bool ESDReceived { get; set; } = true;
        public DateTime? GarnisheeSummonsReceiptDate { get; set; }

        private int nextJusticeID_callCount = 0;

        private enum VariationDocumentAction
        {
            AcceptVariationDocument,
            RejectVariationDocument
        }

        private VariationDocumentAction VariationAction { get; set; }

        public InterceptionManager(InterceptionApplicationData interception, IRepositories repositories,
                                   IRepositories_Finance repositoriesFinance, IFoaeaConfigurationHelper config) :
                                  base(interception, repositories, config,
                                      new InterceptionValidation(interception, repositories, config, null))
        {
            InterceptionApplication = interception;
            InterceptionValidation = Validation as InterceptionValidation;
            DBfinance = repositoriesFinance;

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

        public InterceptionManager(IRepositories repositories, IRepositories_Finance repositoriesFinance, IFoaeaConfigurationHelper config) :
            this(new InterceptionApplicationData(), repositories, repositoriesFinance, config)
        {

        }

        public async Task<bool> LoadApplicationAsync(string enfService, string controlCode, bool loadFinancials)
        {
            bool isSuccess = await base.LoadApplicationAsync(enfService, controlCode);

            InterceptionApplication.IntFinH = null;
            InterceptionApplication.HldbCnd = null;
            if (isSuccess && loadFinancials)
            {
                var finTerms = await DB.InterceptionTable.GetInterceptionFinancialTermsAsync(enfService, controlCode);
                InterceptionApplication.IntFinH = finTerms;

                if (finTerms != null)
                {
                    var holdbackConditions = await DB.InterceptionTable.GetHoldbackConditionsAsync(enfService, controlCode,
                                                                                                       finTerms.IntFinH_Dte);

                    InterceptionApplication.HldbCnd = holdbackConditions;
                }
            }

            return isSuccess;
        }

        public override async Task<bool> LoadApplicationAsync(string enfService, string controlCode)
        {
            // get data from Appl
            return await LoadApplicationAsync(enfService, controlCode, loadFinancials: true);
        }

        public override async Task<bool> CreateApplicationAsync()
        {
            if (!IsValidCategory("I01"))
                return false;

            if (InterceptionApplication.IntFinH is null)
            {
                InterceptionApplication.Messages.AddError("Missing financial terms");
                return false;
            }

            bool success = await base.CreateApplicationAsync();

            if (success)
            {
                InterceptionApplication.IntFinH.Appl_EnfSrv_Cd = InterceptionApplication.Appl_EnfSrv_Cd;
                InterceptionApplication.IntFinH.Appl_CtrlCd = InterceptionApplication.Appl_CtrlCd;
                InterceptionApplication.IntFinH.ActvSt_Cd = "P";
                InterceptionApplication.IntFinH.IntFinH_VarIss_Dte = null;

                foreach (var sourceSpecificHoldback in InterceptionApplication.HldbCnd)
                {
                    sourceSpecificHoldback.Appl_EnfSrv_Cd = InterceptionApplication.Appl_EnfSrv_Cd;
                    sourceSpecificHoldback.Appl_CtrlCd = InterceptionApplication.Appl_CtrlCd;
                    sourceSpecificHoldback.ActvSt_Cd = "P";
                }

                if (InterceptionApplication.IntFinH.IntFinH_PerPym_Money.HasValue &&
                    (InterceptionApplication.IntFinH.IntFinH_PerPym_Money.Value == 0))
                    InterceptionApplication.IntFinH.IntFinH_PerPym_Money = null;

                if (InterceptionApplication.IntFinH.IntFinH_CmlPrPym_Ind.HasValue &&
                    (InterceptionApplication.IntFinH.IntFinH_CmlPrPym_Ind.Value != 1))
                    EventManager.AddEvent(EventCode.C51114_FINANCIAL_TERMS_INCLUDE_NONCUMULATIVE_PERIODIC_PAYMENTS);

                await DB.InterceptionTable.CreateInterceptionFinancialTermsAsync(InterceptionApplication.IntFinH);

                await DB.InterceptionTable.CreateHoldbackConditionsAsync(InterceptionApplication.HldbCnd);

                if (InterceptionApplication.Medium_Cd != "FTP") InterceptionApplication.Messages.AddInformation(EventCode.C50620_VALID_APPLICATION);

                await IncrementGarnSmryAsync(isNewApplication: true);

                if (Config.ESDsites.Contains(Appl_EnfSrv_Cd) && (InterceptionApplication.Medium_Cd == "FTP"))
                    await DB.InterceptionTable.InsertESDrequiredAsync(Appl_EnfSrv_Cd, Appl_CtrlCd, ESDrequired.OriginalESDrequired);

                await EventManager.SaveEventsAsync();

            }

            return success;
        }

        public override async Task UpdateApplicationAsync()
        {
            InterceptionApplication.Appl_LastUpdate_Usr = DB.UpdateSubmitter;
            InterceptionApplication.Appl_LastUpdate_Dte = DateTime.Now;

            await base.UpdateApplicationAsync();
        }


        public async Task UpdateApplicationNoValidationNoFinTermsAsync()
        {
            await base.UpdateApplicationNoValidationAsync();
        }

        public override async Task UpdateApplicationNoValidationAsync()
        {
            await base.UpdateApplicationNoValidationAsync();

            if ((InterceptionApplication.IntFinH is not null) && (InterceptionApplication.IntFinH.IntFinH_Dte != DateTime.MinValue))
            {
                await DB.InterceptionTable.UpdateInterceptionFinancialTermsAsync(InterceptionApplication.IntFinH);

                if ((InterceptionApplication.HldbCnd is not null) && (InterceptionApplication.HldbCnd.Count > 0))
                    await DB.InterceptionTable.UpdateHoldbackConditionsAsync(InterceptionApplication.HldbCnd);
            }
        }

        public async Task<bool> VaryApplicationAsync()
        {
            if (!IsValidCategory("I01"))
                return false;

            // only keep changes that are allowed:
            //   comment and financial terms

            string appl_CommSubm_Text = InterceptionApplication.Appl_CommSubm_Text;
            var newIntFinH = InterceptionApplication.IntFinH;
            var newHldbCnd = InterceptionApplication.HldbCnd;

            // for FTP, also keep changes to address, phone # and ref code:
            var newAppl_Source_RfrNr = InterceptionApplication.Appl_Source_RfrNr;
            var newAppl_Dbtr_Addr_Ln = InterceptionApplication.Appl_Dbtr_Addr_Ln;
            var newAppl_Dbtr_Addr_Ln1 = InterceptionApplication.Appl_Dbtr_Addr_Ln1;
            var newAppl_Dbtr_Addr_CityNme = InterceptionApplication.Appl_Dbtr_Addr_CityNme;
            var newAppl_Dbtr_Addr_PrvCd = InterceptionApplication.Appl_Dbtr_Addr_PrvCd;
            var newAppl_Dbtr_Addr_CtryCd = InterceptionApplication.Appl_Dbtr_Addr_CtryCd;
            var newAppl_Dbtr_Addr_PCd = InterceptionApplication.Appl_Dbtr_Addr_PCd;

            if (!await LoadApplicationAsync(Appl_EnfSrv_Cd, Appl_CtrlCd, loadFinancials: false))
            {
                EventManager.AddEvent(EventCode.C55000_INVALID_VARIATION);
                await EventManager.SaveEventsAsync();

                return false;
            }

            InterceptionApplication.Appl_LastUpdate_Usr = DB.CurrentSubmitter;
            InterceptionApplication.Appl_LastUpdate_Dte = DateTime.Now;

            InterceptionApplication.Appl_CommSubm_Text = appl_CommSubm_Text ?? InterceptionApplication.Appl_CommSubm_Text;
            InterceptionApplication.IntFinH = newIntFinH;
            InterceptionApplication.HldbCnd = newHldbCnd;

            if (InterceptionApplication.Medium_Cd == "FTP")
            {
                InterceptionApplication.Appl_LastUpdate_Usr = "FO2SSS";

                InterceptionApplication.Appl_Source_RfrNr = newAppl_Source_RfrNr;
                InterceptionApplication.Appl_Dbtr_Addr_Ln = newAppl_Dbtr_Addr_Ln;
                InterceptionApplication.Appl_Dbtr_Addr_Ln1 = newAppl_Dbtr_Addr_Ln1;
                InterceptionApplication.Appl_Dbtr_Addr_CityNme = newAppl_Dbtr_Addr_CityNme;
                InterceptionApplication.Appl_Dbtr_Addr_PrvCd = newAppl_Dbtr_Addr_PrvCd;
                InterceptionApplication.Appl_Dbtr_Addr_CtryCd = newAppl_Dbtr_Addr_CtryCd;
                InterceptionApplication.Appl_Dbtr_Addr_PCd = newAppl_Dbtr_Addr_PCd;
            }

            var summSmry = (await DBfinance.SummonsSummaryRepository.GetSummonsSummaryAsync(Appl_EnfSrv_Cd, Appl_CtrlCd)).FirstOrDefault();
            if (summSmry is null)
            {
                await AddSystemErrorAsync(DB, InterceptionApplication.Messages, Config.Recipients.EmailRecipients,
                               $"No summsmry record was found for {Appl_EnfSrv_Cd}-{Appl_CtrlCd}. Cannot vary.");
                return false;
            }

            if (summSmry.Start_Dte >= DateTime.Now)
            {
                EventManager.AddEvent(EventCode.C50881_CANNOT_VARY_TERMS_AT_THIS_TIME);
                await EventManager.SaveEventsAsync();

                return false;
            }


            var currentInterceptionManager = new InterceptionManager(DB, DBfinance, Config)
            {
                CurrentUser = this.CurrentUser
            };
            await currentInterceptionManager.LoadApplicationAsync(Appl_EnfSrv_Cd, Appl_CtrlCd, loadFinancials: true);
            var currentInterceptionApplication = currentInterceptionManager.InterceptionApplication;

            if (!StateEngine.IsValidStateChange(currentInterceptionManager.InterceptionApplication.AppLiSt_Cd, ApplicationState.FINANCIAL_TERMS_VARIED_17))
            {
                InvalidStateChange(currentInterceptionManager.InterceptionApplication.AppLiSt_Cd, ApplicationState.FINANCIAL_TERMS_VARIED_17);
                await EventManager.SaveEventsAsync();

                return false;
            }

            if (!InterceptionValidation.ValidNewFinancialTerms(currentInterceptionApplication))
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

            await SetNewStateTo(ApplicationState.FINANCIAL_TERMS_VARIED_17);

            if (InterceptionApplication.AppLiSt_Cd.NotIn(ApplicationState.INVALID_VARIATION_SOURCE_91,
                                                         ApplicationState.INVALID_VARIATION_FINTERMS_92))
            {
                await DB.InterceptionTable.CreateInterceptionFinancialTermsAsync(InterceptionApplication.IntFinH);
                await DB.InterceptionTable.CreateHoldbackConditionsAsync(InterceptionApplication.HldbCnd);

                await UpdateApplicationNoValidationNoFinTermsAsync();

                await EventManager.SaveEventsAsync();

                if (InterceptionApplication.Medium_Cd != "FTP") InterceptionApplication.Messages.AddInformation(EventCode.C50620_VALID_APPLICATION);

                return true;
            }
            else
            {
                switch (InterceptionApplication.AppLiSt_Cd)
                {
                    case ApplicationState.INVALID_VARIATION_SOURCE_91:
                        if (InterceptionApplication.Medium_Cd != "FTP") InterceptionApplication.Messages.AddError(EventCode.C55002_INVALID_FINANCIAL_TERMS);
                        break;

                    case ApplicationState.INVALID_VARIATION_FINTERMS_92:
                        if (InterceptionApplication.Medium_Cd != "FTP") InterceptionApplication.Messages.AddError(EventCode.C55001_INVALID_SOURCE_HOLDBACK);
                        break;

                    default:
                        if (InterceptionApplication.Medium_Cd != "FTP") InterceptionApplication.Messages.AddError(EventCode.C55000_INVALID_VARIATION);
                        break;
                }

                await EventManager.SaveEventsAsync();

                return false;
            }

        }

        public async Task<List<SummonsSummaryData>> GetFixedAmountRecalcDateRecords()
        {
            return await DBfinance.SummonsSummaryRepository.GetFixedAmountRecalcDateRecordsAsync();
        }

        public async Task FullyServiceApplicationAsync()
        {
            var applicationManagerCopy = new InterceptionManager(DB, DBfinance, Config)
            {
                CurrentUser = this.CurrentUser
            };

            if (!await applicationManagerCopy.LoadApplicationAsync(Appl_EnfSrv_Cd, Appl_CtrlCd, loadFinancials: false))
            {
                string key = ApplKey.MakeKey(Appl_EnfSrv_Cd, Appl_CtrlCd);
                InterceptionApplication.Messages.AddError($"Application {key} does not exists");
                return;
            }

            await SetNewStateTo(ApplicationState.FULLY_SERVICED_13);

            await UpdateApplicationNoValidationAsync();

            await EventManager.SaveEventsAsync();
        }

        public async Task<bool> CancelApplication()
        {
            if (!IsValidCategory("I01"))
                return false;

            string appl_CommSubm_Text = InterceptionApplication.Appl_CommSubm_Text;
            var newIntFinH = InterceptionApplication.IntFinH;
            var newHldbCnd = InterceptionApplication.HldbCnd;

            // var newAppl_Source_RfrNr = InterceptionApplication.Appl_Source_RfrNr;
            var newAppl_Dbtr_Addr_Ln = InterceptionApplication.Appl_Dbtr_Addr_Ln;
            var newAppl_Dbtr_Addr_Ln1 = InterceptionApplication.Appl_Dbtr_Addr_Ln1;
            var newAppl_Dbtr_Addr_CityNme = InterceptionApplication.Appl_Dbtr_Addr_CityNme;
            var newAppl_Dbtr_Addr_PrvCd = InterceptionApplication.Appl_Dbtr_Addr_PrvCd;
            var newAppl_Dbtr_Addr_CtryCd = InterceptionApplication.Appl_Dbtr_Addr_CtryCd;
            var newAppl_Dbtr_Addr_PCd = InterceptionApplication.Appl_Dbtr_Addr_PCd;

            if (!await LoadApplicationAsync(Appl_EnfSrv_Cd, Appl_CtrlCd, loadFinancials: false))
            {
                InterceptionApplication.Messages.AddError($"No application was found in the database for {Appl_EnfSrv_Cd}-{Appl_CtrlCd}");
                return false;
            }

            InterceptionApplication.Appl_LastUpdate_Usr = DB.UpdateSubmitter;
            InterceptionApplication.Appl_LastUpdate_Dte = DateTime.Now;

            InterceptionApplication.Appl_CommSubm_Text = appl_CommSubm_Text ?? InterceptionApplication.Appl_CommSubm_Text;
            InterceptionApplication.IntFinH = newIntFinH;
            InterceptionApplication.HldbCnd = newHldbCnd;

            //InterceptionApplication.Appl_Source_RfrNr = newAppl_Source_RfrNr;
            InterceptionApplication.Appl_Dbtr_Addr_Ln = newAppl_Dbtr_Addr_Ln;
            InterceptionApplication.Appl_Dbtr_Addr_Ln1 = newAppl_Dbtr_Addr_Ln1;
            InterceptionApplication.Appl_Dbtr_Addr_CityNme = newAppl_Dbtr_Addr_CityNme;
            InterceptionApplication.Appl_Dbtr_Addr_PrvCd = newAppl_Dbtr_Addr_PrvCd;
            InterceptionApplication.Appl_Dbtr_Addr_CtryCd = newAppl_Dbtr_Addr_CtryCd;
            InterceptionApplication.Appl_Dbtr_Addr_PCd = newAppl_Dbtr_Addr_PCd;

            if (InterceptionApplication.ActvSt_Cd != "A")
            {
                EventManager.AddEvent(EventCode.C50841_CAN_ONLY_CANCEL_AN_ACTIVE_APPLICATION, activeState: "C");
                await EventManager.SaveEventsAsync();
                return false;
            }

            await SetNewStateTo(ApplicationState.MANUALLY_TERMINATED_14);

            await UpdateApplicationNoValidationAsync();

            await EventManager.SaveEventsAsync();

            if (InterceptionApplication.Medium_Cd != "FTP")
                InterceptionApplication.Messages.AddInformation(EventCode.C50620_VALID_APPLICATION);

            return true;
        }

        public async Task<bool> CompleteApplication()
        {
            if (!IsValidCategory("I01"))
                return false;

            if (!await LoadApplicationAsync(Appl_EnfSrv_Cd, Appl_CtrlCd, loadFinancials: false))
            {
                InterceptionApplication.Messages.AddError($"No application was found in the database for {Appl_EnfSrv_Cd}-{Appl_CtrlCd}");
                return false;
            }

            InterceptionApplication.Appl_LastUpdate_Usr = DB.UpdateSubmitter;
            InterceptionApplication.Appl_LastUpdate_Dte = DateTime.Now;

            await SetNewStateTo(ApplicationState.EXPIRED_15);

            await UpdateApplicationNoValidationAsync();

            await EventManager.SaveEventsAsync();

            if (InterceptionApplication.Medium_Cd != "FTP")
                InterceptionApplication.Messages.AddInformation(EventCode.C50620_VALID_APPLICATION);

            return true;
        }

        public async Task<bool> SuspendApplicationAsync()
        {
            if (!IsValidCategory("I01"))
                return false;

            if (!await LoadApplicationAsync(Appl_EnfSrv_Cd, Appl_CtrlCd, loadFinancials: false))
            {
                InterceptionApplication.Messages.AddError($"No application was found in the database for {Appl_EnfSrv_Cd}-{Appl_CtrlCd}");
                return false;
            }

            InterceptionApplication.Appl_LastUpdate_Usr = DB.CurrentSubmitter;
            InterceptionApplication.Appl_LastUpdate_Dte = DateTime.Now;

            if (InterceptionApplication.AppLiSt_Cd == ApplicationState.AWAITING_DOCUMENTS_FOR_VARIATION_19)
                await DeletePendingFinancialInformationAsync();

            await SetNewStateTo(ApplicationState.APPLICATION_SUSPENDED_35);

            await UpdateApplicationNoValidationAsync();

            await IncrementGarnSmryAsync();

            await DB.InterceptionTable.EISOHistoryDeleteBySINAsync(InterceptionApplication.Appl_Dbtr_Cnfrmd_SIN, false);

            await EventManager.SaveEventsAsync();

            if (InterceptionApplication.Medium_Cd != "FTP") InterceptionApplication.Messages.AddInformation(EventCode.C50620_VALID_APPLICATION);

            return true;
        }

        private async Task DeletePendingFinancialInformationAsync()
        {
            var pendingIntFinHs = await DB.InterceptionTable.GetAllInterceptionFinancialTermsAsync(Appl_EnfSrv_Cd, Appl_CtrlCd);
            var pendingHoldbacks = await DB.InterceptionTable.GetAllHoldbackConditionsAsync(Appl_EnfSrv_Cd, Appl_CtrlCd);

            foreach (var pendingIntFinH in pendingIntFinHs)
                if (pendingIntFinH.ActvSt_Cd == "P")
                    await DB.InterceptionTable.DeleteInterceptionFinancialTermsAsync(pendingIntFinH);

            foreach (var pendingHoldback in pendingHoldbacks)
                if (pendingHoldback.ActvSt_Cd == "P")
                    await DB.InterceptionTable.DeleteHoldbackConditionAsync(pendingHoldback);
        }

        public async Task<bool> AcceptInterceptionAsync(DateTime supportingDocsDate)
        {
            // only keep changes that are allowed:
            //   Source Reference Number, comment and address fields

            string appl_Source_RfrNr = InterceptionApplication.Appl_Source_RfrNr;
            string appl_CommSubm_Text = InterceptionApplication.Appl_CommSubm_Text;
            string appl_Dbtr_Addr_Ln = InterceptionApplication.Appl_Dbtr_Addr_Ln;
            string appl_Dbtr_Addr_Ln1 = InterceptionApplication.Appl_Dbtr_Addr_Ln1;
            string appl_Dbtr_Addr_CityNme = InterceptionApplication.Appl_Dbtr_Addr_CityNme;
            string appl_Dbtr_Addr_PrvCd = InterceptionApplication.Appl_Dbtr_Addr_PrvCd;
            string appl_Dbtr_Addr_CtryCd = InterceptionApplication.Appl_Dbtr_Addr_CtryCd;
            string appl_Dbtr_Addr_PCd = InterceptionApplication.Appl_Dbtr_Addr_PCd;

            await LoadApplicationAsync(Appl_EnfSrv_Cd, Appl_CtrlCd, loadFinancials: false);

            InterceptionApplication.Appl_Source_RfrNr = appl_Source_RfrNr ?? InterceptionApplication.Appl_Source_RfrNr;
            InterceptionApplication.Appl_CommSubm_Text = appl_CommSubm_Text ?? InterceptionApplication.Appl_CommSubm_Text;
            InterceptionApplication.Appl_Dbtr_Addr_Ln = appl_Dbtr_Addr_Ln ?? InterceptionApplication.Appl_Dbtr_Addr_Ln;
            InterceptionApplication.Appl_Dbtr_Addr_Ln1 = appl_Dbtr_Addr_Ln1 ?? InterceptionApplication.Appl_Dbtr_Addr_Ln1;
            InterceptionApplication.Appl_Dbtr_Addr_CityNme = appl_Dbtr_Addr_CityNme ?? InterceptionApplication.Appl_Dbtr_Addr_CityNme;
            InterceptionApplication.Appl_Dbtr_Addr_PrvCd = appl_Dbtr_Addr_PrvCd ?? InterceptionApplication.Appl_Dbtr_Addr_PrvCd;
            InterceptionApplication.Appl_Dbtr_Addr_CtryCd = appl_Dbtr_Addr_CtryCd ?? InterceptionApplication.Appl_Dbtr_Addr_CtryCd;
            InterceptionApplication.Appl_Dbtr_Addr_PCd = appl_Dbtr_Addr_PCd ?? InterceptionApplication.Appl_Dbtr_Addr_PCd;

            if (!IsValidCategory("I01"))
                return false;

            var interceptionDB = DB.InterceptionTable;
            InterceptionApplication.IntFinH = await interceptionDB.GetInterceptionFinancialTermsAsync(Appl_EnfSrv_Cd, Appl_CtrlCd, "P");
            if (InterceptionApplication.IntFinH is not null)
            {
                InterceptionApplication.IntFinH.IntFinH_Affdvt_SubmCd = DB.CurrentSubmitter;
                InterceptionApplication.IntFinH.IntFinH_RcvtAffdvt_Dte = supportingDocsDate;
                InterceptionApplication.HldbCnd = await interceptionDB.GetHoldbackConditionsAsync(Appl_EnfSrv_Cd, Appl_CtrlCd,
                                                                           InterceptionApplication.IntFinH.IntFinH_Dte, "P");
            }

            bool result = await AcceptGarnisheeAsync(supportingDocsDate, isAutoAccept: false);

            if (Config.ESDsites.Contains(Appl_EnfSrv_Cd.Trim()) && (InterceptionApplication.Medium_Cd == "FTP"))
                await DB.InterceptionTable.UpdateESDrequiredAsync(Appl_EnfSrv_Cd, Appl_CtrlCd, supportingDocsDate);

            await EventManager.SaveEventsAsync();

            return result;
        }

        private async Task<bool> AcceptGarnisheeAsync(DateTime supportingDocsDate, bool isAutoAccept = false)
        {
            AcceptedWithin30Days = true;

            if (!isAutoAccept)
            {
                var interceptionDB = DB.InterceptionTable;

                bool isESDsite = IsESD_MEP(Appl_EnfSrv_Cd);

                if (!GarnisheeSummonsReceiptDate.HasValue)
                    GarnisheeSummonsReceiptDate = supportingDocsDate;
                else
                    GarnisheeSummonsReceiptDate = await interceptionDB.GetGarnisheeSummonsReceiptDateAsync(Appl_EnfSrv_Cd, Appl_CtrlCd, isESDsite);

                var dateDiff = GarnisheeSummonsReceiptDate - InterceptionApplication.Appl_Lgl_Dte;
                if (dateDiff.HasValue && dateDiff.Value.Days > 30)
                {
                    AcceptedWithin30Days = false;
                    await RejectInterceptionAsync();

                    return false;
                }
            }

            InterceptionApplication.Appl_LastUpdate_Usr = DB.CurrentSubmitter;
            InterceptionApplication.Appl_LastUpdate_Dte = DateTime.Now;

            InterceptionApplication.Subm_Affdvt_SubmCd = DB.CurrentSubmitter;
            InterceptionApplication.Appl_RecvAffdvt_Dte = supportingDocsDate;

            await SetNewStateTo(ApplicationState.VALID_AFFIDAVIT_NOT_RECEIVED_7);

            await UpdateApplicationNoValidationAsync();

            await EventManager.SaveEventsAsync();

            if (InterceptionApplication.Medium_Cd != "FTP")
                InterceptionApplication.Messages.AddInformation(EventCode.C50620_VALID_APPLICATION);

            return true;

        }

        public async Task<List<InterceptionApplicationData>> GetApplicationsForVariationAutoAcceptAsync(string enfService)
        {
            var applications = await DB.ApplicationTable.GetApplicationsForAutomationAsync(enfService, medium_Cd: null,
                                                                        ApplicationState.AWAITING_DOCUMENTS_FOR_VARIATION_19,
                                                                        "I01", "A");

            var interceptions = new List<InterceptionApplicationData>();
            foreach (var application in applications)
                interceptions.Add(new InterceptionApplicationData(application));

            return interceptions;
        }

        public async Task<bool> IsRefNumberBlockedAsync()
        {
            return await DB.InterceptionTable.IsRefNumberBlockedAsync(InterceptionApplication.Appl_Source_RfrNr);
        }

        public async Task<bool> IsSinBlockedAsync()
        {
            return await DB.InterceptionTable.IsSinBlockedAsync(InterceptionApplication.Appl_Dbtr_Entrd_SIN);
        }

        public async Task  FTBatchNotification_CheckFTTransactionsAdded()
        {
            await DB.InterceptionTable.FTBatchNotification_CheckFTTransactionsAddedAsync();
        }

        public async Task MessageBrokerCRAReconciliation()
        {
            await DB.InterceptionTable.MessageBrokerCRAReconciliationAsync();
        }

        public async Task AutoAcceptVariationsAsync(string enfService)
        {
            string processName = $"Process Auto Accept Variation {enfService}";
            await DB.ProductionAuditTable.InsertAsync(processName, "Auto accept variation", "O");

            DB.CurrentSubmitter = "FO2SSS";

            var applAutomation = await GetApplicationsForVariationAutoAcceptAsync(enfService);

            foreach (var appl in applAutomation)
            {
                var thisManager = new InterceptionManager(appl, DB, DBfinance, Config);
                await thisManager.AcceptVariationAsync();
            }

            await DB.ProductionAuditTable.InsertAsync(processName, "Ended", "O");
        }

        public async Task<bool> AcceptVariationAsync()
        {
            if (!IsValidCategory("I01"))
                return false;

            string newComments = InterceptionApplication.Appl_CommSubm_Text?.Trim();

            if (!await LoadApplicationAsync(Appl_EnfSrv_Cd, Appl_CtrlCd, loadFinancials: false))
            {
                InterceptionApplication.Messages.AddError($"No application was found in the database for {Appl_EnfSrv_Cd}-{Appl_CtrlCd}");
                return false;
            }

            if (!string.IsNullOrEmpty(newComments))
                InterceptionApplication.Appl_CommSubm_Text = newComments;

            if (InterceptionApplication.AppLiSt_Cd != ApplicationState.AWAITING_DOCUMENTS_FOR_VARIATION_19)
            {
                await EventManager.SaveEventsAsync();

                return false;
            }

            if (InterceptionApplication.Medium_Cd == "FTP")
                InterceptionApplication.Appl_LastUpdate_Usr = "FO2SSS";
            else
                InterceptionApplication.Appl_LastUpdate_Usr = DB.CurrentSubmitter;
            InterceptionApplication.Appl_LastUpdate_Dte = DateTime.Now;

            EventManager.AddEvent(EventCode.C51111_VARIATION_ACCEPTED);

            // refresh the amount owed values in SummSmry
            var amountOwedProcess = new AmountOwedProcess(DB, DBfinance);
            var (summSmryNewData, _) = await amountOwedProcess.CalculateAndUpdateAmountOwedForVariationAsync(Appl_EnfSrv_Cd, Appl_CtrlCd);

            await SetNewStateTo(ApplicationState.PARTIALLY_SERVICED_12);

            await ChangeStateForFinancialTermsAsync(oldState: "A", newState: "I", 12);
            await ChangeStateForFinancialTermsAsync(oldState: "P", newState: "A", 12);

            var activeFinTerms = await DB.InterceptionTable.GetInterceptionFinancialTermsAsync(Appl_EnfSrv_Cd, Appl_CtrlCd, "A");

            VariationAction = VariationDocumentAction.AcceptVariationDocument;

            // update application

            decimal preBalance = summSmryNewData.PreBalance;

            await DB.InterceptionTable.InsertBalanceSnapshotAsync(Appl_EnfSrv_Cd, Appl_CtrlCd, preBalance,
                                                                      BalanceSnapshotChangeType.VARIATION_ACCEPTED,
                                                                      intFinH_Date: activeFinTerms.IntFinH_Dte);
            await UpdateApplicationNoValidationAsync();

            await DBfinance.SummonsSummaryRepository.UpdateSummonsSummaryAsync(summSmryNewData);

            if (!string.IsNullOrEmpty(activeFinTerms.IntFinH_DefHldbAmn_Period))
            {
                var fixedAmountData = DBfinance.SummonsSummaryFixedAmountRepository.GetSummonsSummaryFixedAmountAsync(Appl_EnfSrv_Cd, Appl_CtrlCd);
                if (fixedAmountData is null)
                {
                    var newFixedAmountRecalcDateTime = await RecalculateFixedAmountRecalcDateAfterVariationAsync(activeFinTerms, DateTime.Now);
                    await DBfinance.SummonsSummaryFixedAmountRepository.CreateSummonsSummaryFixedAmountAsync(Appl_EnfSrv_Cd, Appl_CtrlCd,
                                                                                                            newFixedAmountRecalcDateTime);
                }
            }
            else
                await DBfinance.SummonsSummaryFixedAmountRepository.DeleteSummSmryFixedAmountRecalcDateAsync(Appl_EnfSrv_Cd, Appl_CtrlCd);

            await EventManager.SaveEventsAsync();

            if (InterceptionApplication.Medium_Cd != "FTP") InterceptionApplication.Messages.AddInformation(EventCode.C50620_VALID_APPLICATION);

            return true;
        }

        public async Task RejectInterceptionAsync()
        {
            InterceptionApplication.Appl_LastUpdate_Usr = DB.CurrentSubmitter;
            InterceptionApplication.Appl_LastUpdate_Dte = DateTime.Now;

            await SetNewStateTo(ApplicationState.APPLICATION_REJECTED_9);

            await UpdateApplicationNoValidationAsync();

            await IncrementGarnSmryAsync();

            await EventManager.SaveEventsAsync();

            if (InterceptionApplication.Medium_Cd != "FTP") InterceptionApplication.Messages.AddInformation(EventCode.C50620_VALID_APPLICATION);
        }

        public async Task<bool> RejectVariationAsync(string applicationRejectReasons)
        {
            if (!IsValidCategory("I01"))
                return false;

            string newComments = InterceptionApplication.Appl_CommSubm_Text?.Trim();

            if (!await LoadApplicationAsync(Appl_EnfSrv_Cd, Appl_CtrlCd, loadFinancials: false))
            {
                InterceptionApplication.Messages.AddError($"No application was found in the database for {Appl_EnfSrv_Cd}-{Appl_CtrlCd}");
                return false;
            }

            if (!string.IsNullOrEmpty(newComments))
                InterceptionApplication.Appl_CommSubm_Text = newComments;

            if (InterceptionApplication.AppLiSt_Cd != ApplicationState.AWAITING_DOCUMENTS_FOR_VARIATION_19)
            {
                await EventManager.SaveEventsAsync();

                return false;
            }

            InterceptionApplication.Appl_LastUpdate_Usr = DB.CurrentSubmitter;
            InterceptionApplication.Appl_LastUpdate_Dte = DateTime.Now;

            if (applicationRejectReasons.Length > 80)
                applicationRejectReasons = applicationRejectReasons?[0..80];

            EventManager.AddEvent(EventCode.C51110_VARIATION_REJECTED, applicationRejectReasons);

            if (newComments == AUTO_REJECT_EXPIRED_TIME_FOR_VARIATION)
                EventManager.AddEvent(EventCode.C50762_VARIATION_REJECTED_BY_FOAEA_AS_VARIATION_DOCUMENT_NOT_RECEIVED_WITHIN_15_DAYS);

            await DeletePendingFinancialTermsAsync();

            VariationAction = VariationDocumentAction.RejectVariationDocument;
            await SetNewStateTo(ApplicationState.PARTIALLY_SERVICED_12);

            await UpdateApplicationNoValidationAsync();

            await EventManager.SaveEventsAsync();

            if (InterceptionApplication.Medium_Cd != "FTP") InterceptionApplication.Messages.AddInformation(EventCode.C50620_VALID_APPLICATION);

            return true;
        }

        public async Task<List<ProcessEISOOUTHistoryData>> GetEISOvalidApplications()
        {
            return await DB.InterceptionTable.GetEISOvalidApplications();
        }

        public async Task<List<EIoutgoingFederalData>> GetEIoutgoingData(string enfSrv)
        {
            return await DB.InterceptionTable.GetEIoutgoingData(enfSrv);
        }

        public override async Task ProcessBringForwardsAsync(ApplicationEventData bfEvent)
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

                await EventManager.SaveEventAsync(bfEvent);
            }
            else
            {
                if (bfEvent.Event_Reas_Cd.HasValue)
                {
                    var dbNotification = DB.NotificationService;
                    switch (bfEvent.Event_Reas_Cd)
                    {
                        case EventCode.C50528_BF_10_DAYS_FROM_RECEIPT_OF_APPLICATION:
                            // do nothing for I01 -- this is now handled by the ESD event processing that every morning
                            break;

                        case EventCode.C50896_AWAITING_DOCUMENTS_FOR_VARIATION:
                            if (InterceptionApplication.AppLiSt_Cd == ApplicationState.AWAITING_DOCUMENTS_FOR_VARIATION_19)
                            {
                                DateTime lastVariationDate = await GetLastVariationDateAsync();
                                int elapsed = Math.Abs((DateTime.Now - lastVariationDate).Days);

                                if (elapsed >= 15)
                                    await RejectVariationAsync(AUTO_REJECT_EXPIRED_TIME_FOR_VARIATION);
                                else
                                {
                                    DateTime dateForNextBF = DateTime.Now.AddDays(5);
                                    EventManager.AddEvent(EventCode.C50896_AWAITING_DOCUMENTS_FOR_VARIATION);
                                    EventManager.AddBFEvent(EventCode.C50896_AWAITING_DOCUMENTS_FOR_VARIATION,
                                                            effectiveTimestamp: dateForNextBF);
                                }
                            }
                            break;

                        case EventCode.C54005_CREATE_A_DEBTOR_LETTER_EVENT_IN_EVNTDBTR:
                            if (InterceptionApplication.Appl_Rcptfrm_Dte.AddDays(20) < DateTime.Now)
                            {
                                await dbNotification.SendEmailAsync("Debtor Letter create date exceeds 20 day range",
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

                await EventManager.SaveEventAsync(bfEvent);
            }

            await EventManager.SaveEventsAsync();
        }
    }
}
