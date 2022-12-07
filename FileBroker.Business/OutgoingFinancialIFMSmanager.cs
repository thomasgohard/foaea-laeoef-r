using FileBroker.Common.Helpers;
using System.Text;

namespace FileBroker.Business
{
    public class OutgoingFinancialIFMSmanager
    {
        private APIBrokerList APIs { get; }
        private RepositoryList DB { get; }

        private FoaeaSystemAccess FoaeaAccess { get; }

        public OutgoingFinancialIFMSmanager(APIBrokerList apis, RepositoryList repositories, IFileBrokerConfigurationHelper config)
        {
            APIs = apis;
            DB = repositories;

            FoaeaAccess = new FoaeaSystemAccess(apis, config.FoaeaLogin);
        }

        public async Task<string> CreateIFMSfile(string fileBaseName, List<string> errors)
        {
            var fileTableData = (await DB.FileTable.GetFileTableDataForCategoryAsync("IFMSFDOUT"))
                                    .Where(s => s.Active == true).First();
            
            string newCycle = (fileTableData.Cycle + 1).ToString();
            string newFilePath = fileTableData.Path.AppendToPath(fileTableData.Name + newCycle, isFileName: true);

            if (File.Exists(newFilePath))
            {
                errors.Add("** Error: File Already Exists");
                return "";
            }

            await FoaeaAccess.SystemLoginAsync();
            try
            {
                var processCodes = await DB.ProcessParameterTable.GetProcessCodesAsync(fileTableData.PrcId);

                var batches = await APIs.Financials.GetActiveCR_PADReventsAsync(processCodes.EnfSrv_Cd);

                if ((batches is null) || (batches.Count == 0))
                {
                    errors.Add("** Error: No IFMS batches!?");
                    return "";
                }

                if (batches.Count != 1)
                {
                    errors.Add("** Error: Too many IFMS batches!?");
                    return "";
                }

                var batch = batches.First();

                var data = await APIs.Financials.GetIFMSasync(batch.Batch_Id);

                string fileContent = GenerateOutputFileContentFromData(data, newCycle);
                await File.WriteAllTextAsync(newFilePath, fileContent);

                await APIs.Financials.CloseCR_PADReventsAsync(batch.Batch_Id, processCodes.EnfSrv_Cd);
                await APIs.Financials.CloseControlBatch(batch.Batch_Id);

                await DB.FileTable.SetNextCycleForFileTypeAsync(fileTableData, newCycle.Length);

                string message = fileTableData.Category + " Outbound IFMS File created successfully.";
                await DB.OutboundAuditTable.InsertIntoOutboundAuditAsync(fileBaseName + "." + newCycle, DateTime.Now,
                                                                         fileCreated: true, message);

            }
            catch (Exception e)
            {
                await DB.OutboundAuditTable.InsertIntoOutboundAuditAsync(fileBaseName + "." + newCycle, DateTime.Now,
                                                         fileCreated: true, e.Message);
            }
            finally
            {
                await FoaeaAccess.SystemLogoutAsync();
            }

            return newFilePath;
        }

        private static string GenerateOutputFileContentFromData(List<IFMSdata> data, string newCycle)
        {
            var result = new StringBuilder();

            result.AppendLine(GenerateHeaderLine(newCycle));
            int chequeCount = 0;
            decimal totalAmount = 0M;
            int totalIPUnumber = 0;
            foreach (var item in data)
            {
                result.AppendLine(GenerateDetailLine(item));
                chequeCount++;
                totalAmount += item.TransferAmt_Money;
                totalIPUnumber += int.Parse(item.IPU_Nr);
            }

            result.AppendLine(GenerateFooterLine(chequeCount, totalAmount, totalIPUnumber));

            return result.ToString();
        }

        private static string GenerateHeaderLine(string newCycle)
        {
            string julianDate = DateTime.Now.AsJulianString();

            return $"01{newCycle}{julianDate}";
        }

        private static string GenerateDetailLine(IFMSdata item)
        {
            decimal transferAmt_MoneyNoDecimal = item.TransferAmt_Money * 100M;
            string result = $"02{item.IPU_Nr,7}{transferAmt_MoneyNoDecimal,13}{item.EnfOff_Fin_VndrCd,7}" +
                            $"{item.Court,49}";

            return result;
        }

        private static string GenerateFooterLine(int chequeCount, decimal totalAmount, int totalIPUnumber)
        {
            decimal totalAmountNoDecimal = totalAmount * 100M;
            string totalIPnum = totalIPUnumber.ToString().Substring(totalIPUnumber.ToString().Length - 7);

            return $"99{chequeCount:000}{totalAmountNoDecimal:0000000000000}{totalIPnum:0000000}";
        }
    }
}
