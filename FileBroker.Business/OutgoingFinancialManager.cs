using FileBroker.Common.Helpers;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text;

namespace FileBroker.Business
{
    public class OutgoingFinancialManager
    {
        private APIBrokerList APIs { get; }
        private RepositoryList DB { get; }

        private FoaeaSystemAccess FoaeaAccess { get; }

        public OutgoingFinancialManager(APIBrokerList apis, RepositoryList repositories, IFileBrokerConfigurationHelper config)
        {
            APIs = apis;
            DB = repositories;

            FoaeaAccess = new FoaeaSystemAccess(apis, config.FoaeaLogin);
        }
        public async Task<string> CreateIFMSfile(string fileBaseName, List<string> errors)
        {
            var fileTableData = (await DB.FileTable.GetFileTableDataForCategoryAsync("IFMSFDOUT"))
                                    .Where(s => s.Active == true).First();
            string cycle = fileTableData.Cycle.ToString();
            string newFilePath = fileTableData.Path.AppendToPath(fileTableData.Name + cycle, isFileName: true);

            if (File.Exists(newFilePath))
            {
                errors.Add("** Error: File Already Exists");
                return "";
            }

            await FoaeaAccess.SystemLoginAsync();
            try
            {
                var processCodes = await DB.ProcessParameterTable.GetProcessCodesAsync(fileTableData.PrcId);
                
                var batches = await APIs.FinancialEvents.GetActiveCR_PADReventsAsync(processCodes.EnfSrv_Cd); 

                if ((batches is null) || (batches.Count() != 1))
                {
                    errors.Add("** Error: Too many IFMS batches!?");
                    return "";
                }

                var batch = batches.First();
                
                var data = await APIs.FinancialEvents.GetIFMSasync(batch.Batch_Id);

                string newCycle = (int.Parse(cycle) + 1).ToString();
                string fileContent = GenerateOutputFileContentFromData(data, newCycle);

                await File.WriteAllTextAsync(newFilePath, fileContent);

                /*
                Dim finDataBatch As New MidTier.Business.DataEntryControlBatchSystem(dataservice.ConnectionStringFOAEA)
                finDataBatch.UpdateFTPControlBatch(BatchID)

                'close events
                finData.UpdatePADRBatchEventData(BatchID, dataservice.EnfSrv_Cd)

                dataservice.InsertOutboundAudit(dataservice.FileName, Date.Now, True, dataservice.ProcessCategory & " Outbound IFMS File created successfully.")
                 */
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
