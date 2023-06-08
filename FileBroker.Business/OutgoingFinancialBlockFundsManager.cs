using FileBroker.Common.Helpers;
using System.Text;

namespace FileBroker.Business;

public class OutgoingFinancialBlockFundsManager
{
    private APIBrokerList APIs { get; }
    private RepositoryList DB { get; }

    private FoaeaSystemAccess FoaeaAccess { get; }

    public OutgoingFinancialBlockFundsManager(APIBrokerList apis, RepositoryList repositories, IFileBrokerConfigurationHelper config)
    {
        APIs = apis;
        DB = repositories;

        FoaeaAccess = new FoaeaSystemAccess(apis, config.FoaeaLogin);
    }

    public async Task<string> CreateBlockFundsFile(string fileBaseName, List<string> errors)
    {
        var fileTableData = await DB.FileTable.GetFileTableDataForFileName(fileBaseName);

        string newCycle = OutgoingFinancialDivertFundsManager.BuildNewCycle(fileBaseName, fileTableData.Cycle);

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

            var blockFundsData = await APIs.Financials.GetBlockFunds(processCodes.EnfSrv_Cd);

            if ((blockFundsData is null) || (blockFundsData.Count == 0))
            {
                errors.Add("** Error: No Block Fund data!?");
                return "";
            }

            string fileContent = GenerateOutputFileContentFromData(blockFundsData, newCycle, processCodes.EnfSrv_Cd);
            await File.WriteAllTextAsync(newFilePath, fileContent);

            if (fileTableData.Transform)
                await TransformFile.Process(fileTableData, newFilePath);

            await DB.FileTable.SetNextCycleForFileType(fileTableData, newCycle.Length);

            string message = fileTableData.Category + $" Outbound {fileBaseName} file created successfully.";
            await DB.OutboundAuditTable.InsertIntoOutboundAudit(fileBaseName + "." + newCycle, DateTime.Now,
                                                                     fileCreated: true, message);

        }
        catch (Exception e)
        {
            await DB.OutboundAuditTable.InsertIntoOutboundAudit(fileBaseName + "." + newCycle, DateTime.Now,
                                                                     fileCreated: true, e.Message);
        }
        finally
        {
            await FoaeaAccess.SystemLogout();
        }

        return newFilePath;
    }

    private static string GenerateOutputFileContentFromData(List<BlockFundData> data, string newCycle, string enfSrv)
    {
        var result = new StringBuilder();

        result.AppendLine(GenerateHeaderLine(newCycle, enfSrv));
        int itemCount = 0;
        long hashSinTotal = 0;
        foreach (var item in data)
        {
            result.AppendLine(GenerateDetailLine(item));
            itemCount++;
            hashSinTotal += long.Parse(item.Appl_Dbtr_Cnfrmd_SIN);
        }

        string sinHashTotal = hashSinTotal.ToString().PadLeft(9, '0');
        if (sinHashTotal.Length >= 10)
            sinHashTotal = sinHashTotal[..9];
        result.AppendLine(GenerateFooterLine(itemCount, sinHashTotal));

        return result.ToString();
    }

    private static string GenerateHeaderLine(string newCycle, string enfSrv)
    {
        string julianDate = DateTime.Now.AsJulianString();

        return $"01{newCycle}{julianDate}{enfSrv}  PROD";
    }

    private static string GenerateDetailLine(BlockFundData item)
    {
        string transactionType = "1";
        string result = $"02{transactionType}{item.Dbtr_Id}{item.Appl_Dbtr_Cnfrmd_SIN}" +
                        $"{item.Start_Dte.AsJulianString(),7}{item.End_Dte.AsJulianString(),7}";

        return result;
    }

    private static string GenerateFooterLine(int itemCount, string sinHashTotal)
    {
        return $"99{itemCount:00000000}0000000000{sinHashTotal}";
    }
        
}
