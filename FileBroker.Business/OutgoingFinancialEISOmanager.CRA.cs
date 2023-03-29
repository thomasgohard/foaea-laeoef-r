using System.Text;

namespace FileBroker.Business
{
    public partial class OutgoingFinancialEISOmanager
    {
        public async Task<string> CreateCRAfile(string fileBaseName, List<string> errors)
        {
            var fileTableData = await DB.FileTable.GetFileTableDataForFileNameAsync(fileBaseName);

            string newCycle = FormatCycleEISO(fileTableData.Cycle + 1);
            string newFilePath = fileTableData.Path.AppendToPath(fileTableData.Name + "." + newCycle, isFileName: true);

            if (File.Exists(newFilePath))
            {
                errors.Add("** Error: File Already Exists");
                return "";
            }

            await FoaeaAccess.SystemLoginAsync();
            try
            {
                var processCodes = await DB.ProcessParameterTable.GetProcessCodesAsync(fileTableData.PrcId);

                var data = await APIs.InterceptionApplications.GetEISOvalidApplications();

                string fileContent = GenerateCRAOutputFileContentFromData(data, newCycle);
                await File.WriteAllTextAsync(newFilePath, fileContent);

                await DB.FileTable.SetNextCycleForFileTypeAsync(fileTableData, newCycle.Length);

                string message = fileTableData.Category + " Outbound CRA EISO File created successfully.";
                await DB.OutboundAuditTable.InsertIntoOutboundAuditAsync(fileBaseName + "." + newCycle, DateTime.Now,
                                                                         fileCreated: true, message);

            }
            catch (Exception e)
            {
                string errorMessage = e.Message;
                await DB.OutboundAuditTable.InsertIntoOutboundAuditAsync(fileBaseName + "." + newCycle, DateTime.Now,
                                                                         fileCreated: true, errorMessage);
            }
            finally
            {
                await FoaeaAccess.SystemLogoutAsync();
            }

            return newFilePath;
        }

        private static string GenerateCRAOutputFileContentFromData(List<ProcessEISOOUTHistoryData> data, string newCycle)
        {
            var result = new StringBuilder();

            result.AppendLine(GenerateCRAHeaderLine(newCycle));
            int totalCount = 0;
            long totalTransfer = 0;
            long sinTotal = 0;
            foreach (var item in data)
            {
                result.AppendLine(GenerateCRADetailLine(item));
                totalCount++;
                totalTransfer += long.Parse(item.TRANS_AMT);
                sinTotal += long.Parse(item.ACCT_NBR);
            }

            result.AppendLine(GenerateCRAFooterLine(totalCount, totalTransfer, sinTotal));

            return result.ToString();
        }

        private static string GenerateCRAHeaderLine(string newCycle)
        {
            return $"0101310201{DateTime.Now:yyyyMMddHHmmss}PROD{newCycle}";
        }

        private static string GenerateCRADetailLine(ProcessEISOOUTHistoryData item)
        {
            string blank = string.Empty;
            string result = $"02{item.ACCT_NBR,9}{item.TRANS_AMT:00000000000}{item.RQST_RFND_EID,8}" +
                            $"{item.OUTPUT_DEST_CD:0000}{item.XREF_ACCT_NBR,12}{item.FOA_DELETE_IND:0}{item.FOA_RECOUP_PRCNT:000}" +
                            $"{blank,30}";
            return result;
        }

        private static string GenerateCRAFooterLine(int totalCount, long totalTransfer, long sinTotal)
        {
            string sinTotalString;
            if (sinTotal.ToString().Length < 9)
                sinTotalString = sinTotal.ToString().PadLeft(9, '0');
            else
                sinTotalString = sinTotal.ToString().PadLeft(9, '0').Substring(sinTotal.ToString().Length - 9);

            string filler = string.Empty;

            return $"99{totalCount:000000000}{totalTransfer:000000000000000}{sinTotalString}{filler,45}";
        }
    }
}
