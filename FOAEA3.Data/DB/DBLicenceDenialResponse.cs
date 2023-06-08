using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBLicenceDenialResponse : DBbase, ILicenceDenialResponseRepository
    {
        public DBLicenceDenialResponse(IDBToolsAsync mainDB) : base(mainDB)
        {

        }

        public MessageDataList Messages => throw new NotImplementedException();

        public async Task<LicenceDenialResponseData> GetLastResponseData(string applEnfSrvCd, string applCtrlCd)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"EnfSrv_Cd", applEnfSrvCd },
                    {"CtrlCd", applCtrlCd }
                };

            var data = await MainDB.GetDataFromStoredProcAsync<LicenceDenialResponseData>("GetLastLicenseResponse", parameters, FillLicenceDenialResultDataFromReader);

            return data.FirstOrDefault();
        }

        public async Task InsertBulkData(List<LicenceDenialResponseData> responseData)
        {
            await MainDB.BulkUpdateAsync<LicenceDenialResponseData>(responseData, "LicRsp");
        }

        public async Task MarkResponsesAsViewed(string enfService)
        {
            var parameters = new Dictionary<string, object>
            {
                {"chrRecptCd", enfService }
            };

            await MainDB.ExecProcAsync("LicAPPLicUpdate", parameters);

        }

        private void FillLicenceDenialResultDataFromReader(IDBHelperReader rdr, LicenceDenialResponseData data)
        {
            data.Appl_EnfSrv_Cd = rdr["Appl_EnfSrv_Cd"] as string;
            data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string;
            data.EnfSrv_Cd = rdr["EnfSrv_Cd"] as string;
            data.LicRsp_Rcpt_Dte = (DateTime)rdr["LicRsp_Rcpt_Dte"];
            data.LicRsp_SeqNr = (short)rdr["LicRsp_SeqNr"];
            data.RqstStat_Cd = (short)rdr["RqstStat_Cd"];
            data.LicRsp_Comments = rdr["LicRsp_Comments"] as string; // can be null 
            if (rdr.ColumnExists("LicRspFilename"))
                data.LicRspFilename = rdr["LicRspFilename"] as string; // can be null 
            if (rdr.ColumnExists("LicRspType"))
                data.LicRspType = rdr["LicRspType"] as string; // can be null 
            if (rdr.ColumnExists("LicRspSource_RefNo"))
                data.LicRspSource_RefNo = rdr["LicRspSource_RefNo"] as string; // can be null 
            if (rdr.ColumnExists("LicRsp_RcptViewed_Ind"))
                data.LicRsp_RcptViewed_Ind = (bool)rdr["LicRsp_RcptViewed_Ind"];
            if (rdr.ColumnExists("LicRsp_RcptViewed_Date"))
                data.LicRsp_RcptViewed_Date = rdr["LicRsp_RcptViewed_Date"] as DateTime?; // can be null 
        }
    }
}
