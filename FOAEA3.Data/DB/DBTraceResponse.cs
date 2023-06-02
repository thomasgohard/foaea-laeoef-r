using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBTraceResponse : DBbase, ITraceResponseRepository
    {
        public DBTraceResponse(IDBToolsAsync mainDB) : base(mainDB)
        {

        }

        public MessageDataList Messages { get; set; }

        public async Task<DataList<TraceResponseData>> GetTraceResponseForApplicationAsync(string applEnfSrvCd, string applCtrlCd, bool checkCycle = false)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"EnfSrv_Cd", applEnfSrvCd },
                    {"CtrlCd", applCtrlCd },
                    {"chkCycle", checkCycle }
                };

            var data = await MainDB.GetDataFromStoredProcAsync<TraceResponseData>("TraceResultsGetTrace", parameters, FillTraceResultDataFromReader);

            return new DataList<TraceResponseData>(data, MainDB.LastError);
        }

        public async Task<List<CraFieldData>> GetCraFields()
        {
            return await MainDB.GetAllDataAsync<CraFieldData>("CraField", FillCraFieldFromReader);
        }

        private void FillCraFieldFromReader(IDBHelperReader rdr, CraFieldData data)
        {
            data.CRAField_Id = (int)rdr["CRAField_Id"];
            data.CRAFormSchedule = rdr["CRAFormSchedule"] as string; // can be null 
            data.CRAFieldCodeOld = rdr["CRAFieldCodeOld"] as string; // can be null 
            data.CRAFieldCode = rdr["CRAFieldCode"] as string; // can be null 
            data.CRAComment = rdr["CRAComment"] as string; // can be null 
            data.CRAFieldName = rdr["CRAFieldName"] as string; // can be null 
            data.CRAFieldDescriptionEnglish = rdr["CRAFieldDescriptionEnglish"] as string; // can be null 
            data.CRAFieldDescriptionFrench = rdr["CRAFieldDescriptionFrench"] as string; // can be null 
        }

        public async Task<List<CraFormData>> GetCraForms()
        {
            return await MainDB.GetAllDataAsync<CraFormData>("CraForm", FillCraFormFromReader);
        }

        private void FillCraFormFromReader(IDBHelperReader rdr, CraFormData data)
        {
            data.CRAForm_Id = (int)rdr["CRAForm_Id"];
            data.CRAFormYear = (int)rdr["CRAFormYear"];
            data.CRAFormDescription = rdr["CRAFormDescription"] as string;
            data.CRAFormPDFName = rdr["CRAFormPDFName"] as string;
            data.CRAFormSchedule = rdr["CRAFormSchedule"] as string;
            data.CRAFormProvince = rdr["CRAFormProvince"] as string;
            data.CRAFormLanguage = rdr["CRAFormLanguage"] as string; // can be null 
        }

        public async Task InsertBulkDataAsync(List<TraceResponseData> responseData)
        {
            await MainDB.BulkUpdateAsync<TraceResponseData>(responseData, "TrcRsp");
        }

        public async Task<DataList<TraceFinancialResponseData>> GetTraceResponseFinancialsForApplication(string applEnfSrvCd, string applCtrlCd)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"Appl_EnfSrv_Cd", applEnfSrvCd },
                    {"Appl_CtrlCd", applCtrlCd }
                };

            var data = await MainDB.GetDataFromStoredProcAsync<TraceFinancialResponseData>("TrcRspFin_SelectForAppl", parameters, FillTraceFinancialResultDataFromReader);

            return new DataList<TraceFinancialResponseData>(data, MainDB.LastError);
        }

        public async Task<DataList<TraceFinancialResponseData>> GetActiveTraceResponseFinancialsForApplication(string applEnfSrvCd, string applCtrlCd)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"EnfSrv_Cd", applEnfSrvCd },
                    {"CtrlCd", applCtrlCd }
                };

            var data = await MainDB.GetDataFromStoredProcAsync<TraceFinancialResponseData>("TrcRspFin_ActiveSelectForAppl", parameters, FillTraceFinancialResultDataFromReader);

            return new DataList<TraceFinancialResponseData>(data, MainDB.LastError);
        }

        public async Task<DataList<TraceFinancialResponseDetailData>> GetTraceResponseFinancialDetails(int traceResponseFinancialId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"TrcRspFin_Id", traceResponseFinancialId}
                };

            var data = await MainDB.GetDataFromStoredProcAsync<TraceFinancialResponseDetailData>("TrcRspFin_Dtl_SelectForTrcRspFin", parameters, FillTraceFinancialDetailResultDataFromReader);

            return new DataList<TraceFinancialResponseDetailData>(data, MainDB.LastError);
        }

        public async Task<DataList<TraceFinancialResponseDetailValueData>> GetTraceResponseFinancialDetailValues(int traceResponseFinancialDetailId)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"TrcRspFin_Dtl_Id", traceResponseFinancialDetailId}
                };

            var data = await MainDB.GetDataFromStoredProcAsync<TraceFinancialResponseDetailValueData>("TrcRspFin_Dtl_Value_SelectForTrcRspFin_Dtl", parameters, FillTraceFinancialDetailValueResultDataFromReader);

            return new DataList<TraceFinancialResponseDetailValueData>(data, MainDB.LastError);
        }

        public async Task<int> CreateTraceFinancialResponse(TraceFinancialResponseData data)
        {
            var parameters = new Dictionary<string, object> {
                    { "Appl_EnfSrv_Cd",  data.Appl_EnfSrv_Cd},
                    { "Appl_CtrlCd",  data.Appl_CtrlCd},
                    { "EnfSrv_Cd",  data.EnfSrv_Cd},
                    { "TrcRsp_Rcpt_Dte",  data.TrcRsp_Rcpt_Dte},
                    { "TrcRsp_SeqNr",  data.TrcRsp_SeqNr},
                    { "TrcSt_Cd",  data.TrcSt_Cd},
                    { "TrcRsp_Trace_CyclNr",  data.TrcRsp_Trace_CyclNr},
                    { "ActvSt_Cd",  data.ActvSt_Cd},
                    { "Prcs_RecType",  data.Prcs_RecType},                   
                    { "SIN",  data.Sin},
                    { "SIN_XRef",  data.SinXref}
                };

            if (data.TrcRsp_RcptViewed_Dte is not null)
                parameters.Add("TrcRsp_RcptViewed_Dte", data.TrcRsp_RcptViewed_Dte);

            return await MainDB.GetDataFromStoredProcViaReturnParameterAsync<int>("TrcRspFin_Insert", parameters, "TrcRspFin_Id");            
        }

        public async Task<int> CreateTraceFinancialResponseDetail(TraceFinancialResponseDetailData data)
        {
            var parameters = new Dictionary<string, object> {
                    { "TrcRspFin_Id",  data.TrcRspFin_Id},
                    { "FiscalYear",  data.FiscalYear},
                    { "TaxForm",  data.TaxForm}
                };

            return await MainDB.GetDataFromStoredProcViaReturnParameterAsync<int>("TrcRspFin_Dtl_Insert", parameters, "TrcRspFin_Dtl_Id");
        }

        public async Task<int> CreateTraceFinancialResponseDetailValue(TraceFinancialResponseDetailValueData data)
        {
            var parameters = new Dictionary<string, object> {
                    { "TrcRspFin_Dtl_Id",  data.TrcRspFin_Dtl_Id},
                    { "FieldName",  data.FieldName},
                    { "FieldValue",  data.FieldValue}
                };

            return await MainDB.GetDataFromStoredProcViaReturnParameterAsync<int>("TrcRspFin_Dtl_Value_Insert", parameters, "TrcRspFin_Dtl_Value_Id");
        }

        public async Task UpdateTraceResponseFinancial(TraceFinancialResponseData data)
        {
            var parameters = new Dictionary<string, object> {
                    { "TrcRspFin_Id",  data.TrcRspFin_Id},
                    { "Appl_EnfSrv_Cd",  data.Appl_EnfSrv_Cd},
                    { "Appl_CtrlCd",  data.Appl_CtrlCd},
                    { "EnfSrv_Cd",  data.EnfSrv_Cd},
                    { "TrcRsp_Rcpt_Dte",  data.TrcRsp_Rcpt_Dte},
                    { "TrcRsp_SeqNr",  data.TrcRsp_SeqNr},
                    { "TrcSt_Cd",  data.TrcSt_Cd},
                    { "TrcRsp_Trace_CyclNr",  data.TrcRsp_Trace_CyclNr},
                    { "ActvSt_Cd",  data.ActvSt_Cd},
                    { "Prcs_RecType",  data.Prcs_RecType},
                    { "SIN",  data.Sin},
                    { "SIN_XRef",  data.SinXref}
                };

            if (data.TrcRsp_RcptViewed_Dte is not null)
                parameters.Add("TrcRsp_RcptViewed_Dte", data.TrcRsp_RcptViewed_Dte);

            await MainDB.ExecProcAsync("TrcRspFin_Update", parameters);
        }

        public async Task MarkResponsesAsViewedAsync(string enfService)
        {
            var parameters = new Dictionary<string, object>
            {
                {"chrRecptCd", enfService }
            };

            await MainDB.ExecProcAsync("TrcAPPTraceUpdate", parameters);
        }

        public async Task DeleteCancelledApplicationTraceResponseDataAsync(string applEnfSrvCd, string applCtrlCd, string enfSrvCd)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Appl_EnfSrv_Cd", applEnfSrvCd },
                { "Appl_CtrlCd", applCtrlCd },
                { "enfSrvCode", enfSrvCd[..2] }
            };

            await MainDB.ExecProcAsync("DeleteCanceledApplTrcRspData", parameters);
        }

        private void FillTraceResultDataFromReader(IDBHelperReader rdr, TraceResponseData data)
        {
            data.Appl_EnfSrv_Cd = rdr["Appl_EnfSrv_Cd"] as string;
            data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string;
            data.EnfSrv_Cd = rdr["EnfSrv_Cd"] as string;
            data.TrcRsp_Rcpt_Dte = (DateTime)rdr["TrcRsp_Rcpt_Dte"];
            data.TrcRsp_SeqNr = (int)rdr["TrcRsp_SeqNr"];
            data.TrcRsp_EmplNme = rdr["TrcRsp_EmplNme"] as string; // can be null 
            data.TrcRsp_EmplNme1 = rdr["TrcRsp_EmplNme1"] as string; // can be null 
            data.TrcSt_Cd = rdr["TrcSt_Cd"] as string;
            data.TrcRsp_Addr_Ln = rdr["TrcRsp_Addr_Ln"] as string; // can be null 
            data.TrcRsp_Addr_Ln1 = rdr["TrcRsp_Addr_Ln1"] as string; // can be null 
            data.TrcRsp_Addr_CityNme = rdr["TrcRsp_Addr_CityNme"] as string; // can be null 
            data.TrcRsp_Addr_PrvCd = rdr["TrcRsp_Addr_PrvCd"] as string; // can be null 
            data.TrcRsp_Addr_CtryCd = rdr["TrcRsp_Addr_CtryCd"] as string; // can be null 
            data.TrcRsp_Addr_PCd = rdr["TrcRsp_Addr_PCd"] as string; // can be null 
            data.TrcRsp_Addr_LstUpdte = rdr["TrcRsp_Addr_LstUpdte"] as DateTime?; // can be null 
            data.AddrTyp_Cd = rdr["AddrTyp_Cd"] as string; // can be null 
            data.TrcRsp_SubmViewed_Ind = (byte)rdr["TrcRsp_SubmViewed_Ind"];
            data.TrcRsp_RcptViewed_Ind = (byte)rdr["TrcRsp_RcptViewed_Ind"];
            data.TrcRsp_SubmAddrUsed_Ind = (byte)rdr["TrcRsp_SubmAddrUsed_Ind"];
            data.TrcRsp_SubmAddrHasValue_Ind = (byte)rdr["TrcRsp_SubmAddrHasValue_Ind"];
            data.TrcRsp_Trace_CyclNr = (short)rdr["TrcRsp_Trace_CyclNr"];
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
            data.Prcs_RecType = rdr["Prcs_RecType"] as int?; // can be null 
            data.TrcRsp_RcptViewed_Dte = rdr["TrcRsp_RcptViewed_Dte"] as DateTime?; // can be null 

            // extra values
            data.Subm_SubmCd = rdr["Subm_SubmCd"] as string;
            data.Address = rdr["Address"] as string;
            data.Originator = rdr["originator"] as string;
        }

        private void FillTraceFinancialResultDataFromReader(IDBHelperReader rdr, TraceFinancialResponseData data)
        {
            data.TrcRspFin_Id = (int)rdr["TrcRspFin_Id"];
            data.Appl_EnfSrv_Cd = rdr["Appl_EnfSrv_Cd"] as string;
            data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string;
            data.EnfSrv_Cd = rdr["EnfSrv_Cd"] as string;
            data.TrcRsp_Rcpt_Dte = (DateTime)rdr["TrcRsp_Rcpt_Dte"];
            data.TrcRsp_SeqNr = (int)rdr["TrcRsp_SeqNr"];
            data.TrcSt_Cd = rdr["TrcSt_Cd"] as string;
            data.TrcRsp_Trace_CyclNr = (short)rdr["TrcRsp_Trace_CyclNr"];
            data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
            data.Prcs_RecType = rdr["Prcs_RecType"] as int?; // can be null 
            data.TrcRsp_RcptViewed_Dte = rdr["TrcRsp_RcptViewed_Dte"] as DateTime?; // can be null 
            data.Sin = rdr["SIN"] as string;
            data.SinXref = rdr["SIN_XRef"] as string; // can be null 
        }

        private void FillTraceFinancialDetailResultDataFromReader(IDBHelperReader rdr, TraceFinancialResponseDetailData data)
        {
            data.TrcRspFin_Dtl_Id = (int)rdr["TrcRspFin_Dtl_Id"];
            data.TrcRspFin_Id = (int)rdr["TrcRspFin_Id"];
            data.FiscalYear = (short) rdr["FiscalYear"];
            data.TaxForm = rdr["TaxForm"] as string;
        }

        private void FillTraceFinancialDetailValueResultDataFromReader(IDBHelperReader rdr, TraceFinancialResponseDetailValueData data)
        {
            data.TrcRspFin_Dtl_Value_Id = (int)rdr["TrcRspFin_Dtl_Value_Id"];
            data.TrcRspFin_Dtl_Id = (int)rdr["TrcRspFin_Dtl_Id"];
            data.FieldName = rdr["FieldName"] as string;
            data.FieldValue = rdr["FieldValue"] as string;
        }

    }
}
