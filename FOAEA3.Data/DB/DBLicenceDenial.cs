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
    internal class DBLicenceDenial : DBbase, ILicenceDenialRepository
    {
        public DBLicenceDenial(IDBToolsAsync mainDB) : base(mainDB)
        {

        }

        public async Task<LicenceDenialApplicationData> GetLicenceDenialData(string appl_EnfSrv_Cd, string appl_L01_CtrlCd = null, string appl_L03_CtrlCd = null)
        {


            var parameters = new Dictionary<string, object>
                    {
                        {"Appl_EnfSrv_Cd", appl_EnfSrv_Cd}
                    };

            if (!string.IsNullOrEmpty(appl_L01_CtrlCd))
                parameters.Add("appl_L01_CtrlCd", appl_L01_CtrlCd);

            if (!string.IsNullOrEmpty(appl_L03_CtrlCd))
                parameters.Add("appl_L03_CtrlCd", appl_L03_CtrlCd);

            var data = await MainDB.GetDataFromStoredProcAsync<LicenceDenialApplicationData>("LicSusp_Select", parameters, FillDataFromReader);

            var result = data.FirstOrDefault();

            return result;
        }

        public async Task<List<LicenceSuspensionHistoryData>> GetLicenceSuspensionHistory(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            var parameters = new Dictionary<string, object>
                    {
                        {"ControlCode", appl_CtrlCd },
                        {"EnforcementServiceCode", appl_EnfSrv_Cd}
                    };

            var data = await MainDB.GetDataFromStoredProcAsync<LicenceSuspensionHistoryData>("LicSuspApplGetLicenseSuspensionHist", parameters, FillSuspensionHistoryDataFromReader);

            return data;
        }

        public async Task CreateLicenceDenialData(LicenceDenialApplicationData data)
        {
            var parameters = SetParameters(data);

            await MainDB.ExecProcAsync("LicSuspInsert", parameters);

        }

        public async Task UpdateLicenceDenialData(LicenceDenialApplicationData data)
        {
            var parameters = SetParameters(data);

            await MainDB.ExecProcAsync("LicSuspUpdate", parameters);
        }

        public async Task<bool> CloseSameDayLicenceEvent(string appl_EnfSrv_Cd, string appl_L01_CtrlCd, string appl_L03_CtrlCd)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Appl_EnfSrv_Cd", appl_EnfSrv_Cd },
                { "L01Appl_CtrlCd", appl_L01_CtrlCd },
                { "L03Appl_CtrlCd", appl_L03_CtrlCd }
            };

            return await MainDB.GetDataFromStoredProcViaReturnParameterAsync<bool>("CloseSameDayLicenseEvent", parameters, "L01Event");
        }

        private static Dictionary<string, object> SetParameters(LicenceDenialApplicationData data)
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
                        {"LicSusp_Still_InEffect_Ind", data.LicSusp_Still_InEffect_Ind },
                        {"LicSusp_AnyLicRvkd_Ind", data.LicSusp_AnyLicRvkd_Ind },
                        {"LicSusp_AnyLicReinst_Ind", data.LicSusp_AnyLicReinst_Ind },
                        {"LicSusp_LiStCd", data.LicSusp_LiStCd },
                    };

            if (!string.IsNullOrEmpty(data.LicSusp_Dbtr_EmplNme))
                parameters.Add("LicSusp_Dbtr_EmplNme", data.LicSusp_Dbtr_EmplNme);
            else
                parameters.Add("LicSusp_Dbtr_EmplNme", DBNull.Value);

            if (!string.IsNullOrEmpty(data.LicSusp_Dbtr_EmplAddr_Ln))
                parameters.Add("LicSusp_Dbtr_EmplAddr_Ln", data.LicSusp_Dbtr_EmplAddr_Ln);
            else
                parameters.Add("LicSusp_Dbtr_EmplAddr_Ln", DBNull.Value);

            if (!string.IsNullOrEmpty(data.LicSusp_Dbtr_EmplAddr_Ln1))
                parameters.Add("LicSusp_Dbtr_EmplAddr_Ln1", data.LicSusp_Dbtr_EmplAddr_Ln1);
            else
                parameters.Add("LicSusp_Dbtr_EmplAddr_Ln1", DBNull.Value);

            if (!string.IsNullOrEmpty(data.LicSusp_Dbtr_EmplAddr_CityNme))
                parameters.Add("LicSusp_Dbtr_EmplAddr_CityNme", data.LicSusp_Dbtr_EmplAddr_CityNme);
            else
                parameters.Add("LicSusp_Dbtr_EmplAddr_CityNme", DBNull.Value);

            if (!string.IsNullOrEmpty(data.LicSusp_Dbtr_EmplAddr_PrvCd))
                parameters.Add("LicSusp_Dbtr_EmplAddr_PrvCd", data.LicSusp_Dbtr_EmplAddr_PrvCd);
            else
                parameters.Add("LicSusp_Dbtr_EmplAddr_PrvCd", DBNull.Value);

            if (!string.IsNullOrEmpty(data.LicSusp_Dbtr_EmplAddr_CtryCd))
                parameters.Add("LicSusp_Dbtr_EmplAddr_CtryCd", data.LicSusp_Dbtr_EmplAddr_CtryCd);
            else
                parameters.Add("LicSusp_Dbtr_EmplAddr_CtryCd", DBNull.Value);

            if (!string.IsNullOrEmpty(data.LicSusp_Dbtr_EmplAddr_PCd))
                parameters.Add("LicSusp_Dbtr_EmplAddr_PCd", data.LicSusp_Dbtr_EmplAddr_PCd);
            else
                parameters.Add("LicSusp_Dbtr_EmplAddr_PCd", DBNull.Value);

            if (!string.IsNullOrEmpty(data.LicSusp_Dbtr_EyesColorCd))
                parameters.Add("LicSusp_Dbtr_EyesColorCd", data.LicSusp_Dbtr_EyesColorCd);
            else
                parameters.Add("LicSusp_Dbtr_EyesColorCd", "NUL");

            if (!string.IsNullOrEmpty(data.LicSusp_Dbtr_HeightUOMCd))
                parameters.Add("LicSusp_Dbtr_HeightUOMCd", data.LicSusp_Dbtr_HeightUOMCd);
            else
                parameters.Add("LicSusp_Dbtr_HeightUOMCd", DBNull.Value);

            if (!string.IsNullOrEmpty(data.LicSusp_Dbtr_PhoneNumber))
                parameters.Add("LicSusp_Dbtr_PhoneNumber", data.LicSusp_Dbtr_PhoneNumber);
            else
                parameters.Add("LicSusp_Dbtr_PhoneNumber", DBNull.Value);

            if (!string.IsNullOrEmpty(data.LicSusp_Dbtr_EmailAddress))
                parameters.Add("LicSusp_Dbtr_EmailAddress", data.LicSusp_Dbtr_EmailAddress);
            else
                parameters.Add("LicSusp_Dbtr_EmailAddress", DBNull.Value);

            if (!string.IsNullOrEmpty(data.LicSusp_Dbtr_Brth_CityNme))
                parameters.Add("LicSusp_Dbtr_Brth_CityNme", data.LicSusp_Dbtr_Brth_CityNme);
            else
                parameters.Add("LicSusp_Dbtr_Brth_CityNme", DBNull.Value);

            if (!string.IsNullOrEmpty(data.LicSusp_Dbtr_Brth_CtryCd))
                parameters.Add("LicSusp_Dbtr_Brth_CtryCd", data.LicSusp_Dbtr_Brth_CtryCd);
            else
                parameters.Add("LicSusp_Dbtr_Brth_CtryCd", "999");

            if (!string.IsNullOrEmpty(data.LicSusp_Appl_CtrlCd))
                parameters.Add("LicSusp_Appl_CtrlCd", data.LicSusp_Appl_CtrlCd);
            else
                parameters.Add("LicSusp_Appl_CtrlCd", DBNull.Value);

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

            if (data.LicSusp_Dbtr_HeightQty.HasValue)
                parameters.Add("LicSusp_Dbtr_HeightQty", data.LicSusp_Dbtr_HeightQty.Value);
            else
                parameters.Add("LicSusp_Dbtr_HeightQty", DBNull.Value);

            return parameters;
        }

        public async Task<List<LicenceDenialOutgoingFederalData>> GetFederalOutgoingData(int maxRecords,
                                                                      string activeState,
                                                                      ApplicationState lifeState,
                                                                      string enfServiceCode)
        {

            var parameters = new Dictionary<string, object>
            {
                { "intRecMax", maxRecords },
                { "dchrActvSt_Cd", activeState },
                { "sntAppLiSt_Cd", lifeState },
                { "chrEnfSrv_Cd", enfServiceCode }
            };

            return await MainDB.GetDataFromStoredProcAsync<LicenceDenialOutgoingFederalData>("MessageBrokerGetLICOUTOutboundData",
                                                                            parameters, FillLicenceDenialOutgoingFederalData);

        }

        public async Task<List<LicenceDenialToApplData>> GetLicenceDenialToApplData(string fedSource)
        {
            var parameters = new Dictionary<string, object>
            {
                { "chrEnfSrv_Cd", fedSource }
            };

            return await MainDB.GetDataFromStoredProcAsync<LicenceDenialToApplData>("MessageBrokerGetLICENSEInboundToApplData",
                                                                         parameters, FillLicenceDenialToApplDataFromReader);
        }

        public async Task<List<LicenceDenialOutgoingProvincialData>> GetProvincialOutgoingData(int maxRecords, string activeState, string recipientCode, bool isXML = true)
        {
            var parameters = new Dictionary<string, object>
            {
                { "intRecMax", maxRecords },
                { "dchrActvSt_Cd", activeState },
                { "chrRecptCd", recipientCode },
                { "isXML", isXML ? 1 : 0 }
            };

            var data = await MainDB.GetDataFromStoredProcAsync<LicenceDenialOutgoingProvincialData>("MessageBrokerGetLICAPPOUTOutboundData",
                                                                                 parameters, FillLicenceDenialOutgoingProvincialData);
            return data;

        }

        public async Task<List<SingleStringColumnData>> GetActiveLO1ApplsForDebtor(string appl_EnfSrv_Cd, string appl_CtrlCd)
        {
            var parameters = new Dictionary<string, object>
                    {
                        {"Appl_CtrlCd", appl_CtrlCd },
                        {"Appl_EnfSrv_Cd", appl_EnfSrv_Cd}
                    };

            var data = await MainDB.GetDataFromStoredProcAsync<SingleStringColumnData>("ApplGetL01sForDebtor", parameters, FillActiveL01ApplsForDebtor);

            return data;
        }

        private void FillActiveL01ApplsForDebtor(IDBHelperReader rdr, SingleStringColumnData data)
        {
            data.Value = rdr[0] as string;
        }

        private void FillLicenceDenialToApplDataFromReader(IDBHelperReader rdr, LicenceDenialToApplData data)
        {
            data.Dtl_Reas_Cd = (int?)rdr["dtl_Reas_Cd"];
            data.Dtl_List = (short)rdr["dtl_List"];
            data.Dtl_ActvSt = rdr["dtl_ActvSt"] as string;
            data.Dtl_Id = (int)rdr["dtl_Id"];
            data.Event_Reas_Cd = (int)rdr["Event_Reas_Cd"];
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
            data.Event_Id = (int)rdr["Event_Id"];
            data.Appl_EnfSrv_Cd = rdr["Appl_EnfSrv_Cd"] as string;
            data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string;
        }

        private void FillLicenceDenialOutgoingFederalData(IDBHelperReader rdr, LicenceDenialOutgoingFederalData data)
        {
            data.Event_dtl_Id = (int)rdr["Event_dtl_Id"];
            data.Event_Reas_Cd = (int?)rdr["Event_Reas_Cd"];
            data.Event_Reas_Text = rdr["Event_Reas_Text"] as string;
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
            data.Recordtype = rdr["Recordtype"] as string;
            data.RequestType = rdr["Val_1"] as string;
            data.Appl_EnfSrv_Cd = rdr["Val_2"] as string;
            data.Appl_CtrlCd = rdr["Val_3"] as string;
            data.Appl_Dbtr_Cnfrmd_SIN = rdr["Val_4"] as string;
            data.Appl_Dbtr_FrstNme = rdr["Val_5"] as string;
            data.Appl_Dbtr_MddleNme = rdr["Val_6"] as string;
            data.Appl_Dbtr_SurNme = rdr["Val_7"] as string;
            data.Appl_Dbtr_Parent_SurNme = rdr["Val_8"] as string;
            data.Appl_Dbtr_Gendr_Cd = rdr["Val_9"] as string;
            data.Appl_Dbtr_Brth_Dte = rdr["Val_10"] as string;
            data.LicSusp_Dbtr_Brth_CityNme = rdr["Val_11"] as string;
            data.LicSusp_Dbtr_Brth_CtryCd = rdr["Val_12"] as string;
            data.LicSusp_Dbtr_EyesColorCd = rdr["Val_13"] as string;
            data.LicSusp_Dbtr_HeightQty = rdr["Val_14"] as string;
            data.Dbtr_Addr_Ln = rdr["Val_15"] as string;
            data.Dbtr_Addr_Ln1 = rdr["Val_16"] as string;
            data.Dbtr_Addr_CityNme = rdr["Val_17"] as string;
            data.Dbtr_Addr_PrvCd = rdr["Val_18"] as string;
            data.Dbtr_Addr_PCd = rdr["Val_19"] as string;
            data.Dbtr_Addr_CtryCd = rdr["Val_20"] as string;
            data.LicSusp_Dbtr_EmplNme = rdr["Val_21"] as string;
            data.LicSusp_Dbtr_EmplAddr_Ln = rdr["Val_22"] as string;
            data.LicSusp_Dbtr_EmplAddr_Ln1 = rdr["Val_23"] as string;
            data.LicSusp_Dbtr_EmplAddr_CityNme = rdr["Val_24"] as string;
            data.LicSusp_Dbtr_EmplAddr_PrvCd = rdr["Val_25"] as string;
            data.LicSusp_Dbtr_EmplAddr_PCd = rdr["Val_26"] as string;
            data.LicSusp_Dbtr_EmplAddr_CtryCd = rdr["Val_27"] as string;
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

        private void FillLicenceDenialOutgoingProvincialData(IDBHelperReader rdr, LicenceDenialOutgoingProvincialData data)
        {
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
            data.Recordtype = rdr["Recordtype"] as string;
            data.Appl_EnfSrv_Cd = rdr["Val_1"] as string;
            data.Subm_SubmCd = rdr["Val_2"] as string;
            data.Appl_CtrlCd = rdr["Val_3"] as string;
            data.Appl_Source_RfrNr = rdr["Val_4"] as string;
            data.Subm_Recpt_SubmCd = rdr["Val_5"] as string;
            data.LicRsp_Rcpt_Dte = (DateTime)rdr["Val_6"];
            data.LicRsp_SeqNr = rdr["Val_7"] as string;
            data.RqstStat_Cd = (short)rdr["Val_8"];
            data.EnfSrv_Cd = rdr["Val_9"] as string;
        }

    }
}
