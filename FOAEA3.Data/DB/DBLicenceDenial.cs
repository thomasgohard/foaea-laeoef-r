using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FOAEA3.Data.DB
{
    internal class DBLicenceDenial : DBbase, ILicenceDenialRepository
    {
        public DBLicenceDenial(IDBTools mainDB) : base(mainDB)
        {

        }

        public LicenceDenialApplicationData GetLicenceDenialData(string appl_EnfSrv_Cd, string appl_L01_CtrlCd = null , string appl_L03_CtrlCd = null)
        {


            var parameters = new Dictionary<string, object>
                    {
                        {"Appl_EnfSrv_Cd", appl_EnfSrv_Cd}
                    };

            if (!string.IsNullOrEmpty(appl_L01_CtrlCd))
                parameters.Add("appl_L01_CtrlCd", appl_L01_CtrlCd);

            if (!string.IsNullOrEmpty(appl_L03_CtrlCd))
                parameters.Add("appl_L03_CtrlCd", appl_L03_CtrlCd);

            var data = MainDB.GetDataFromStoredProc<LicenceDenialApplicationData>("LicSusp_Select", parameters, FillDataFromReader);

            var result = data.FirstOrDefault();

            return result; 
        }

        public List<LicenceSuspensionHistoryData> GetLicenceSuspensionHistory(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            var parameters = new Dictionary<string, object>
                    {
                        {"ControlCode", appl_CtrlCd },
                        {"EnforcementServiceCode", appl_EnfSrv_Cd}
                    };

            var data = MainDB.GetDataFromStoredProc<LicenceSuspensionHistoryData>("LicSuspApplGetLicenseSuspensionHist", parameters, FillSuspensionHistoryDataFromReader);

            return data;
        }

        public void CreateLicenceDenialData(LicenceDenialApplicationData data)
        {
            var parameters = SetParameters(data);

            MainDB.ExecProc("LicSuspInsert", parameters);

        }

        public void UpdateLicenceDenialData(LicenceDenialApplicationData data)
        {
            var parameters = SetParameters(data);

            MainDB.ExecProc("LicSuspUpdate", parameters);
        }

        public bool CloseSameDayLicenceEvent(string appl_EnfSrv_Cd, string appl_L01_CtrlCd, string appl_L03_CtrlCd)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Appl_EnfSrv_Cd", appl_EnfSrv_Cd },
                { "L01Appl_CtrlCd", appl_L01_CtrlCd },
                { "L03Appl_CtrlCd", appl_L03_CtrlCd }
            };

            return MainDB.GetDataFromStoredProcViaReturnParameter<bool>("CloseSameDayLicenseEvent", parameters, "L01Event");
        }

        private Dictionary<string, object> SetParameters(LicenceDenialApplicationData data)
        {
            var parameters = new Dictionary<string, object>
                    {
                        {"Appl_EnfSrv_Cd", data.Appl_EnfSrv_Cd },
                        {"Appl_CtrlCd", data.Appl_CtrlCd },
                        {"LicSusp_SupportOrder_Dte", data.LicSusp_SupportOrder_Dte },
                        {"LicSusp_NoticeSentToDbtr_Dte", data.LicSusp_NoticeSentToDbtr_Dte },
                        {"LicSusp_CourtNme", data.LicSusp_CourtNme },
                        {"PymPr_Cd", data.PymPr_Cd },
                        {"LicSusp_NrOfPymntsInDefault", data.LicSusp_NrOfPymntsInDefault ?? 0},
                        {"LicSusp_AmntOfArrears", data.LicSusp_AmntOfArrears ?? 0M },
                        {"LicSusp_Dbtr_EmplNme", data.LicSusp_Dbtr_EmplNme },
                        {"LicSusp_Dbtr_EmplAddr_Ln", data.LicSusp_Dbtr_EmplAddr_Ln },
                        {"LicSusp_Dbtr_EmplAddr_Ln1", data.LicSusp_Dbtr_EmplAddr_Ln1 },
                        {"LicSusp_Dbtr_EmplAddr_CityNme", data.LicSusp_Dbtr_EmplAddr_CityNme },
                        {"LicSusp_Dbtr_EmplAddr_PrvCd", data.LicSusp_Dbtr_EmplAddr_PrvCd },
                        {"LicSusp_Dbtr_EmplAddr_CtryCd", data.LicSusp_Dbtr_EmplAddr_CtryCd },
                        {"LicSusp_Dbtr_EmplAddr_PCd", data.LicSusp_Dbtr_EmplAddr_PCd },
                        {"LicSusp_Dbtr_EyesColorCd", data.LicSusp_Dbtr_EyesColorCd },
                        {"LicSusp_Dbtr_HeightUOMCd", data.LicSusp_Dbtr_HeightUOMCd },
                        {"LicSusp_Dbtr_HeightQty", data.LicSusp_Dbtr_HeightQty ?? 0},
                        {"LicSusp_Dbtr_PhoneNumber", data.LicSusp_Dbtr_PhoneNumber },
                        {"LicSusp_Dbtr_EmailAddress", data.LicSusp_Dbtr_EmailAddress },
                        {"LicSusp_Dbtr_Brth_CityNme", data.LicSusp_Dbtr_Brth_CityNme },
                        {"LicSusp_Dbtr_Brth_CtryCd", data.LicSusp_Dbtr_Brth_CtryCd },
                        {"LicSusp_Still_InEffect_Ind", data.LicSusp_Still_InEffect_Ind },
                        {"LicSusp_AnyLicRvkd_Ind", data.LicSusp_AnyLicRvkd_Ind },
                        {"LicSusp_AnyLicReinst_Ind", data.LicSusp_AnyLicReinst_Ind },
                        {"LicSusp_LiStCd", data.LicSusp_LiStCd },
                        {"LicSusp_Appl_CtrlCd", data.LicSusp_Appl_CtrlCd }
                    };

            if (data.LicSusp_TermRequestDte.HasValue)
                parameters.Add("LicSusp_TermRequestDte", data.LicSusp_TermRequestDte.Value);
            else
                parameters.Add("LicSusp_TermRequestDte", DBNull.Value);

            if (!string.IsNullOrEmpty(data.LicSusp_Dbtr_LastAddr_Ln))
                parameters.Add("LicSusp_Dbtr_LastAddr_Ln", data.LicSusp_Dbtr_LastAddr_Ln);

            if (!string.IsNullOrEmpty(data.LicSusp_Dbtr_LastAddr_Ln1))
                parameters.Add("LicSusp_Dbtr_LastAddr_Ln1", data.LicSusp_Dbtr_LastAddr_Ln1);

            if (!string.IsNullOrEmpty(data.LicSusp_Dbtr_LastAddr_CityNme))
                parameters.Add("LicSusp_Dbtr_LastAddr_CityNme", data.LicSusp_Dbtr_LastAddr_CityNme);

            if (!string.IsNullOrEmpty(data.LicSusp_Dbtr_LastAddr_PrvCd))
                parameters.Add("LicSusp_Dbtr_LastAddr_PrvCd", data.LicSusp_Dbtr_LastAddr_PrvCd);

            if (!string.IsNullOrEmpty(data.LicSusp_Dbtr_LastAddr_CtryCd))
                parameters.Add("LicSusp_Dbtr_LastAddr_CtryCd", data.LicSusp_Dbtr_LastAddr_CtryCd);

            if (!string.IsNullOrEmpty(data.LicSusp_Dbtr_LastAddr_PCd))
                parameters.Add("LicSusp_Dbtr_LastAddr_PCd", data.LicSusp_Dbtr_LastAddr_PCd);

            if (data.LicSusp_Declaration_Ind.HasValue)
                parameters.Add("LicSusp_Declaration_Ind", data.LicSusp_Declaration_Ind.Value);

            return parameters;
        }

        private void FillDataFromReader(IDBHelperReader rdr, LicenceDenialApplicationData data)
        {
            data.Appl_EnfSrv_Cd = rdr["Appl_EnfSrv_Cd"] as string;
            data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string;
            data.LicSusp_SupportOrder_Dte = (DateTime)rdr["LicSusp_SupportOrder_Dte"];
            data.LicSusp_NoticeSentToDbtr_Dte = (DateTime)rdr["LicSusp_NoticeSentToDbtr_Dte"];
            data.LicSusp_CourtNme = rdr["LicSusp_CourtNme"] as string;
            data.PymPr_Cd = rdr["PymPr_Cd"] as string;
            data.LicSusp_NrOfPymntsInDefault = rdr["LicSusp_NrOfPymntsInDefault"] as short?; // can be null 
            data.LicSusp_AmntOfArrears = rdr["LicSusp_AmntOfArrears"] as decimal?; // can be null 
            data.LicSusp_Dbtr_EmplNme = rdr["LicSusp_Dbtr_EmplNme"] as string; // can be null 
            data.LicSusp_Dbtr_EmplAddr_Ln = rdr["LicSusp_Dbtr_EmplAddr_Ln"] as string; // can be null 
            data.LicSusp_Dbtr_EmplAddr_Ln1 = rdr["LicSusp_Dbtr_EmplAddr_Ln1"] as string; // can be null 
            data.LicSusp_Dbtr_EmplAddr_CityNme = rdr["LicSusp_Dbtr_EmplAddr_CityNme"] as string; // can be null 
            data.LicSusp_Dbtr_EmplAddr_PrvCd = rdr["LicSusp_Dbtr_EmplAddr_PrvCd"] as string; // can be null 
            data.LicSusp_Dbtr_EmplAddr_CtryCd = rdr["LicSusp_Dbtr_EmplAddr_CtryCd"] as string; // can be null 
            data.LicSusp_Dbtr_EmplAddr_PCd = rdr["LicSusp_Dbtr_EmplAddr_PCd"] as string; // can be null 
            data.LicSusp_Dbtr_EyesColorCd = rdr["LicSusp_Dbtr_EyesColorCd"] as string; // can be null 
            data.LicSusp_Dbtr_HeightUOMCd = rdr["LicSusp_Dbtr_HeightUOMCd"] as string; // can be null 
            data.LicSusp_Dbtr_HeightQty = rdr["LicSusp_Dbtr_HeightQty"] as int?; // can be null 
            data.LicSusp_Dbtr_Brth_CityNme = rdr["LicSusp_Dbtr_Brth_CityNme"] as string; // can be null 
            data.LicSusp_Dbtr_Brth_CtryCd = rdr["LicSusp_Dbtr_Brth_CtryCd"] as string;
            data.LicSusp_TermRequestDte = rdr["LicSusp_TermRequestDte"] as DateTime?; // can be null 
            data.LicSusp_Still_InEffect_Ind = (byte)rdr["LicSusp_Still_InEffect_Ind"];
            data.LicSusp_AnyLicRvkd_Ind = (byte)rdr["LicSusp_AnyLicRvkd_Ind"];
            data.LicSusp_AnyLicReinst_Ind = (byte)rdr["LicSusp_AnyLicReinst_Ind"];
            data.LicSusp_LiStCd = (short)rdr["LicSusp_LiStCd"];
            data.LicSusp_Appl_CtrlCd = rdr["LicSusp_Appl_CtrlCd"] as string; // can be null 
            data.LicSusp_Dbtr_LastAddr_Ln = rdr["LicSusp_Dbtr_LastAddr_Ln"] as string; // can be null 
            data.LicSusp_Dbtr_LastAddr_Ln1 = rdr["LicSusp_Dbtr_LastAddr_Ln1"] as string; // can be null 
            data.LicSusp_Dbtr_LastAddr_CityNme = rdr["LicSusp_Dbtr_LastAddr_CityNme"] as string; // can be null 
            data.LicSusp_Dbtr_LastAddr_PrvCd = rdr["LicSusp_Dbtr_LastAddr_PrvCd"] as string; // can be null 
            data.LicSusp_Dbtr_LastAddr_CtryCd = rdr["LicSusp_Dbtr_LastAddr_CtryCd"] as string; // can be null 
            data.LicSusp_Dbtr_LastAddr_PCd = rdr["LicSusp_Dbtr_LastAddr_PCd"] as string; // can be null 
        }

        private void FillSuspensionHistoryDataFromReader(IDBHelperReader rdr, LicenceSuspensionHistoryData data)
        {
            data.EnforcementServiceCode = rdr["EnforcementServiceCode"] as string;
            data.ControlCode = rdr["ControlCode"] as string;
            data.FromName = rdr["FromName"] as string;
            data.ConvertedResponseDate = rdr["ConvertedResponseDate"] as string;
            data.ResponseDate = (DateTime)rdr["ResponseDate"];
            data.ResponseCode = (short)rdr["ResponseCode"];
            data.LicRspSource_RefNo = rdr["LicRspSource_RefNo"] as string;
            data.ResponseDescription_E = rdr["ResponseDescription_E"] as string;
            data.ResponseDescription_F = rdr["ResponseDescription_F"] as string;
        }

    }
}
