using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Base;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBSummFAFR_DE : DBbase, ISummFAFR_DERepository
    {
        public DBSummFAFR_DE(IDBToolsAsync mainDB) : base(mainDB)
        {

        }

        public async Task<SummFAFR_DE_Data> GetSummFaFrDeAsync(int summFAFR_Id)
        {
            var parameters = new Dictionary<string, object>() {
                { "CtrlSummFaFrId", summFAFR_Id }
            };

            var data = await MainDB.GetDataFromStoredProcAsync<SummFAFR_DE_Data>("GetSummFaFrDEForDivertFunds", parameters, FillDataFromReader);

            return data.FirstOrDefault();
        }

        public async Task<DataList<SummFAFR_DE_Data>> GetSummFaFrDeReadyBatchesAsync(string enfSrv_Src_Cd, string DAFABatchId)
        {
            var parameters = new Dictionary<string, object>() {
                { "chrEnfSrv_Src_Cd", enfSrv_Src_Cd },
                { "chrReadyBatchId", DAFABatchId }
            };

            var data = await MainDB.GetDataFromStoredProcAsync<SummFAFR_DE_Data>("SummFaFrDeGetReadyBatches", parameters, FillDataFromReader);

            return new DataList<SummFAFR_DE_Data>(data, MainDB.LastError);

        }

        public async Task<int> CountDuplicateFundsAvailable(string debtorId, string enfsrv_src_cd, string enfsrv_loc_cd,
                                                            string paymentId, DateTime payableDate, int summFaFrId = 0)
        {
            var parameters = new Dictionary<string, object>
            {
                { "chrDebtor_Id", debtorId },
                { "chrEnfSrv_Src_Cd", enfsrv_src_cd },
                { "chrEnfSrv_Loc_Cd", enfsrv_loc_cd },
                { "chrEnfSrv_SubLoc_Cd", null},
                { "chrFAPym_Id", paymentId},
                { "dtmPayableDate", payableDate }
            };

            if (summFaFrId != 0)
                parameters.Add("intSummFAFR_ID", summFaFrId);

            return await MainDB.GetDataFromProcSingleValueAsync<int>("CountDuplicateFA", parameters);
        }

        public async Task<int> CountDuplicateFundsReversal(string debtorId, string enfsrv_src_cd, string enfsrv_loc_cd,
                                                           string paymentId, DateTime payableDate, int summFaFrId = 0)
        {
            var parameters = new Dictionary<string, object>
            {
                { "chrDebtor_Id", debtorId },
                { "chrEnfSrv_Src_Cd", enfsrv_src_cd },
                { "chrEnfSrv_Loc_Cd", enfsrv_loc_cd },
                { "chrEnfSrv_SubLoc_Cd", null},
                { "chrFAPym_Id", paymentId},
                { "dtmPayableDate", payableDate }
            };
                        
            if (summFaFrId != 0)
                parameters.Add("intSummFAFR_ID", summFaFrId);

            return await MainDB.GetDataFromProcSingleValueAsync<int>("CountDuplicateFR", parameters);
        }

        public async Task<SummFAFR_DE_Data> CreateFaFrDeAsync(SummFAFR_DE_Data data)
        {
            var parameters = new Dictionary<string, object> {
                {"Appl_EnfSrv_Cd", data.Appl_EnfSrv_Cd },
                {"Appl_CtrlCd", data.Appl_CtrlCd },
                {"Batch_Id", data.Batch_Id },
                {"SummFAFR_OrigFA_Ind", data.SummFAFR_OrigFA_Ind },
                {"SummFAFRLiSt_Cd", data.SummFAFRLiSt_Cd },
                {"SummFAFR_Reas_Cd", data.SummFAFR_Reas_Cd },
                {"EnfSrv_Src_Cd", data.EnfSrv_Src_Cd },
                {"EnfSrv_Loc_Cd", data.EnfSrv_Loc_Cd },
                {"EnfSrv_SubLoc_Cd", data.EnfSrv_SubLoc_Cd },
                {"SummFAFR_FA_Pym_Id", data.SummFAFR_FA_Pym_Id },
                {"SummFAFR_MultiSumm_Ind", data.SummFAFR_MultiSumm_Ind },
                {"SummFAFR_Post_Dte", data.SummFAFR_Post_Dte },
                {"SummFAFR_FA_Payable_Dte", data.SummFAFR_FA_Payable_Dte },
                {"SummFAFR_AvailDbtrAmt_Money", data.SummFAFR_AvailDbtrAmt_Money },
                {"SummFAFR_AvailAmt_Money", data.SummFAFR_AvailAmt_Money },
                {"SummFAFR_Comments", data.SummFAFR_Comments },
                {"Dbtr_Id", data.Dbtr_Id }
            };

            var summFaFrData = await MainDB.GetDataFromStoredProcAsync<SummFAFR_DE_Data>("SummFaFrDeInsert", parameters, FillDataFromReader);

            return summFaFrData.FirstOrDefault();
        }

        private void FillDataFromReader(IDBHelperReader rdr, SummFAFR_DE_Data data)
        {
            data.SummFAFR_Id = (int)rdr["SummFAFR_Id"];
            data.Appl_EnfSrv_Cd = rdr["Appl_EnfSrv_Cd"] as string; // can be null 
            data.Appl_CtrlCd = rdr["Appl_CtrlCd"] as string; // can be null 
            data.Batch_Id = rdr["Batch_Id"] as string; // can be null
            data.EnfSrv_Src_Cd = rdr["EnfSrv_Src_Cd"] as string; // can be null 
            data.SummFAFR_FA_Payable_Dte = rdr["SummFAFR_FA_Payable_Dte"] as DateTime?; // can be null 
            data.SummFAFR_AvailDbtrAmt_Money = rdr["SummFAFR_AvailDbtrAmt_Money"] as decimal?; // can be null 
            data.Dbtr_Id = rdr["Dbtr_Id"] as string; // can be null 

            if (rdr.ColumnExists("SummFAFR_OrigFA_Ind")) data.SummFAFR_OrigFA_Ind = rdr["SummFAFR_OrigFA_Ind"] as byte?; // can be null 
            if (rdr.ColumnExists("SummFAFRLiSt_Cd")) data.SummFAFRLiSt_Cd = rdr["SummFAFRLiSt_Cd"] as short?; // can be null 
            if (rdr.ColumnExists("SummFAFR_Reas_Cd")) data.SummFAFR_Reas_Cd = rdr["SummFAFR_Reas_Cd"] as int?; // can be null 
            if (rdr.ColumnExists("EnfSrv_Loc_Cd")) data.EnfSrv_Loc_Cd = rdr["EnfSrv_Loc_Cd"] as string; // can be null 
            if (rdr.ColumnExists("EnfSrv_SubLoc_Cd")) data.EnfSrv_SubLoc_Cd = rdr["EnfSrv_SubLoc_Cd"] as string; // can be null 
            if (rdr.ColumnExists("SummFAFR_FA_Pym_Id")) data.SummFAFR_FA_Pym_Id = rdr["SummFAFR_FA_Pym_Id"] as string; // can be null 
            if (rdr.ColumnExists("SummFAFR_MultiSumm_Ind")) data.SummFAFR_MultiSumm_Ind = rdr["SummFAFR_MultiSumm_Ind"] as byte?; // can be null 
            if (rdr.ColumnExists("SummFAFR_Post_Dte")) data.SummFAFR_Post_Dte = rdr["SummFAFR_Post_Dte"] as DateTime?; // can be null 
            if (rdr.ColumnExists("SummFAFR_AvailAmt_Money")) data.SummFAFR_AvailAmt_Money = rdr["SummFAFR_AvailAmt_Money"] as decimal?; // can be null 
            if (rdr.ColumnExists("SummFAFR_Comments")) data.SummFAFR_Comments = rdr["SummFAFR_Comments"] as string; // can be null 

            if (rdr.ColumnExists("SummFAFR_OrigFA_IndDesc"))
            {
                string origDesc = rdr["SummFAFR_OrigFA_IndDesc"] as string;
                if (origDesc == "FA")
                    data.SummFAFR_OrigFA_Ind = 1;
                else
                    data.SummFAFR_OrigFA_Ind = 0;
            }
        }

    }
}
