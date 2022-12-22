using DBHelper;
using FileBroker.Common.Helpers;

namespace FileBroker.Business
{
    public class IncomingFederalTrainingManager
    {
        private APIBrokerList APIs { get; }
        private RepositoryList DB { get; }

        private FoaeaSystemAccess FoaeaAccess { get; }

        public IncomingFederalTrainingManager(APIBrokerList apis, RepositoryList repositories,
                                              IFileBrokerConfigurationHelper config)
        {
            APIs = apis;
            DB = repositories;

            FoaeaAccess = new FoaeaSystemAccess(apis, config.FoaeaLogin);
        }

        public async Task<List<string>> ProcessIncomingTraining(string flatFileContent, string flatFileName)
        {
            var fileTableData = await GetFileTableDataAsync(flatFileName);

            var trainingFileData = new FedInterceptionTrainingBase();

            var errors = new List<string>();

            if (!fileTableData.Active.HasValue || !fileTableData.Active.Value)
            {
                errors.Add($"[{fileTableData.Name}] is not active.");
                return errors;
            }

            await DB.FileTable.SetIsFileLoadingValueAsync(fileTableData.PrcId, true);

            string fileCycle = Path.GetExtension(flatFileName)[1..];
            try
            {
                var fileLoader = new IncomingFederalTrainingFileLoader(DB.FlatFileSpecs, fileTableData.PrcId);
                await fileLoader.FillFederalTrainingFileDataFromFlatFileAsync(trainingFileData, flatFileContent, errors);

                if (errors.Any())
                    return errors;

                string subCategory = fileTableData.Category[6..]; 

                ValidateHeader(trainingFileData.TRIN01, flatFileName, ref errors);
                ValidateDetails(trainingFileData.TRIN02, subCategory, ref errors);
                ValidateFooter(trainingFileData.TRIN99, trainingFileData.TRIN02, ref errors);

                if (errors.Any())
                    return errors;

                await FoaeaAccess.SystemLoginAsync();
                try
                {
                    string batchName;
                    if (string.IsNullOrEmpty(trainingFileData.TRIN01.Batch_Identifier))
                        batchName = trainingFileData.TRIN01.Batch_Identifier;
                    else
                        batchName = trainingFileData.TRIN01.Cycle.ToString();

                    if (!errors.Any())
                    {
                        var incomingTrainingTable = BuildIncomingTrainingData(batchName, trainingFileData.TRIN02,
                                                                              out int recordCountFAFR,
                                                                              out decimal totalAmountFAFR);

                        var controlBatchData = await CreateControlBatch(batchName, recordCountFAFR, totalAmountFAFR);

                        bool success = await CreateTransactions(flatFileName, controlBatchData, trainingFileData.TRIN02);

                        if (success)
                        {
                            if (recordCountFAFR > 0)
                                await APIs.ControlBatches.MarkBatchAsLoaded(controlBatchData);

                            await DB.FundsAvailableIncomingTable.UpdateFundsAvailableIncomingTraining(incomingTrainingTable);

                            await DB.FileTable.SetNextCycleForFileTypeAsync(fileTableData, fileCycle.Length);
                        }

                    }
                }
                finally
                {
                    await FoaeaAccess.SystemLogoutAsync();
                }

            }
            catch (Exception e)
            {
                errors.Add("An error occurred: " + e.Message);
            }
            finally
            {
                await DB.FileTable.SetIsFileLoadingValueAsync(fileTableData.PrcId, false);
            }

            return errors;

        }

        private async Task<FileTableData> GetFileTableDataAsync(string flatFileName)
        {
            string fileNameNoCycle = Path.GetFileNameWithoutExtension(flatFileName);

            return await DB.FileTable.GetFileTableDataForFileNameAsync(fileNameNoCycle);
        }

