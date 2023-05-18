using FileBroker.Common.Helpers;
using System.Text;

namespace FileBroker.Business
{
    public class OutgoingFinancialDivertFundsManager
    {
        private APIBrokerList APIs { get; }
        private RepositoryList DB { get; }

        private FoaeaSystemAccess FoaeaAccess { get; }

        public OutgoingFinancialDivertFundsManager(APIBrokerList apis, RepositoryList repositories, IFileBrokerConfigurationHelper config)
        {
            APIs = apis;
            DB = repositories;

            FoaeaAccess = new FoaeaSystemAccess(apis, config.FoaeaLogin);
        }

        public async Task<string> CreateBlockFundsFile(string fileBaseName, List<string> errors)
        {
            var fileTableData = await DB.FileTable.GetFileTableDataForFileName(fileBaseName);

            string newCycle = BuildNewCycle(fileBaseName, fileTableData.Cycle);

            string newFilePath = fileTableData.Path.AppendToPath(fileTableData.Name + "." + newCycle, isFileName: true);

            if (File.Exists(newFilePath))
            {
                errors.Add("** Error: File Already Exists");
                return "";
            }

            await FoaeaAccess.SystemLogin();
            try
            {
                var processCodes = await DB.ProcessParameterTable.GetProcessCodesAsync(fileTableData.PrcId);

                var readyDivertFundsBatches = await APIs.ControlBatches.GetReadyDivertFundsBatches(processCodes.EnfSrv_Cd, processCodes.EnfSrv_Loc_Cd);
                if ((readyDivertFundsBatches is null) || !readyDivertFundsBatches.Any())
                {
                    errors.Add("No ready DF batches found!");
                    return "";
                }

                if (readyDivertFundsBatches.Count > 1)
                {
                    errors.Add("Too many DF batches found!");
                    return "";
                }


                var batch = readyDivertFundsBatches.First();
                var divertFundsData = await APIs.Financials.GetDivertFundsAsync(processCodes.EnfSrv_Cd, batch.Batch_Id);

                if ((divertFundsData is null) || (!divertFundsData.Any()))
                {
                    errors.Add("** Error: No Divert Fund data!?");
                    return "";
                }

                string fileContent = await GenerateOutputFileContentFromData(divertFundsData, newCycle, processCodes.EnfSrv_Cd,
                                                                             batch.Batch_Id, batch.DataEntryBatch_Id);
                await File.WriteAllTextAsync(newFilePath, fileContent);

                if (fileTableData.Transform)
                    await TransformFile.Process(fileTableData, newFilePath);

                await DB.FileTable.SetNextCycleForFileType(fileTableData, newCycle.Length);

                string message = fileTableData.Category + $" Outbound {fileBaseName} file created successfully.";
                await DB.OutboundAuditTable.InsertIntoOutboundAuditAsync(fileBaseName + "." + newCycle, DateTime.Now,
                                                                         fileCreated: true, message);

                /*
                    'update the FA Incoming table
                    dataservice.UpdateFATableData(DataEntryBatchID, dataservice.ProcessCategory.Substring(0, 3))

                    'update the control batch
                    Dim finDataBatch As New MidTier.Business.DataEntryControlBatchSystem(dataservice.ConnectionStringFOAEA)
                    finDataBatch.UpdateFTPControlBatch(BatchID)                 
                 */

            }
            catch (Exception e)
            {
                await DB.OutboundAuditTable.InsertIntoOutboundAuditAsync(fileBaseName + "." + newCycle, DateTime.Now,
                                                                         fileCreated: true, e.Message);
            }
            finally
            {
                await FoaeaAccess.SystemLogout();
            }

            return newFilePath;
        }

        private async Task<string> GenerateOutputFileContentFromData(List<DivertFundData> data, string newCycle, string enfSrv,
                                                                     string batchId, string dataEntryBatchId)
        {
            var result = new StringBuilder();

            var fundsAvailableData = await DB.FundsAvailableIncomingTable.GetFundsAvailableIncomingTrainingData(batchId);

            result.AppendLine(GenerateHeaderLine(newCycle, enfSrv, dataEntryBatchId));
            int itemCount = 0;
            decimal totalDiverted = 0;
            long hashSinTotal = 0;
            foreach (var fundsAvailable in fundsAvailableData.OrderBy(m => m.Ordinal ?? 0))
            {
                var item = data.Where(m => m.SummFAFR_FA_Pym_Id == fundsAvailable.Payment_Id).FirstOrDefault();

                if (item is not null)
                {
                    result.AppendLine(GenerateDetailLine(item, fundsAvailable));
                    itemCount++;
                    totalDiverted += item.SummDF_DivertedDbtrAmt_Money ?? 0M;
                    hashSinTotal += long.Parse(item.SummFAFR_FA_Pym_Id);
                }
            }

            string sinHashTotal = hashSinTotal.ToString().PadLeft(9, '0');
            if (sinHashTotal.Length >= 10)
                sinHashTotal = sinHashTotal[..9];
            result.AppendLine(GenerateFooterLine(itemCount, totalDiverted, sinHashTotal));

            return result.ToString();
        }

        private static string GenerateHeaderLine(string newCycle, string enfSrv, string dataEntryBatchId)
        {
            string julianDate = DateTime.Now.AsJulianString();

            return $"01{newCycle}{julianDate}{enfSrv}22{dataEntryBatchId,-10}PROD";
        }

        private static string GenerateDetailLine(DivertFundData item, FundsAvailableIncomingTrainingData fundsAvailable)
        {
            string payableDate = item.SummFAFR_FA_Payable_Dte?.AsJulianString();
            decimal divertAmount = (item.SummDF_DivertedDbtrAmt_Money ?? 0M) * 100M;
            decimal availAmount = (item.SummFAFR_AvailDbtrAmt_Money ?? 0M) * 100M;

            string result = $"04" +
                            $"{item.Dbtr_Id,7}" +
                            $"{item.SummFAFR_FA_Pym_Id,16}" +
                            $"{payableDate,7}" +
                            $"{divertAmount:0000000000}" +
                            $"{item.EnfSrv_Loc_Cd,2}" +
                            $"{item.EnfSrv_SubLoc_Cd ?? string.Empty,2}" +
                            $"{availAmount:0000000000}" +
                            $"  " +
                            $"{fundsAvailable.XRefSin,9}";

            return result;
        }

        private static string GenerateFooterLine(int itemCount, decimal totalDiverted, string sinHashTotal)
        {
            totalDiverted *= 100M;

            return $"99{itemCount:00000000}{totalDiverted:0000000000}{sinHashTotal}";
        }

        public static string BuildNewCycle(string fileBaseName, int cycle)
        {
            int thisNewCycle = cycle + 1;

            int cycleLength;
            if (fileBaseName.ToUpper().StartsWith("TR"))
            {
                cycleLength = 6;
                if (thisNewCycle == 1000000)
                    thisNewCycle = 1;
            }
            else
            {
                cycleLength = 3;
                if (thisNewCycle == 1000)
                    thisNewCycle = 1;
            }

            string newCycle = thisNewCycle.ToString(new string('0', cycleLength));

            return newCycle;
        }
    }
}
