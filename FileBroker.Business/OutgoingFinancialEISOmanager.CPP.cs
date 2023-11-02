using System.Text;

namespace FileBroker.Business
{
    public partial class OutgoingFinancialEISOmanager
    {
        public async Task<string> CreateCPPfile(string fileBaseName, List<string> errors)
        {
            var fileTableData = await DB.FileTable.GetFileTableDataForFileName(fileBaseName);

            string newCycle = FormatCycleEISO(fileTableData.Cycle + 1);
            string newFilePath = fileTableData.Path.AppendToPath(fileTableData.Name + "." + newCycle, isFileName: true);

            if (File.Exists(newFilePath))
            {
                errors.Add("** Error: File Already Exists");
                return "";
            }

            await FoaeaAccess.SystemLogin();
            try
            {
                var processCodes = await DB.ProcessParameterTable.GetProcessCodes(fileTableData.PrcId);

                var data = await APIs.InterceptionApplications.GetEIexchangeOutData(processCodes.EnfSrv_Cd);

                string fileContent = GenerateCPPOutputFileContentFromData(data, newCycle);
                await File.WriteAllTextAsync(newFilePath, fileContent);

                await DB.FileTable.SetNextCycleForFileType(fileTableData, newCycle.Length);

                string message = fileTableData.Category + " Outbound CPP EISO File created successfully.";
                await DB.OutboundAuditTable.InsertIntoOutboundAudit(fileBaseName + "." + newCycle, DateTime.Now,
                                                                         fileCreated: true, message);
            }
            catch (Exception e)
            {
                string errorMessage = e.Message;
                await DB.OutboundAuditTable.InsertIntoOutboundAudit(fileBaseName + "." + newCycle, DateTime.Now,
                                                                         fileCreated: true, errorMessage);
            }
            finally
            {
                await FoaeaAccess.SystemLogout();
            }

            return newFilePath;
        }

        private static string GenerateCPPOutputFileContentFromData(List<EIoutgoingFederalData> data, string newCycle)
        {
            var result = new StringBuilder();

            result.AppendLine(GenerateCPPHeaderLine(newCycle));
            int totalCount = 0;
            foreach (var item in data)
            {
                result.AppendLine(GenerateCPPDetailLine(item));
                totalCount++;
            }

            result.AppendLine(GenerateCPPFooterLine(totalCount));

            return result.ToString();
        }

        private static string GenerateCPPHeaderLine(string newCycle)
        {
            return $"01DOJ CPP {DateTime.Now:ddMMyyyyHHmmssffff}00000P{newCycle}";
        }

        private static string GenerateCPPDetailLine(EIoutgoingFederalData item)
        {
            string blank = string.Empty;
            long arrearsBalance = Convert.ToInt64(item.Arrears_Balance * 100M);
            long outstandingFees = Convert.ToInt64(item.FeeOwedTtl_Money * 100M);
            long debtorFixedAmount = Convert.ToInt64(item.Debtor_Fixed_Amt ?? 0M * 100M);
            long amountPerPayment = Convert.ToInt64(item.Amount_Per_Payment ?? 0M * 100M);
            string fixedAmountFlag = item.Fixed_Amt_Flag ? "1" : "0";

            string result = $"02{item.Appl_Dbtr_Cnfrmd_SIN,9}{item.Dbtr_Id,7}{item.Appl_JusticeNrSfx,1}" +
                            $"{item.Debt_Percentage:000}{arrearsBalance:000000000}{outstandingFees:000000000}" +
                            $"{debtorFixedAmount:000000000}{amountPerPayment:000000000}{item.EnfOff_Fin_VndrCd.Trim(),7}" +
                            $"{fixedAmountFlag,1}{blank,76}";
            return result;
        }

        private static string GenerateCPPFooterLine(int totalCount)
        {
            string filler = string.Empty;
            return $"99{totalCount:000000000}000000000000000{filler,116}";
        }
    }
}
