using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Enums;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBSINResult : DBbase, ISINResultRepository
    {
        public DBSINResult(IDBToolsAsync mainDB) : base(mainDB)
        {

        }

        public MessageDataList Messages { get; set; }

        public async Task CreateSINResults(SINResultData resultData)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Appl_EnfSrv_Cd", resultData.Appl_EnfSrv_Cd },
                { "Appl_CtrlCd", resultData.Appl_CtrlCd },
                { "SVR_TimeStamp", resultData.SVR_TimeStamp },
                { "ValStat_Cd", resultData.ValStat_Cd },
                { "ActvSt_Cd", resultData.ActvSt_Cd }
            };

            if (!string.IsNullOrEmpty(resultData.SVR_TolCd))
                parameters.Add("SVR_TolCd", resultData.SVR_TolCd);

            if (!string.IsNullOrEmpty(resultData.SVR_SIN))
                parameters.Add("SVR_SIN", resultData.SVR_SIN);

            if (resultData.SVR_DOB_TolCd.HasValue)
                parameters.Add("SVR_DOB_TolCd", resultData.SVR_DOB_TolCd);

            if (resultData.SVR_GvnNme_TolCd.HasValue)
                parameters.Add("SVR_GvnNme_TolCd", resultData.SVR_GvnNme_TolCd);

            if (resultData.SVR_SurNme_TolCd.HasValue)
                parameters.Add("SVR_SurNme_TolCd", resultData.SVR_SurNme_TolCd);

            if (resultData.SVR_MotherNme_TolCd.HasValue)
                parameters.Add("SVR_ParentNme_TolCd", resultData.SVR_MotherNme_TolCd);

            if (resultData.SVR_Gendr_TolCd.HasValue)
                parameters.Add("SVR_Gendr_TolCd", resultData.SVR_Gendr_TolCd);

            await MainDB.ExecProcAsync("SINValRslt_Insert", parameters);

        }

        public async Task<DataList<SINResultData>> GetSINResults(string applEnfSrvCd, string applCtrlCd)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"applEnfSrvCd", applEnfSrvCd},
                    {"applCtrlCd", applCtrlCd}
                };

            var data = await MainDB.GetDataFromStoredProcAsync<SINResultData>("DataModGetSINValData", parameters, FillSINResultDataFromReader);

            return new DataList<SINResultData>(data, MainDB.LastError);
        }

        public async Task<DataList<SINResultWithHistoryData>> GetSINResultsWithHistory(string applEnfSrvCd, string applCtrlCd)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"Appl_EnfSrv_Cd", applEnfSrvCd},
                    {"Appl_CtrlCd", applCtrlCd}
                };

            var data = await MainDB.GetDataFromStoredProcAsync<SINResultWithHistoryData>("SummAccGetSINVerificationResultsList", parameters, FillSINResultWithHistoryDataFromReader);

            return new DataList<SINResultWithHistoryData>(data, MainDB.LastError);
        }

        public async Task<List<SINOutgoingFederalData>> GetFederalSINOutgoingData(int maxRecords,
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

            return await MainDB.GetDataFromStoredProcAsync<SINOutgoingFederalData>("MessageBrokerGetSINOUTOutboundData",
                                                                        parameters, FillSINOutgoingFederalData);
        }

        public async Task InsertBulkData(List<SINResultData> responseData)
        {
            await MainDB.BulkUpdateAsync<SINResultData>(responseData, "SINValRslt");
        }

        private void FillSINOutgoingFederalData(IDBHelperReader rdr, SINOutgoingFederalData data)
        {
            data.Event_dtl_Id = (int)rdr["Event_dtl_Id"];
            data.Event_Reas_Cd = rdr["Event_Reas_Cd"] as string;
            data.Event_Reas_Text = rdr["Event_Reas_Text"] as string;
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
            data.Recordtype = rdr["Recordtype"] as string;
            data.Appl_EnfSrv_Cd = rdr["Val_1"] as string; // Val_1
            data.Appl_CtrlCd = rdr["Val_2"] as string; // Val_2
            data.Appl_Dbtr_Entrd_SIN = rdr["Val_3"] as string; // Val_3
            data.Appl_Dbtr_FrstNme = rdr["Val_4"] as string; // Val_4
            data.Appl_Dbtr_MddleNme = rdr["Val_5"] as string; // Val_5
            data.Appl_Dbtr_SurNme = rdr["Val_6"] as string; // Val_6
            data.Appl_Dbtr_Parent_SurNme = rdr["Val_7"] as string; // Val_7
            data.Appl_Dbtr_Gendr_Cd = rdr["Val_8"] as string; // Val_8
            data.Appl_Dbtr_Brth_Dte = rdr["Val_9"] as string; // Val_9
        }

        private void FillSINResultWithHistoryDataFromReader(IDBHelperReader rdr, SINResultWithHistoryData data)
        {
            data.Appl_EnfSrv_Cd = rdr["Appl_EnfSrv_Cd"] as string;
            data.Subm_SubmCd = rdr["Subm_SubmCd"] as string;
            data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string;
            data.Appl_Source_RfrNr = rdr["Appl_Source_RfrNr"] as string; // can be null 
            data.SVR_TimeStamp = (DateTime)rdr["SVR_TimeStamp"];
            data.SVR_DOB_TolCd = rdr["SVR_DOB_TolCd"] as string; // can be null 
            data.SVR_GvnNme_TolCd = rdr["SVR_GvnNme_TolCd"] as string; // can be null 
            data.SVR_MddlNme_TolCd = rdr["SVR_MddlNme_TolCd"] as string; // can be null 
            data.SVR_SurNme_TolCd = rdr["SVR_SurNme_TolCd"] as string; // can be null 
            //data.SVR_MotherNme_TolCd = rdr["SVR_MotherNme_TolCd"] as string; // can be null 
            data.SVR_MotherNme_TolCd = rdr["SVR_ParentNme_TolCd"] as string; // can be null 
            data.SVR_Gendr_TolCd = rdr["SVR_Gendr_TolCd"] as string; // can be null 
            data.SVR_TolCd = rdr["SVR_TolCd"] as string; // can be null 
            data.ValStat_Cd = (short)rdr["ValStat_Cd"];
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
            data.Appl_Dbtr_Brth_Dte = rdr["Appl_Dbtr_Brth_Dte"] as DateTime?; // can be null 
            data.Appl_Dbtr_FrstNme = rdr["Appl_Dbtr_FrstNme"] as string; // can be null 
            data.Appl_Dbtr_MddleNme = rdr["Appl_Dbtr_MddleNme"] as string; // can be null 
            data.Appl_Dbtr_SurNme = rdr["Appl_Dbtr_SurNme"] as string; // can be null 
            data.Appl_Dbtr_Parent_SurNme = rdr["Appl_Dbtr_Parent_SurNme"] as string; // can be null 
            data.Appl_Dbtr_Gendr_Cd = rdr["Appl_Dbtr_Gendr_Cd"] as string; // can be null 
            data.Appl_Dbtr_Entrd_SIN = rdr["Appl_Dbtr_Entrd_SIN"] as string; // can be null 
        }

        private void FillSINResultDataFromReader(IDBHelperReader rdr, SINResultData data)
        {
            data.Appl_EnfSrv_Cd = rdr["Appl_EnfSrv_Cd"] as string;
            data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string;
            data.SVR_TimeStamp = (DateTime)rdr["SVR_TimeStamp"];
            data.SVR_TolCd = rdr["SVR_TolCd"] as string; // can be null 
            data.SVR_SIN = rdr["SVR_SIN"] as string; // can be null 
            data.SVR_DOB_TolCd = rdr["SVR_DOB_TolCd"] as short?; // can be null 
            data.SVR_GvnNme_TolCd = rdr["SVR_GvnNme_TolCd"] as short?; // can be null 
            data.SVR_MddlNme_TolCd = rdr["SVR_MddlNme_TolCd"] as short?; // can be null 
            data.SVR_SurNme_TolCd = rdr["SVR_SurNme_TolCd"] as short?; // can be null 
            data.SVR_MotherNme_TolCd = rdr["SVR_ParentNme_TolCd"] as short?; // can be null 
                                                                             // data.SVR_MotherNme_TolCd = rdr["SVR_MotherNme_TolCd"] as short?; // can be null 
            data.SVR_Gendr_TolCd = rdr["SVR_Gendr_TolCd"] as short?; // can be null 
            data.ValStat_Cd = (short)rdr["ValStat_Cd"];
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
        }
    }
}