        private static void ValidateHeader(FedInterceptionTraining_RecType01 dataFromFile, string flatFileName, ref List<string> errors)
        {
            int cycle = FileHelper.GetCycleFromFilename(flatFileName);
            if (dataFromFile.Cycle != cycle)
            {
                errors.Add($"Cycle in file [{dataFromFile.Cycle}] does not match cycle of file [{cycle}]");
            }
        }

        private static void ValidateDetails(List<FedInterceptionTraining_RecType02> details, string subCategory,
                                            ref List<string> errors)
        {
            foreach (var detail in details)
            {
                if (string.IsNullOrEmpty(detail.TransactionType))
                    errors.Add($"Item: TransactionType missing from table FAFRFT{subCategory} 02");
                else if (detail.TransactionType.NotIn(TransactionType.FundAvailable, TransactionType.FundReversal, TransactionType.FundTransferred))
                    errors.Add($"Invalid Transaction Type ({detail.TransactionType}). Debtor ID :{detail.DebtorId}");

                if (string.IsNullOrEmpty(detail.DebtorId))
                    errors.Add($"Item: DebtorId missing from table FAFRFT{subCategory} 02");

                if (string.IsNullOrEmpty(detail.PaymentIdentifier))
                    errors.Add($"Item: PaymentIdentifier missing from table FAFRFT{subCategory} 02");

                if (detail.PayableDate == DateTime.MinValue)
                    errors.Add($"Item: PayableDate missing from table FAFRFT{subCategory} 02");

                if (detail.AvailableOrTransferredAmt is null)
                    errors.Add($"Item: AvailableOrTransferredAmt missing from table FAFRFT{subCategory} 02");

                if ((subCategory == "TRA") && string.IsNullOrEmpty(detail.XRefSin))
                    errors.Add($"Item: XRefSin missing from table FAFRFT{subCategory} 02");
            }
        }

        private static void ValidateFooter(FedInterceptionTraining_RecType99 dataFromFile, List<FedInterceptionTraining_RecType02> details,
                                           ref List<string> errors)
        {
            long totalAmount = 0;
            long hashSum = 0;
            foreach (var detail in details)
            {
                totalAmount += detail.AvailableOrTransferredAmt ?? 0;
                hashSum += long.Parse(detail.PaymentIdentifier.Substring(7, 9));
            }

            if (totalAmount != dataFromFile.AmountTotal)
                errors.Add($"FAFRTotalAmount and FTTotalAmount: {totalAmount} does not match trailer record total: {dataFromFile.AmountTotal}");

            if (dataFromFile.ResponseCnt != details.Count)
                errors.Add($"FAFRRecordCount and FTRecordCount: {details.Count} does not match trailer record total: {dataFromFile.ResponseCnt}");

            if ((hashSum != dataFromFile.SINHashTotal) && (dataFromFile.SINHashTotal != 0))
            {
                long rightMost9 = long.Parse(hashSum.ToString().Substring(hashSum.ToString().Length - 9));
                if (rightMost9 != dataFromFile.SINHashTotal)
                    errors.Add($"Hash Sum Invalid: {dataFromFile.SINHashTotal}");
            }
        }

