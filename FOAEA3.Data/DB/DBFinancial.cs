using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBFinancial : DBbase, IFinancialRepository
    {
        public DBFinancial(IDBToolsAsync mainDB) : base(mainDB)
        {
        }

        public async Task<List<CR_PADReventData>> GetActiveCR_PADReventsAsync(string enfSrv)
        {
            var parameters = new Dictionary<string, object>
                {
                    { "enfSvrCode",  enfSrv}
                };

            return await MainDB.GetDataFromStoredProcAsync<CR_PADReventData>("MessageBrokerFinancialPADRBatchExists", parameters, FillCR_PADReventsFromReader);
        }

        public async Task CloseCR_PADReventsAsync(string batchId, string enfSrv)
        {
            var parameters = new Dictionary<string, object>
                {
                    { "batchID",  batchId},
                    { "enfSvrCode",  enfSrv}
                };

            await MainDB.ExecProcAsync("CtrlBatchUpdateFTPCtrlBatch", parameters);
        }

        public async Task<List<BlockFundData>> GetBlockFundsData(string enfSrv)
        {
            var parameters = new Dictionary<string, object>
                {
                    { "enfSvrCode",  enfSrv}
                };

            return await MainDB.GetDataFromStoredProcAsync<BlockFundData>("MessageBrokerGetFinancialBlockFundsData", parameters, FillBlockFundDataFromReader);
        }

        private void FillBlockFundDataFromReader(IDBHelperReader rdr, BlockFundData data)
        {
            data.Dbtr_Id = rdr["Dbtr_Id"] as string;
            data.Appl_Dbtr_Cnfrmd_SIN = rdr["Appl_Dbtr_Cnfrmd_SIN"] as string;
            data.Start_Dte = (DateTime) rdr["Start_Dte"];
            data.End_Dte = (DateTime) rdr["End_Dte"];
            data.Appl_Dbtr_FrstNme = rdr["Appl_Dbtr_FrstNme"] as string;
            data.Appl_Dbtr_MddleNme = rdr["Appl_Dbtr_MddleNme"] as string;
            data.Appl_Dbtr_SurNme = rdr["Appl_Dbtr_SurNme"] as string;
            data.Appl_Dbtr_LngCd = rdr["Appl_Dbtr_LngCd"] as string;
            
            if (rdr.ColumnExists("Appl_Dbtr_Addr_Ln")) data.Appl_Dbtr_Addr_Ln = rdr["Appl_Dbtr_Addr_Ln"] as string;
            if (rdr.ColumnExists("Appl_Dbtr_Addr_Ln1")) data.Appl_Dbtr_Addr_Ln1 = rdr["Appl_Dbtr_Addr_Ln1"] as string;
            if (rdr.ColumnExists("Appl_Dbtr_Addr_CityNme")) data.Appl_Dbtr_Addr_CityNme = rdr["Appl_Dbtr_Addr_CityNme"] as string;
            if (rdr.ColumnExists("Appl_Dbtr_Addr_PrvCd")) data.Appl_Dbtr_Addr_PrvCd = rdr["Appl_Dbtr_Addr_PrvCd"] as string;
            if (rdr.ColumnExists("Appl_Dbtr_Addr_CtryCd")) data.Appl_Dbtr_Addr_CtryCd = rdr["Appl_Dbtr_Addr_CtryCd"] as string;
            if (rdr.ColumnExists("Appl_Dbtr_Addr_PCd")) data.Appl_Dbtr_Addr_PCd = rdr["Appl_Dbtr_Addr_PCd"] as string;
            
            if (rdr.ColumnExists("Appl_Dbtr_Gendr_Cd")) data.Appl_Dbtr_Gendr_Cd = rdr["Appl_Dbtr_Gendr_Cd"] as string;
            if (rdr.ColumnExists("Appl_Dbtr_Brth_Dte")) data.Appl_Dbtr_Brth_Dte = (DateTime) rdr["Appl_Dbtr_Brth_Dte"];
        }

        public async Task<List<IFMSdata>> GetIFMSdataAsync(string batchId)
        {
            var parameters = new Dictionary<string, object>
                {
                    { "batchID",  batchId}
                };

            return await MainDB.GetDataFromStoredProcAsync<IFMSdata>("MessageBrokerGetIFMSBatchData", parameters, FillIFMSdataFromReader);
        }

        public async Task CloseControlBatchAsync(string batchId)
        {
            var parameters = new Dictionary<string, object>
                {
                    { "batchID",  batchId}
                };

            await MainDB.ExecProcAsync("CtrlBatchUpdateFTPCtrlBatch", parameters);
        }

        private void FillIFMSdataFromReader(IDBHelperReader rdr, IFMSdata data)
        {
            data.IPU_Nr = rdr["IPU_Nr"] as string;
            data.TransferAmt_Money = (decimal)rdr["TransferAmt_Money"];
            data.EnfOff_Fin_VndrCd = rdr["EnfOff_Fin_VndrCd"] as string;
            data.EnfSrv_Cd = rdr["EnfSrv_Cd"] as string;
            data.Court = rdr["Court"] as string;
        }

        private void FillCR_PADReventsFromReader(IDBHelperReader rdr, CR_PADReventData data)
        {
            data.Event_Id = (int)rdr["Event_Id"];
            data.Batch_Id = rdr["Batch_Id"] as string;
            data.Event_TimeStamp = (DateTime)rdr["Event_TimeStamp"];
            if (rdr.ColumnExists("Event_Compl_Dte"))
                data.Event_Compl_Dte = rdr["Event_Compl_Dte"] as DateTime?; // can be null 
            if (rdr.ColumnExists("ActvSt_Cd"))
                data.ActvSt_Cd = rdr["ActvSt_Cd"] as string;
        }


    }
}
