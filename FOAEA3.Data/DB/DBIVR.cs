using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model.Interfaces.Repository;
using FOAEA3.Model.IVR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    public class DBIVR : DBbase, IIVRRepository
    {

        public DBIVR(IDBToolsAsync mainDB) : base(mainDB)
        {

        }

        public async Task<CheckSinReturnData> CheckSinCount(CheckSinGetData data)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Debtor_SIN",  data.DebtorSin}
            };

            var result = await MainDB.GetDataFromStoredProcAsync<CheckSinReturnData>("fpIVR_Check_Sin", 
                                                                        parameters,
                                                                        FillDataFromReaderGetSinCount);
            return result.FirstOrDefault();
        }

        private void FillDataFromReaderGetSinCount(IDBHelperReader rdr, CheckSinReturnData data)
        {
            data.SinCount = (int)(rdr["SinCount"]);
        }

        public async Task<CheckCreditorIdReturnData> CheckCreditorId(CheckCreditorIdGetData data)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Creditor_Id",  data.CreditorId}
            };

            var result = await MainDB.GetDataFromStoredProcAsync<CheckCreditorIdReturnData>("fpIVR_Check_Creditor_Id",
                                                                        parameters,
                                                                        FillDataFromReaderCheckCreditorId);
            return result.FirstOrDefault();
        }

        private void FillDataFromReaderCheckCreditorId(IDBHelperReader rdr, CheckCreditorIdReturnData data)
        {
            data.CreditorCount = (int)(rdr["CREDITOR_COUNT"]);
        }

         public async Task<CheckControlCodeReturnData> CheckControlCode(CheckControlCodeGetData data)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Debtor_SIN",  data.DebtorSin},
                { "Ctrl_Cd",  data.ControlCode}
            };

            var result = await MainDB.GetDataFromStoredProcAsync<CheckControlCodeReturnData>("fpIVR_Check_Ctrl_Cd",
                                                                        parameters,
                                                                        FillDataFromReaderCheckControlCode);
            return result.FirstOrDefault();
        }

        private void FillDataFromReaderCheckControlCode(IDBHelperReader rdr, CheckControlCodeReturnData data)
        {
            data.ControlCodeCount = (int)(rdr["CTRL_CDCOUNT"]);
        }

         public async Task<CheckDebtorIdReturnData> CheckDebtorId(CheckDebtorIdGetData data)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Debtor_SIN",  data.DebtorSin},
                { "Debtor_Id",  data.DebtorId}
            };

            var result = await MainDB.GetDataFromStoredProcAsync<CheckDebtorIdReturnData>("fpIVR_Check_Debtor_Id",
                                                                        parameters,
                                                                        FillDataFromReaderCheckDebtorId);
            return result.FirstOrDefault();
        }

        private void FillDataFromReaderCheckDebtorId(IDBHelperReader rdr, CheckDebtorIdReturnData data)
        {
            data.DebtorIdCount = (int)(rdr["DEBTOR_IDCOUNT"]);
        }

         public async Task<CheckDebtorLetterReturnData> CheckDebtorLetter(CheckDebtorLetterGetData data)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Debtor_SIN",  data.DebtorSin},
                { "Debtor_Id",  data.DebtorId},
                { "Letter", data.Letter }
            };

            var result = await MainDB.GetDataFromStoredProcAsync<CheckDebtorLetterReturnData>("fpIVR_Check_Debtor_Letter",
                                                                        parameters,
                                                                        FillDataFromReaderCheckDebtorLetter);
            return result.FirstOrDefault();
        }

        private void FillDataFromReaderCheckDebtorLetter(IDBHelperReader rdr, CheckDebtorLetterReturnData data)
        {
            data.LetterCount = (int)(rdr["LETTERCOUNT"]);
        }

        public async Task<GetAgencyReturnData> GetAgency(GetAgencyGetData data)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Debtor_SIN",  data.DebtorSin},
                { "Debtor_Id",  data.DebtorId},
                { "Letter", data.Letter },
                { "appl_enfsrv_cd", data.EnforcementCode }
            };

            var result = await MainDB.GetDataFromStoredProcAsync<GetAgencyReturnData>("fpIVR_Get_Agency",
                                                                        parameters,
                                                                        FillDataFromReaderGetAgency);
            return result.FirstOrDefault();
        }

        private void FillDataFromReaderGetAgency(IDBHelperReader rdr, GetAgencyReturnData data)
        {
            data.EnforcementCode = rdr["EnfSrv_Cd"] as string;
            data.EnfOffAbbrCd = rdr["EnfOff_AbbrCd"] as string;
            data.AgencyType = rdr["Agency_Type"] as string;
            data.EnfOffOrgCd = rdr["EnfOff_OrgCd"] as string;
            data.EnfOffName = rdr["EnfOff_Name"] as string;
            data.EnfOffAddrLn = rdr["EnfOff_Addr_Ln"] as string;
            data.EnfOffAddrLn1 = rdr["EnfOff_Addr_Ln1"] as string;
            data.EnfOffAddrCityNme = rdr["EnfOff_Addr_CityNme"] as string;
            data.ProvCode = rdr["ProvCode"] as string;
            data.EnfSrvAddrPCd = rdr["EnfSrv_Addr_PCd"] as string;
            data.TelNumber = rdr["TelNumber"] as string;
        }

        public async Task<GetAgencyDebReturnData> GetAgencyDeb(GetAgencyDebGetData data)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Debtor_SIN",  data.DebtorSin},
                { "Debtor_Id",  data.DebtorId},
                { "debtorNrSfx", data.DebtorNrSfx }
            };

            var result = await MainDB.GetDataFromStoredProcAsync<GetAgencyDebReturnData>("fpIVR_Get_Agency_Deb",
                                                                        parameters,
                                                                        FillDataFromReaderGetAgencyDeb);
            return result.FirstOrDefault();
        }

        private void FillDataFromReaderGetAgencyDeb(IDBHelperReader rdr, GetAgencyDebReturnData data)
        {
            data.EnforcementCode = rdr["Enforcement_Cd"] as string;
        }

        public async Task<GetApplControlCodeReturnData> GetApplControlCode(GetApplControlCodeGetData data)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Debtor_SIN",  data.DebtorSin},
                { "Debtor_Id",  data.DebtorId},
                { "debtorNrSfx", data.DebtorNrSfx }
            };

            var result = await MainDB.GetDataFromStoredProcAsync<GetApplControlCodeReturnData>("fpIVR_Get_Appl_Ctrlcd",
                                                                        parameters,
                                                                        FillDataFromReaderGetApplControlCode);
            return result.FirstOrDefault();
        }

        private void FillDataFromReaderGetApplControlCode(IDBHelperReader rdr, GetApplControlCodeReturnData data)
        {
            data.ControlCode = rdr["Appl_Ctrlcd"] as string;
        }

        public async Task<GetApplEnforcementCodeReturnData> GetApplEnforcementCode(GetApplEnforcementCodeGetData data)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Debtor_SIN",  data.DebtorSin},
                { "Debtor_Id",  data.DebtorId},
                { "debtorNrSfx", data.DebtorNrSfx }
            };

            var result = await MainDB.GetDataFromStoredProcAsync<GetApplEnforcementCodeReturnData>("fpIVR_Get_Appl_Enfsrv_Cd",
                                                                        parameters,
                                                                        FillDataFromReaderGetApplEnforcementCode);
            return result.FirstOrDefault();
        }

        private void FillDataFromReaderGetApplEnforcementCode(IDBHelperReader rdr, GetApplEnforcementCodeReturnData data)
        {
            data.EnforcementCode = rdr["Appl_Enfsrv_Cd"] as string;
        }

        public async Task<GetHoldbackConditionReturnData> GetHoldbackCondition(GetHoldbackConditionGetData data)
        {
            var parameters = new Dictionary<string, object>
            {
                { "appl_enfsrv_cd",  data.EnforcementCode},
                { "appl_ctrlcd", data.ControlCode}
            };

            var result = await MainDB.GetDataFromStoredProcAsync<GetHoldbackConditionReturnData>("fpIVR_Get_HldbCnd",
                                                                        parameters,
                                                                        FillDataFromReaderGetHoldbackCondition);
            return result.FirstOrDefault();
        }

        private void FillDataFromReaderGetHoldbackCondition(IDBHelperReader rdr, GetHoldbackConditionReturnData data)
        {
            data.EnfSrcCd = rdr["EnfSrc_Cd"] as string;
            data.HldbCtgCd = rdr["HldbCtg_Cd"] as string;
            data.HldbSrcHldValue = rdr["Hldb_SrcHldValue"] as string;
            data.HldbMxmPerChq = rdr["Hldb_MxmPerChq"] as string;
        }

        public async Task<GetL01AgencyReturnData> GetL01Agency(GetL01AgencyGetData data)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Debtor_SIN",  data.DebtorSin},
                { "Ctrl_cd", data.ControlCode}
            };

            var result = await MainDB.GetDataFromStoredProcAsync<GetL01AgencyReturnData>("fpIVR_Get_L01Agency",
                                                                        parameters,
                                                                        FillDataFromReaderGetL01Agency);
            return result.FirstOrDefault();
        }

        private void FillDataFromReaderGetL01Agency(IDBHelperReader rdr, GetL01AgencyReturnData data)
        {
            data.ActvStCd = rdr["ActvSt_Cd"] as string;
            data.ApplLglDte = rdr["Appl_Lgl_Dte"] as string;
            data.EnforcementCode = rdr["EnfSrv_Cd"] as string;
            data.EnfOffAbbrCd = rdr["EnfOff_AbbrCd"] as string;
        }

        public async Task<List<GetPaymentsReturnData>> GetPayments(GetPaymentsGetData data)
        {
            var parameters = new Dictionary<string, object>
            {
                { "appl_enfsrv_cd",  data.EnforcementCode},
                { "appl_ctrlcd", data.ControlCode}
            };

            var result = await MainDB.GetDataFromStoredProcAsync<GetPaymentsReturnData>("fpIVR_Get_Payments",
                                                                        parameters,
                                                                        FillDataFromReaderGetPayments);
            return result;
        }

        private void FillDataFromReaderGetPayments(IDBHelperReader rdr, GetPaymentsReturnData data)
        {
            data.SrcDept = rdr["SrcDept"] as string;
            data.FaAmount = rdr["FA_Amount"] as string;
            data.FaDate = rdr["FA_Date"] as string;
            data.DivertReq = rdr["Divert_Req"] as string;
            data.FeeInc = rdr["Fee_Inc"] as string;
            data.MepSummons = rdr["MEP_Summons"] as string;
            data.CourtSummons = rdr["Court_Summons"] as string;
            data.DivertDate = rdr["Divert_Date"] as string;
            data.DivertAmt = rdr["Divert_Amt"] as string;
            data.FeeAmount = rdr["Fee_Amount"] as string;
            data.CrDate = rdr["CR_Date"] as string;
            data.CrAmount = rdr["CR_Amount"] as string;
        }

        public async Task<GetSummonsReturnData> GetSummons(GetSummonsGetData data)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Debtor_SIN",  data.DebtorSin},
                { "Debtor_Id",  data.DebtorId},
                { "debtorNrSfx", data.DebtorNrSfx }
            };

            var result = await MainDB.GetDataFromStoredProcAsync<GetSummonsReturnData>("fpIVR_Get_Summons",
                                                                        parameters,
                                                                        FillDataFromReaderGetSummons);
            return result.FirstOrDefault();
        }

        private void FillDataFromReaderGetSummons(IDBHelperReader rdr, GetSummonsReturnData data)
        {
            data.EnforcementCode = rdr["Appl_EnfSrv_Cd"] as string;
            data.ControlCode = rdr["Appl_ctrlcd"] as string;
            data.SrcDept = rdr["src_dept"] as string;
            data.EnfSrvNme = rdr["EnfSrv_Nme"] as string;
            data.DebtorId = rdr["DebtorId"] as string;
            data.ApplDbtrCnfrmdSin = rdr["Appl_Dbtr_Cnfrmd_SIN"] as string;
            data.Dbtr = rdr["Dbtr"] as string;
            data.Crdtr = rdr["Crdtr"] as string;
            data.ApplSourceRfrNr = rdr["Appl_Source_RfrNr"] as string;
            data.Status = rdr["Status"] as string;
            data.GarnIssue = rdr["GarnIssue"] as string;
            data.EnfOffOrgCd = rdr["EnfOff_OrgCd"] as string;
            data.CourtCd = rdr["CourtCd"] as string;
            data.GarnEffective = rdr["GarnEffective"] as string;
            data.Arrears = rdr["Arrears"] as string;
            data.PayPer = rdr["PayPer"] as string;
            data.Periodic = rdr["Periodic"] as string;
            data.Variation = rdr["Variation"] as string;
            data.Cancel = rdr["Cancel"] as string;
            data.Suspend = rdr["Suspend"] as string;
            data.Expired = rdr["Expired"] as string;
            data.Satisfied = rdr["Satisfied"] as string;
            data.LmpOwed = rdr["lmpOwed"] as string;
            data.FeeOwed = rdr["FeeOwed"] as string;
            data.PerOwed = rdr["PerOwed"] as string;
            data.LastCalc = rdr["LastCalc"] as string;
            data.HldbCtgCd = rdr["HldbCtg_Cd"] as string;
            data.HldbDefaultValue = rdr["Hldb_DefaultValue"] as string;
        }

    }
}