        private static List<FundsAvailableIncomingTrainingData> BuildIncomingTrainingData(string batchName,
                                                                        List<FedInterceptionTraining_RecType02> details,
                                                                        out int recordCountFAFR, out decimal totalAmountFAFR)
        {
            var incomingTrainingTable = new List<FundsAvailableIncomingTrainingData>();

            totalAmountFAFR = 0;
            recordCountFAFR = 0;

            decimal totalAmountFT = 0;
            int recordCountFT = 0;

            for (int i = 0; i == details.Count; i++)
            {
                var thisData = details[i];
                details[i] = new FedInterceptionTraining_RecType02
                {
                    RecType = thisData.RecType,
                    TransactionType = thisData.TransactionType,
                    DebtorId = thisData.DebtorId,
                    PaymentIdentifier = thisData.PaymentIdentifier,
                    PayableDate = thisData.PayableDate,
                    AvailableOrTransferredAmt = thisData.AvailableOrTransferredAmt,
                    EnfSrv_Loc_Cd = thisData.EnfSrv_Loc_Cd,
                    EnfSrv_SubLoc_Cd = null,
                    Lctr_Cd = thisData.Lctr_Cd,
                    XRefSin = thisData.XRefSin
                };

                incomingTrainingTable.Add(new FundsAvailableIncomingTrainingData
                {
                    BatchId = batchName,
                    Ordinal = i,
                    Payment_Id = details[i].PaymentIdentifier,
                    XRefSin = details[i].XRefSin,
                    Process_State = "A"
                });

                if (details[i].TransactionType.In(TransactionType.FundAvailable, TransactionType.FundReversal))
                {
                    recordCountFAFR++;
                    totalAmountFAFR += details[i].AvailableOrTransferredAmt ?? 0;
                }
                else // FundTransferred
                {
                    recordCountFT++;
                    totalAmountFT += details[i].AvailableOrTransferredAmt ?? 0;
                }
            }

            return incomingTrainingTable;
        }

        private async Task<ControlBatchData> CreateControlBatch(string batchName, int recordCountFAFR, decimal totalAmountFAFR)
        {
            var batchData = new ControlBatchData
            {
                Batch_Id = null,
                EnfSrv_Src_Cd = "TR00",
                DataEntryBatch_Id = batchName,
                BatchType_Cd = "FA",
                Batch_Post_Dte = DateTime.Now,
                Batch_Compl_Dte = null,
                Medium_Cd = "FTP",
                SourceRecCnt = recordCountFAFR,
                DoJRecCnt = recordCountFAFR,
                SourceTtlAmt_Money = totalAmountFAFR / 100M,
                DoJTtlAmt_Money = totalAmountFAFR / 100M,
                BatchLiSt_Cd = 1,
                Batch_Reas_Cd = null,
                Batch_Pend_Ind = 0,
                PendTtlAmt_Money = null
            };

            return await APIs.ControlBatches.CreateControlBatch(batchData);
        }

        private async Task<bool> CreateTransactions(string flatFileName, ControlBatchData controlBatchData,
                                                    List<FedInterceptionTraining_RecType02> details)
        {
            try
            {
                foreach (var detail in details.Where(m => m.TransactionType.In(TransactionType.FundAvailable, TransactionType.FundReversal))
                                              .OrderByDescending(m => m.TransactionType))
                {
                    var newTransaction = new SummFAFR_DE_Data
                    {
                        Appl_EnfSrv_Cd = string.Empty,
                        Appl_CtrlCd = string.Empty,
                        Batch_Id = controlBatchData.Batch_Id,
                        EnfSrv_Src_Cd = "TR00",
                        EnfSrv_Loc_Cd = detail.EnfSrv_Loc_Cd,
                        EnfSrv_SubLoc_Cd = detail.EnfSrv_SubLoc_Cd,
                        SummFAFR_FA_Pym_Id = detail.PaymentIdentifier,
                        SummFAFR_Post_Dte = DateTime.Now,
                        SummFAFR_FA_Payable_Dte = detail.PayableDate,
                        SummFAFR_AvailAmt_Money = detail.AvailableOrTransferredAmt,
                        Dbtr_Id = detail.DebtorId.Length > 7 ? detail.DebtorId[0..7] : detail.DebtorId,
                    };

                    await APIs.Transactions.InsertFaFrDe(detail.TransactionType, newTransaction);
                }

                // TODO: insert FTs?

            }
            catch (Exception e)
            {
                await DB.ErrorTrackingTable.MessageBrokerErrorAsync($"Error Sending Transactions. Batch Cancelled",
                                                                    $"Error processing  {flatFileName}", e, displayExceptionError: true);
                return false;
            }

            return true;
        }

    }
}
