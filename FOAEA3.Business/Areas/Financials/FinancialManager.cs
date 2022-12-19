using FOAEA3.Model;
using FOAEA3.Model.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Business.Areas.Financials
{
    public class FinancialManager
    {
        private IRepositories DB { get; }
        private IRepositories_Finance DBfinance { get; }

        public FinancialManager(IRepositories repositories, IRepositories_Finance repositoriesFinance)
        {
            DB = repositories;
            DBfinance = repositoriesFinance;
        }

        public async Task<List<CR_PADReventData>> GetActiveCR_PADReventsAsync(string enfSrv)
        {
            return await DBfinance.FinancialRepository.GetActiveCR_PADReventsAsync(enfSrv);
        }

        public async Task CloseCR_PADReventsAsync(string batchId, string enfSrv)
        {
            await DBfinance.FinancialRepository.CloseCR_PADReventsAsync(batchId, enfSrv);
        }

        public async Task<List<BlockFundData>> GetBlockFundsData(string enfSrv)
        {
            return await DBfinance.FinancialRepository.GetBlockFundsData(enfSrv);
        }

        public async Task<List<DivertFundData>> GetDivertFundsData(string enfSrv, string batchId)
        {
            return await DBfinance.FinancialRepository.GetDivertFundsData(enfSrv, batchId);
        }

        public async Task<List<BatchSimpleData>> GetReadyDivertFundsBatches(string enfSrv_Cd, string enfSrv_Loc_Cd)
        {
            return await DBfinance.ControlBatchRepository.GetReadyDivertFundsBatches(enfSrv_Cd, enfSrv_Loc_Cd);
        }

        public async Task<List<IFMSdata>> GetIFMSdataAsync(string batchId)
        {
            return await DBfinance.FinancialRepository.GetIFMSdataAsync(batchId);
        }

        public async Task CloseControlBatchAsync(string batchId)
        {
            await DBfinance.FinancialRepository.CloseControlBatchAsync(batchId);
        }

        public async Task<DateTime> GetDateLastUIBatchLoaded()
        {
            return await DB.InterceptionTable.GetDateLastUIBatchLoaded();
        }

        public async Task<(string, string, string, string)> CreateXFControlBatchAsync(ControlBatchData batchData)
        {
            var controlBatchDB = DBfinance.ControlBatchRepository;

            var values = new ControlBatchData()
            {
                Batch_Id = batchData.Batch_Id,
                EnfSrv_Src_Cd = batchData.EnfSrv_Src_Cd,
                DataEntryBatch_Id = batchData.DataEntryBatch_Id,
                BatchType_Cd = batchData.BatchType_Cd,
                Batch_Post_Dte = batchData.Batch_Post_Dte,
                Batch_Compl_Dte = batchData.Batch_Compl_Dte,
                Medium_Cd = batchData.Medium_Cd,
                SourceRecCnt = batchData.SourceRecCnt,
                DoJRecCnt = batchData.DoJRecCnt,
                SourceTtlAmt_Money = batchData.SourceTtlAmt_Money,
                DoJTtlAmt_Money = batchData.DoJTtlAmt_Money,
                BatchLiSt_Cd = batchData.BatchLiSt_Cd,
                Batch_Reas_Cd = batchData.Batch_Reas_Cd,
                Batch_Pend_Ind = batchData.Batch_Pend_Ind,
                PendTtlAmt_Money = batchData.PendTtlAmt_Money,
                FeesTtlAmt_Money = batchData.FeesTtlAmt_Money
            };

            return await controlBatchDB.CreateXFControlBatchAsync(values);
        }

        public async Task<ControlBatchData> CreateControlBatchAsync(ControlBatchData controlBatchData)
        {
            string batchID;
            string reasonCode;

            (_, batchID, reasonCode, _) = await CreateXFControlBatchAsync(controlBatchData);

            controlBatchData.Batch_Id = batchID;

            if (int.TryParse(reasonCode, out int code))
                controlBatchData.Batch_Reas_Cd = code;

            return controlBatchData;
        }

        public async Task<ControlBatchData> GetControlBatch(string batchId)
        {
            var controlBatchDB = DBfinance.ControlBatchRepository;
            return await controlBatchDB.GetControlBatchAsync(batchId);
        }
    }
}
