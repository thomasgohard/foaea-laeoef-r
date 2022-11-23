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
    internal class DBSummFAFR : DBbase, ISummFAFRRepository
    {
        public DBSummFAFR(IDBToolsAsync mainDB) : base(mainDB)
        {

        }

        public async Task<DataList<SummFAFR_Data>> GetSummFaFrAsync(int summFAFR_Id)
        {
            var parameters = new Dictionary<string, object>() {
                { "CtrlSummFaFrId", summFAFR_Id }
            };

            var data = await MainDB.GetDataFromStoredProcAsync<SummFAFR_Data>("", parameters, FillDataFromReader);

            return new DataList<SummFAFR_Data>(data, MainDB.LastError);

        }

        public async Task<DataList<SummFAFR_Data>> GetSummFaFrListAsync(List<SummFAFR_DE_Data> summFAFRs)
        {
            var firstFAFR = summFAFRs[0];

            var parameters = new Dictionary<string, object>() {
                { "chrEnfSrv_Src_Cd", firstFAFR.EnfSrv_Src_Cd },
                { "chrEnfSrv_Loc_Cd", firstFAFR.EnfSrv_Loc_Cd },
                { "dtmSummFAFR_FA_Payable_Dte", firstFAFR.SummFAFR_FA_Payable_Dte },
                { "chrSummFAFR_FA_Pym_Id", firstFAFR.SummFAFR_FA_Pym_Id }
            };

            if (!string.IsNullOrEmpty(firstFAFR.EnfSrv_SubLoc_Cd))
                parameters.Add("chrEnfSrv_SubLoc_Cd", firstFAFR.EnfSrv_SubLoc_Cd);

            if (!string.IsNullOrEmpty(firstFAFR.Batch_Id))
                parameters.Add("CtrlFAFRBatchId", firstFAFR.Batch_Id);

            var data = await MainDB.GetDataFromStoredProcAsync<SummFAFR_Data>("GetSummFaFrForDivertFunds", parameters, FillDataFromReader);

            return new DataList<SummFAFR_Data>(data, MainDB.LastError);
        }

        private void FillDataFromReader(IDBHelperReader rdr, SummFAFR_Data data)
        {
            data.SummFAFR_Id = (int)rdr["SummFAFR_Id"];
            data.Appl_EnfSrv_Cd = rdr["Appl_EnfSrv_Cd"] as string; // can be null 
            data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string; // can be null 
            data.Batch_Id = rdr["Batch_Id"] as string;
            data.SummFAFR_OrigFA_Ind = (byte)rdr["SummFAFR_OrigFA_Ind"];
            data.SummFAFRLiSt_Cd = (short)rdr["SummFAFRLiSt_Cd"];
            data.SummFAFR_Reas_Cd = rdr["SummFAFR_Reas_Cd"] as int?; // can be null 
            data.EnfSrv_Src_Cd = rdr["EnfSrv_Src_Cd"] as string;
            data.EnfSrv_Loc_Cd = rdr["EnfSrv_Loc_Cd"] as string; // can be null 
            data.EnfSrv_SubLoc_Cd = rdr["EnfSrv_SubLoc_Cd"] as string; // can be null 
            data.SummFAFR_FA_Pym_Id = rdr["SummFAFR_FA_Pym_Id"] as string; // can be null 
            data.SummFAFR_MultiSumm_Ind = rdr["SummFAFR_MultiSumm_Ind"] as byte?; // can be null 
            data.SummFAFR_Post_Dte = (DateTime)rdr["SummFAFR_Post_Dte"];
            data.SummFAFR_FA_Payable_Dte = rdr["SummFAFR_FA_Payable_Dte"] as DateTime?; // can be null 
            data.SummFAFR_AvailDbtrAmt_Money = rdr["SummFAFR_AvailDbtrAmt_Money"] as decimal?; // can be null 
            data.SummFAFR_AvailAmt_Money = rdr["SummFAFR_AvailAmt_Money"] as decimal?; // can be null 
            data.SummFAFR_Comments = rdr["SummFAFR_Comments"] as string; // can be null 
        }
    }
}
