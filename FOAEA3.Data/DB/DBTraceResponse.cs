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

        public async Task InsertBulkDataAsync(List<TraceResponseData> responseData)
        {
            await MainDB.BulkUpdateAsync<TraceResponseData>(responseData, "TrcRsp");
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
                { "enfSrvCode", enfSrvCd.Substring(0,2) }
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

    }
}
