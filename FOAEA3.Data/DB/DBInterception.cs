using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBInterception : DBbase, IInterceptionRepository
    {

        public DBInterception(IDBToolsAsync mainDB) : base(mainDB)
        {

        }

        public async Task<List<InterceptionApplicationData>> FindMatchingActiveApplicationsAsync(string appl_EnfSrv_Cd, string appl_CtrlCd,
                                                                        string confirmedSIN, string creditorFirstName,
                                                                        string creditorSurname)
        {

            var parameters = new Dictionary<string, object>
                {
                    {"Appl_EnfSrv_Cd", appl_EnfSrv_Cd},
                    {"Appl_CtrlCd", appl_CtrlCd },
                    {"Appl_Dbtr_Cnfrmd_SIN", confirmedSIN },
                    {"Appl_Crdtr_FrstNme", creditorFirstName },
                    {"Appl_Crdtr_SurNme", creditorSurname }
                };

            var data = await MainDB.GetDataFromStoredProcAsync<InterceptionApplicationData>("GetMatchingActiveApplications", parameters, DBApplication.FillApplicationDataFromReader);

            return data;
        }

        public async Task<bool> IsFeeCumulativeForApplicationAsync(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"EnfSrvCd", appl_EnfSrv_Cd},
                    {"CtrlCd", appl_CtrlCd }
                };

            int result = await MainDB.GetDataFromStoredProcAsync<int>("IsFeeCumulativeForApplication", parameters);

            return (result == 1);
        }

        public async Task<bool> IsVariationIncreaseAsync(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"appl_enfsrvcd", appl_EnfSrv_Cd},
                    {"appl_ctrlcd", appl_CtrlCd }
                };

            int result = await MainDB.GetDataFromStoredProcAsync<int>("ApplVerifyVariationIncrease", parameters);

            return (result == 1);
        }

        public async Task<InterceptionFinancialHoldbackData> GetInterceptionFinancialTermsAsync(string appl_EnfSrv_Cd, string appl_CtrlCd,
                                                                               string activeState = "A")
        {
            var parameters = new Dictionary<string, object>
                {
                    {"Appl_Enfsrv_Cd", appl_EnfSrv_Cd},
                    {"Appl_CtrlCd", appl_CtrlCd },
                    {"ActvSt_Cd", activeState }
                };

            var data = await MainDB.GetDataFromStoredProcAsync<InterceptionFinancialHoldbackData>("GetIntFinHtoActivateDeactivate", parameters, FillIntFinHDataFromReader);

            return data.SingleOrDefault();
        }

        public async Task<List<InterceptionFinancialHoldbackData>> GetAllInterceptionFinancialTermsAsync(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"Appl_Enfsrv_Cd", appl_EnfSrv_Cd},
                    {"Appl_CtrlCd", appl_CtrlCd }
                };

            var data = await MainDB.GetDataFromStoredProcAsync<InterceptionFinancialHoldbackData>("DefaultHoldbackGetHoldback", parameters, FillIntFinHDataFromReader);

            return data;
        }

        public async Task CreateInterceptionFinancialTermsAsync(InterceptionFinancialHoldbackData intFinH)
        {
            var parameters = SetIntFinHParameters(intFinH);

            _ = await MainDB.ExecProcAsync("IntFinH_Insert", parameters);
        }

        public async Task UpdateInterceptionFinancialTermsAsync(InterceptionFinancialHoldbackData intFinH)
        {
            var parameters = SetIntFinHParameters(intFinH);

            _ = await MainDB.ExecProcAsync("IntFinH_Update", parameters);
        }

        public async Task DeleteInterceptionFinancialTermsAsync(InterceptionFinancialHoldbackData intFinH)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Appl_EnfSrv_Cd", intFinH.Appl_EnfSrv_Cd},
                {"Appl_CtrlCd", intFinH.Appl_CtrlCd},
                {"IntFinH_Dte", intFinH.IntFinH_Dte}
            };

            _ = await MainDB.ExecProcAsync("IntFinH_Delete", parameters);
        }

        private static Dictionary<string, object> SetIntFinHParameters(InterceptionFinancialHoldbackData intFinH)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Appl_EnfSrv_Cd", intFinH.Appl_EnfSrv_Cd},
                {"Appl_CtrlCd", intFinH.Appl_CtrlCd},
                {"IntFinH_Dte", intFinH.IntFinH_Dte},
                {"HldbCtg_Cd", intFinH.HldbCtg_Cd},
                {"IntFinH_LmpSum_Money", intFinH.IntFinH_LmpSum_Money},
                {"IntFinH_TtlAmn_Money", intFinH.IntFinH_TtlAmn_Money},
                {"IntFinH_LiStCd", intFinH.IntFinH_LiStCd},
                {"ActvSt_Cd", intFinH.ActvSt_Cd}
            };

            if (intFinH.IntFinH_RcvtAffdvt_Dte.HasValue)
                parameters.Add("IntFinH_RcvtAffdvt_Dte", intFinH.IntFinH_RcvtAffdvt_Dte.Value);

            if (!string.IsNullOrEmpty(intFinH.IntFinH_Affdvt_SubmCd))
                parameters.Add("IntFinH_Affdvt_SubmCd", intFinH.IntFinH_Affdvt_SubmCd);

            if (!string.IsNullOrEmpty(intFinH.PymPr_Cd))
                parameters.Add("PymPr_Cd", intFinH.PymPr_Cd);

            if (!string.IsNullOrEmpty(intFinH.HldbTyp_Cd))
                parameters.Add("HldbTyp_Cd", intFinH.HldbTyp_Cd);

            if (intFinH.IntFinH_DefHldbAmn_Money.HasValue)
                parameters.Add("IntFinH_DefHldbAmn_Money", intFinH.IntFinH_DefHldbAmn_Money.Value);

            if (intFinH.IntFinH_DefHldbPrcnt.HasValue)
                parameters.Add("IntFinH_DefHldbPrcnt", intFinH.IntFinH_DefHldbPrcnt.Value);

            if (intFinH.IntFinH_CmlPrPym_Ind.HasValue)
                parameters.Add("IntFinH_CmlPrPym_Ind", intFinH.IntFinH_CmlPrPym_Ind.Value);

            if (intFinH.IntFinH_MxmTtl_Money.HasValue)
                parameters.Add("IntFinH_MxmTtl_Money", intFinH.IntFinH_MxmTtl_Money.Value);

            if (intFinH.IntFinH_PerPym_Money.HasValue)
                parameters.Add("IntFinH_PerPym_Money", intFinH.IntFinH_PerPym_Money.Value);

            if (intFinH.IntFinH_VarIss_Dte.HasValue)
                parameters.Add("IntFinH_VarIss_Dte", intFinH.IntFinH_VarIss_Dte.Value);

            if (!string.IsNullOrEmpty(intFinH.IntFinH_CreateUsr))
                parameters.Add("IntFinH_CreateUsr", intFinH.IntFinH_CreateUsr);

            if (!string.IsNullOrEmpty(intFinH.IntFinH_DefHldbAmn_Period))
                parameters.Add("IntFinH_DefHldbAmn_Period", intFinH.IntFinH_DefHldbAmn_Period);

            return parameters;
        }

        public async Task<List<HoldbackConditionData>> GetHoldbackConditionsAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, DateTime intFinH_Date,
                                                                 string activeState = "A")
        {
            var parameters = new Dictionary<string, object>
                {
                    {"Appl_Enfsrv_Cd", appl_EnfSrv_Cd},
                    {"Appl_CtrlCd", appl_CtrlCd },
                    {"IntFinH_Dte", intFinH_Date }
                };

            var data = await MainDB.GetDataFromStoredProcAsync<HoldbackConditionData>("GetHldbCndToActivateDeactivate", parameters, FillHldbCndDataFromReader);

            return data.Where(m => m.ActvSt_Cd == activeState).ToList();
        }

        public async Task<List<HoldbackConditionData>> GetAllHoldbackConditionsAsync(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"Appl_Enfsrv_Cd", appl_EnfSrv_Cd},
                    {"Appl_CtrlCd", appl_CtrlCd }
                };

            var data = await MainDB.GetDataFromStoredProcAsync<HoldbackConditionData>("GetHldbCndForAppl", parameters, FillHldbCndDataFromReader);
            return data;
        }

        public async Task CreateHoldbackConditionsAsync(List<HoldbackConditionData> holdbackConditions)
        {
            foreach (var holdbackCondition in holdbackConditions)
                await CreateHoldbackConditionAsync(holdbackCondition);
        }

        private async Task CreateHoldbackConditionAsync(HoldbackConditionData holdbackCondition)
        {
            var parameters = SetHoldbackConditionParameters(holdbackCondition);

            _ = await MainDB.ExecProcAsync("HldbCnd_Insert", parameters);
        }

        public async Task UpdateHoldbackConditionsAsync(List<HoldbackConditionData> holdbackConditions)
        {
            foreach (var holdbackCondition in holdbackConditions)
                await UpdateHoldbackConditionAsync(holdbackCondition);
        }

        private async Task UpdateHoldbackConditionAsync(HoldbackConditionData holdbackCondition)
        {
            var parameters = SetHoldbackConditionParameters(holdbackCondition);

            _ = await MainDB.ExecProcAsync("HldbCnd_Update", parameters);
        }

        public async Task DeleteHoldbackConditionsAsync(List<HoldbackConditionData> holdbackConditions)
        {
            foreach (var holdbackCondition in holdbackConditions)
                await DeleteHoldbackConditionAsync(holdbackCondition);
        }

        public async Task DeleteHoldbackConditionAsync(HoldbackConditionData holdbackCondition)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Appl_EnfSrv_Cd", holdbackCondition.Appl_EnfSrv_Cd},
                {"Appl_CtrlCd", holdbackCondition.Appl_CtrlCd},
                {"IntFinH_Dte", holdbackCondition.IntFinH_Dte},
                {"EnfSrv_Cd", holdbackCondition.EnfSrv_Cd}
            };

            _ = await MainDB.ExecProcAsync("HldbCnd_Delete", parameters);
        }

        private static Dictionary<string, object> SetHoldbackConditionParameters(HoldbackConditionData holdbackCondition)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Appl_EnfSrv_Cd", holdbackCondition.Appl_EnfSrv_Cd},
                {"Appl_CtrlCd", holdbackCondition.Appl_CtrlCd},
                {"IntFinH_Dte", holdbackCondition.IntFinH_Dte},
                {"EnfSrv_Cd", holdbackCondition.EnfSrv_Cd},
                {"HldbCtg_Cd", holdbackCondition.HldbCtg_Cd},
                {"HldbCnd_LiStCd", holdbackCondition.HldbCnd_LiStCd},
                {"ActvSt_Cd", holdbackCondition.ActvSt_Cd}
            };

            if (holdbackCondition.HldbCnd_MxmPerChq_Money.HasValue)
                parameters.Add("HldbCnd_MxmPerChq_Money", holdbackCondition.HldbCnd_MxmPerChq_Money.Value);

            if (holdbackCondition.HldbCnd_SrcHldbAmn_Money.HasValue)
                parameters.Add("HldbCnd_SrcHldbAmn_Money", holdbackCondition.HldbCnd_SrcHldbAmn_Money.Value);

            if (holdbackCondition.HldbCnd_SrcHldbPrcnt.HasValue)
                parameters.Add("HldbCnd_SrcHldbPrcnt", holdbackCondition.HldbCnd_SrcHldbPrcnt.Value);

            return parameters;
        }

        public async Task<List<InterceptionApplicationData>> GetSameCreditorForI01Async(string appl_CtrlCd, string submCd, string enteredSIN,
                                                                       byte confirmedSIN, string activeState)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Appl_CtrlCd", appl_CtrlCd},
                {"Subm_SubmCd", submCd},
                {"Appl_Dbtr_Entrd_SIN", enteredSIN ?? " "},
                {"Appl_SIN_Cnfrmd_Ind", confirmedSIN},
                {"ActvSt_Cd", activeState}
            };

            var data = await MainDB.GetDataFromStoredProcAsync<InterceptionApplicationData>("GetSameCreditorForI01", parameters, DBApplication.FillApplicationDataFromReader);

            return data;
        }

        public async Task<string> GetApplicationJusticeNumberAsync(string confirmedSIN, string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"Appl_Enfsrv_Cd", appl_EnfSrv_Cd},
                    {"Appl_CtrlCd", appl_CtrlCd },
                    {"Appl_Dbtr_Cnfrmd_SIN", confirmedSIN}
                };

            return await MainDB.GetDataFromProcSingleValueAsync<string>("GetApplJusticeNr", parameters);
        }

        public async Task<string> GetDebtorIDAsync(string first3Char)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"First3Char", first3Char}
                };

            return await MainDB.GetDataFromProcSingleValueAsync<string>("GetSummSmryDebtorID", parameters);
        }

        public async Task<bool> IsAlreadyUsedJusticeNumberAsync(string justiceNumber)
        {
            var parameters = new Dictionary<string, object>
            {
                {"JusticeNr", justiceNumber}
            };

            var data = await MainDB.GetDataFromStoredProcAsync<InterceptionApplicationData>("ApplGetApplByJusticeNR", parameters, DBApplication.FillApplicationDataFromReader);

            if (data.Count > 0)
            {
                if (data[0].Appl_JusticeNr.Trim().ToUpper() == justiceNumber.Trim().ToUpper())
                    return true;
            }

            return false;
        }

        public async Task<DateTime> GetGarnisheeSummonsReceiptDateAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, bool isESD)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"Appl_Enfsrv_Cd", appl_EnfSrv_Cd},
                    {"Appl_CtrlCd", appl_CtrlCd },
                    {"isESD", isESD}
                };

            return await MainDB.GetDataFromProcSingleValueAsync<DateTime>("GetGarnisheeSummonsReceiptDate", parameters);
        }

        public async Task<int> GetTotalActiveSummonsAsync(string appl_EnfSrv_Cd, string enfOfficeCode)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"Appl_EnfSrv_Cd", appl_EnfSrv_Cd},
                    {"EnfOff_Cd", enfOfficeCode }
                };

            return await MainDB.GetDataFromProcSingleValueAsync<int>("GetTotalActiveSummons", parameters);
        }

        public async Task<string> EISOHistoryDeleteBySINAsync(string confirmedSIN, bool removeSIN)
        {
            var parameters = new Dictionary<string, object>
            {
                {"SIN", confirmedSIN},
                {"removeSIN", removeSIN }
            };

            var returnParameters = new Dictionary<string, string>
            {
                {"message", "S120"}
            };

            var data = await MainDB.GetDataFromStoredProcViaReturnParametersAsync("MessageBrokerDeleteEISOOUTHistoryBySIN", parameters, returnParameters);

            return data["message"] as string;
        }

        public async Task<List<ProcessEISOOUTHistoryData>> GetEISOHistoryBySINAsync(string confirmedSIN)
        {
            var parameters = new Dictionary<string, object>
            {
                {"ConfirmedSIN", confirmedSIN}
            };

            var data = await MainDB.GetDataFromStoredProcAsync<ProcessEISOOUTHistoryData>("Prcs_EISOOUT_History_SelectForSIN", parameters, FillEISOOUTFromReader);

            return data;
        }

        private void FillEISOOUTFromReader(IDBHelperReader rdr, ProcessEISOOUTHistoryData data)
        {
            data.TRANS_TYPE_CD = rdr["TRANS_TYPE_CD"] as string;
            data.ACCT_NBR = rdr["ACCT_NBR"] as string;
            data.TRANS_AMT = rdr["TRANS_AMT"] as string;
            data.RQST_RFND_EID = rdr["RQST_RFND_EID"] as string;
            data.OUTPUT_DEST_CD = rdr["OUTPUT_DEST_CD"] as string;
            data.XREF_ACCT_NBR = rdr["XREF_ACCT_NBR"] as string;
            data.FOA_DELETE_IND = rdr["FOA_DELETE_IND"] as string;
            data.FOA_RECOUP_PRCNT = rdr["FOA_RECOUP_PRCNT"] as string;
            data.BLANK_AREA = rdr["BLANK_AREA"] as string;
            data.PAYMENT_RECEIVED = rdr["PAYMENT_RECEIVED"] as int?; // can be null 
        }

        public async Task<(bool, DateTime)> IsNewESDreceivedAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, ESDrequired originalESDrequired)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Appl_EnfSrv_Cd", appl_EnfSrv_Cd },
                { "Appl_CtrlCd", appl_CtrlCd },
                { "AmendedESDRequired", originalESDrequired }
            };
            var returnParameters = new Dictionary<string, string>
            {
                { "NewESDexists", "B" },
                { "ESDReceivedDate", "D" }
            };
            
            var returnValues = await MainDB.GetDataFromStoredProcViaReturnParametersAsync("IsNewESDReceived", parameters, returnParameters);

            return ((bool)returnValues["NewESDexists"], (DateTime)returnValues["ESDReceivedDate"]);
        }

        public async Task<List<ApplicationData>> GetApplicationsForReject()
        {
            return await MainDB.GetDataFromStoredProcAsync<ApplicationData>("GetI01ApplicationsForReject", DBApplication.FillApplicationDataFromReader);
        }

        public async Task<List<ApplicationData>> GetTerminatedI01()
        {
            return await MainDB.GetDataFromStoredProcAsync<ApplicationData>("GetTerminatedI01", DBApplication.FillApplicationDataFromReader);
        }

        public async Task<ApplicationData> GetAutoAcceptGarnisheeOverrideData(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Appl_EnfSrv_Cd", appl_EnfSrv_Cd },
                { "Appl_CtrlCd", appl_CtrlCd }
            };

            var data = await MainDB.GetDataFromStoredProcAsync<ApplicationData>("GetAutoAcceptGarnisheeOverrideData", parameters, DBApplication.FillApplicationDataFromReader);

            return data.FirstOrDefault();
        }

        public async Task<bool> IsSinBlockedAsync(string appl_Dbtr_Entrd_SIN)
        {
            var parameters = new Dictionary<string, object>
            {
                { "sin", appl_Dbtr_Entrd_SIN }
            };
            int count = await MainDB.GetDataFromStoredProcAsync<int>("ExGratiaIsSINBlocked", parameters);

            return count > 0;
        }
        public async Task<bool> IsRefNumberBlockedAsync(string appl_Source_RfrNr)
        {
            var parameters = new Dictionary<string, object>
            {
                { "refnr", appl_Source_RfrNr }
            };
            int count = await MainDB.GetDataFromStoredProcAsync<int>("ExGratiaIsRefNrBlocked", parameters);

            return count > 0;
        }

        public async Task<List<ElectronicSummonsDocumentRequiredData>> GetESDrequiredAsync()
        {
            return await MainDB.GetDataFromStoredProcAsync<ElectronicSummonsDocumentRequiredData>("GetESDRequiredData", FillESDrequiredData);
        }

        private void FillESDrequiredData(IDBHelperReader rdr, ElectronicSummonsDocumentRequiredData data)
        {
            data.Appl_EnfSrv_Cd = rdr["Appl_EnfSrv_Cd"] as string;
            data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string;
            data.ESDRequired = (ESDrequired)rdr["ESDRequired"];
            data.ESDRequiredDate = (DateTime)rdr["ESDRequiredDate"];
            if (rdr.ColumnExists("ESDReceivedDate"))
                data.ESDReceivedDate = (DateTime?)rdr["ESDReceivedDate"];
            if (rdr.ColumnExists("Subm_SubmCd"))
                data.Subm_SubmCd = rdr["Subm_SubmCd"] as string;
            if (rdr.ColumnExists("Subm_Recpt_SubmCd"))
                data.Subm_Recpt_SubmCd = rdr["Subm_Recpt_SubmCd"] as string;
            if (rdr.ColumnExists("AppLiSt_Cd"))
                data.AppLiSt_Cd = (ApplicationState)rdr["AppLiSt_Cd"];
            if (rdr.ColumnExists("ActvSt_Cd"))
                data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
        }

        public async Task InsertESDrequiredAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, ESDrequired originalESDrequired,
                                      DateTime? esdReceivedDate = null)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Appl_EnfSrv_Cd", appl_EnfSrv_Cd },
                {"Appl_CtrlCd", appl_CtrlCd },
                {"ESDRequired", originalESDrequired }
            };

            if (esdReceivedDate.HasValue)
                parameters.Add("ESDReceivedDate", esdReceivedDate.Value);

            await MainDB.ExecProcAsync("InsertESDRequired", parameters);

        }

        public async Task UpdateESDrequiredAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, DateTime? esdReceivedDate = null, bool resetUpdate = false)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Appl_EnfSrv_Cd", appl_EnfSrv_Cd },
                {"Appl_CtrlCd", appl_CtrlCd },
                {"bResetUpdate", resetUpdate }
            };

            if (esdReceivedDate.HasValue)
                parameters.Add("dESDReceivedDate", esdReceivedDate.Value);
            else
                parameters.Add("dESDReceivedDate", null);

            await MainDB.ExecProcAsync("UpdateESDRequired", parameters);

        }

        public async Task<ElectronicSummonsDocumentZipData> GetESDasync(string fileName)
        {
            var parameters = new Dictionary<string, object>
            {
                {"FileName", fileName }
            };

            var data = await MainDB.GetDataFromStoredProcAsync<ElectronicSummonsDocumentZipData>("ESDZIPs_FindByFileName", parameters, FillElectronicSummonsDocumentZipDataFromReader);

            return data.SingleOrDefault();
        }

        public async Task<ElectronicSummonsDocumentZipData> CreateESDasync(int processId, string fileName, DateTime dateReceived)
        {
            var parameters = new Dictionary<string, object>
            {
                {"PrcID", processId },
                {"ZipName", fileName },
                {"DateReceived", dateReceived }
            };

            await MainDB.ExecProcAsync("ESDZipsInsert", parameters);

            return new ElectronicSummonsDocumentZipData
            {
                ZipID = MainDB.LastReturnValue,
                PrcID = processId,
                ZipName = fileName,
                DateReceived = dateReceived
            };
        }

        public async Task<ElectronicSummonsDocumentPdfData> CreateESDPDFasync(ElectronicSummonsDocumentPdfData newPDFentry)
        {
            var parameters = new Dictionary<string, object>
            {
                {"ZipID", newPDFentry.ZipID },
                {"PDFName", newPDFentry.PDFName },
                {"EnfSrv", newPDFentry.EnfSrv[..2] },
                {"Ctrl", newPDFentry.Ctrl }
            };

            await MainDB.ExecProcAsync("ESDPDFsInsert", parameters);

            newPDFentry.PDFid = MainDB.LastReturnValue;

            return newPDFentry;
        }

        public async Task<List<ElectronicSummonsDocumentData>> FindDocumentsForApplicationAsync(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            var parameters = new Dictionary<string, object>
            {
                {"EnfSrv", appl_EnfSrv_Cd },
                {"Ctrl", appl_CtrlCd }
            };

            return await MainDB.GetDataFromStoredProcAsync<ElectronicSummonsDocumentData>("ESDFindDocuments", parameters, FillElectronicSummonsDocumentDataFromReader);
        }

        private void FillElectronicSummonsDocumentDataFromReader(IDBHelperReader rdr, ElectronicSummonsDocumentData data)
        {
            data.ZipName = rdr["ZipName"] as string;
            data.PdfName = rdr["PDFName"] as string;
        }

        private void FillElectronicSummonsDocumentZipDataFromReader(IDBHelperReader rdr, ElectronicSummonsDocumentZipData data)
        {
            data.ZipID = (int)rdr["ZipID"];
            data.PrcID = (int)rdr["PrcID"];
            data.ZipName = rdr["ZipName"] as string;
            data.DateReceived = (DateTime)rdr["DateReceived"];
        }

        public async Task InsertBalanceSnapshotAsync(string appl_EnfSrv_Cd, string appl_CtrlCd, decimal totalAmount,
                                          BalanceSnapshotChangeType changeType, int? summFAFR_id = null, DateTime? intFinH_Date = null)
        {
            var parameters = new Dictionary<string, object>
            {
                {"Appl_EnfSrv_Cd", appl_EnfSrv_Cd },
                {"Appl_CtrlCd", appl_CtrlCd },
                {"Appl_TotalAmnt", totalAmount},
                {"change_type", (int) changeType}
            };

            if (summFAFR_id.HasValue)
                parameters.Add("SummFAFR_Id", summFAFR_id.Value);

            if (intFinH_Date.HasValue)
                parameters.Add("IntFinH_Dte", intFinH_Date.Value);

            await MainDB.ExecProcAsync("InsertBalanceSnapshot", parameters);

        }

        public async Task<List<ExGratiaListData>> GetExGratiasAsync()
        {
            return await MainDB.GetAllDataAsync<ExGratiaListData>("ExGratiaList", FillExGratiaListFromReader);
        }

        public async Task<List<PaymentPeriodData>> GetPaymentPeriodsAsync()
        {
            return await MainDB.GetAllDataAsync<PaymentPeriodData>("PymPr", FillPaymentPeriodFromReader);
        }

        public async Task<List<HoldbackTypeData>> GetHoldbackTypesAsync()
        {
            return await MainDB.GetAllDataAsync<HoldbackTypeData>("HldbTyp", FillHolbackTypeFromReader);
        }

        private void FillIntFinHDataFromReader(IDBHelperReader rdr, InterceptionFinancialHoldbackData data)
        {
            data.Appl_EnfSrv_Cd = rdr["Appl_EnfSrv_Cd"] as string;
            data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string;
            data.IntFinH_Dte = (DateTime)rdr["IntFinH_Dte"];
            data.IntFinH_RcvtAffdvt_Dte = rdr["IntFinH_RcvtAffdvt_Dte"] as DateTime?; // can be null 
            data.IntFinH_Affdvt_SubmCd = rdr["IntFinH_Affdvt_SubmCd"] as string; // can be null 
            data.PymPr_Cd = rdr["PymPr_Cd"] as string; // can be null 
            if (rdr.ColumnExists("IntFinH_NextRecalcDate_Cd"))
                data.IntFinH_NextRecalcDate_Cd = rdr["IntFinH_NextRecalcDate_Cd"] as int?; // can be null 
            data.HldbTyp_Cd = rdr["HldbTyp_Cd"] as string; // can be null 
            data.IntFinH_DefHldbAmn_Money = rdr["IntFinH_DefHldbAmn_Money"] as decimal?; // can be null 
            data.IntFinH_DefHldbPrcnt = rdr["IntFinH_DefHldbPrcnt"] as int?; // can be null 
            data.HldbCtg_Cd = rdr["HldbCtg_Cd"] as string;
            data.IntFinH_CmlPrPym_Ind = rdr["IntFinH_CmlPrPym_Ind"] as byte?; // can be null 
            data.IntFinH_MxmTtl_Money = rdr["IntFinH_MxmTtl_Money"] as decimal?; // can be null 
            data.IntFinH_PerPym_Money = rdr["IntFinH_PerPym_Money"] as decimal?; // can be null 
            data.IntFinH_LmpSum_Money = (decimal)rdr["IntFinH_LmpSum_Money"];
            data.IntFinH_TtlAmn_Money = (decimal)rdr["IntFinH_TtlAmn_Money"];
            data.IntFinH_VarIss_Dte = rdr["IntFinH_VarIss_Dte"] as DateTime?; // can be null 
            data.IntFinH_CreateUsr = rdr["IntFinH_CreateUsr"] as string; // can be null 
            data.IntFinH_LiStCd = (short)rdr["IntFinH_LiStCd"];
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
            data.IntFinH_DefHldbAmn_Period = rdr["IntFinH_DefHldbAmn_Period"] as string; // can be null 
        }

        private void FillHldbCndDataFromReader(IDBHelperReader rdr, HoldbackConditionData data)
        {
            data.Appl_EnfSrv_Cd = rdr["Appl_EnfSrv_Cd"] as string;
            data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string;
            data.IntFinH_Dte = (DateTime)rdr["IntFinH_Dte"];
            data.EnfSrv_Cd = rdr["EnfSrv_Cd"] as string;
            data.HldbCnd_MxmPerChq_Money = rdr["HldbCnd_MxmPerChq_Money"] as decimal?; // can be null 
            data.HldbCnd_SrcHldbAmn_Money = rdr["HldbCnd_SrcHldbAmn_Money"] as decimal?; // can be null 
            data.HldbCnd_SrcHldbPrcnt = rdr["HldbCnd_SrcHldbPrcnt"] as int?; // can be null 
            data.HldbCtg_Cd = rdr["HldbCtg_Cd"] as string;
            data.HldbCnd_LiStCd = (short)rdr["HldbCnd_LiStCd"];
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
        }

        private void FillExGratiaListFromReader(IDBHelperReader rdr, ExGratiaListData data)
        {
            data.ID = (int)rdr["ID"];
            data.EnfSrv_Cd = rdr["EnfSrv_Cd"] as string;
            data.Source_Ref_Nr = rdr["Source_Ref_Nr"] as string;
            data.DateAdded = (DateTime)rdr["DateAdded"];
            data.Surname = rdr["Surname"] as string;
            data.SIN = rdr["SIN"] as string;
            data.BlockSIN = (bool)rdr["BlockSIN"];
            data.BlockREF = (bool)rdr["BlockREF"];
        }

        private void FillPaymentPeriodFromReader(IDBHelperReader rdr, PaymentPeriodData data)
        {
            data.PymPr_Cd = rdr["PymPr_Cd"] as string;
            data.PymPr_Txt_E = rdr["PymPr_Txt_E"] as string;
            data.PymPr_Txt_F = rdr["PymPr_Txt_F"] as string;
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
        }

        private void FillHolbackTypeFromReader(IDBHelperReader rdr, HoldbackTypeData data)
        {
            data.HldbTyp_Cd = rdr["HldbTyp_Cd"] as string;
            data.HldbTyp_Txt_E = rdr["HldbTyp_Txt_E"] as string; // can be null 
            data.HldbTyp_Txt_F = rdr["HldbTyp_Txt_F"] as string; // can be null 
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
        }

        public Task<bool> IsSinBlocked(string appl_Dbtr_Entrd_SIN)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRefNumberBlocked(string appl_Source_RfrNr)
        {
            throw new NotImplementedException();
        }
    }
}
