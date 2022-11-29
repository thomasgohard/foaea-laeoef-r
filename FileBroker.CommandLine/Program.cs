using DBHelper;
using FileBroker.CommandLine;
using FileBroker.Common;
using FileBroker.Data.DB;
using FileBroker.Model.Interfaces;
using FOAEA3.Resources.Helpers;

IFileBrokerConfigurationHelper Config = new FileBrokerConfigurationHelper(args);

var mainDB = new DBToolsAsync(Config.FileBrokerConnection);

Console.WriteLine("Backend Processes Command Line Tool");
Console.WriteLine("");
Console.WriteLine("FileBroker DB: " + mainDB.ConnectionString);
Console.WriteLine("Email Recipient: " + Config.EmailRecipient);
Console.WriteLine("");
Console.WriteLine("OPTION 1 - Run FileBroker Job");
Console.WriteLine("OPTION 2 - Check for New Files");
Console.WriteLine("OPTION 3 - Disable File Process");
Console.WriteLine("OPTION 4 - Enable File Process");
Console.WriteLine("OPTION 5 - Send PADR Summary Email");
Console.WriteLine("OPTION 6 - Create Debtor Letters");
Console.WriteLine("OPTION X - Exit");
Console.WriteLine("");
Console.Write("Enter OPTION number: ");
Console.WriteLine("");

string option;
if (args.Length == 0)
    option = Console.ReadLine() ?? string.Empty;
else
    option = args[0] ?? string.Empty;

if ((option.ToUpper() != "X") && ValidationHelper.IsValidInteger(option))
{
    string processName;
    string result;

    switch (option)
    {
        case "1":
            if (args.Length > 1)
                processName = args[1];
            else
            {
                Console.WriteLine("");
                Console.WriteLine("Run FileBroker job:");
                processName = Console.ReadLine();
            }
            await RunFileBrokerJob(processName, mainDB);
            break;

        case "2":
            //CheckForNewFiles();
            break;

        case "3":
            if (args.Length > 1)
                processName = args[1];
            else
            {
                Console.WriteLine("");
                Console.WriteLine("Disable File Process:");
                processName = Console.ReadLine();
            }
            result = await DisableFileProcess(processName, mainDB);
            Console.WriteLine(result);
            break;

        case "4":
            if (args.Length > 1)
                processName = args[1];
            else
            {
                Console.WriteLine("");
                Console.WriteLine("Disable File Process:");
                processName = Console.ReadLine();
            }
            result = await EnableFileProcess(processName, mainDB);
            Console.WriteLine(result);
            break;

        case "5":
            //SendPADREmail();
            break;

        case "6":
            //CreateDebtorLetters();
            break;

        default:
            Console.WriteLine("Unknown option selected: " + option);
            break;
    }

    return;
}

if (args.Length == 0)
{
    Console.WriteLine("Exiting... Press any key to continue...");
    Console.ReadKey();
}
else
    Console.WriteLine("Exiting...");

static async Task<string> DisableFileProcess(string fileName, IDBToolsAsync mainDB)
{
    var fileTable = new DBFileTable(mainDB);
    var fileData = await fileTable.GetFileTableDataForFileNameAsync(fileName);
    if ((fileData is not null) && (fileData.Active is true) && (fileData.Type.ToLower() == "in"))
    {
        await fileTable.DisableFileProcess(fileData.PrcId);
        return $"FOAEA File: {fileName} disabled.";
    }
    else
        return "Invalid FOAEA File: " + fileName;
}

static async Task<string> EnableFileProcess(string fileName, IDBToolsAsync mainDB)
{
    var fileTable = new DBFileTable(mainDB);
    var fileData = await fileTable.GetFileTableDataForFileNameAsync(fileName);
    if ((fileData is not null) && (fileData.Active is true) && (fileData.Type.ToLower() == "in"))
    {
        await fileTable.EnableFileProcess(fileData.PrcId);
        return $"FOAEA File: {fileName} disabled.";
    }
    else
        return "Invalid FOAEA File: " + fileName;
}

static async Task RunFileBrokerJob(string processName, IDBToolsAsync mainDB)
{

    if (processName.ToUpper().IndexOf("CRA") > -1)
    {
        // EISO_OUT();
        return;
    }
    switch (processName)
    {
        case "daily":
            await DailyJob.Run();
            break;

        case "weekly":
            //Weeklyjob();
            break;

        case "EISO_OUT":
            //EISO_OUT();
            break;

        case "EIEISO_OUT":
            //EIEISO_OUT();
            break;

        case "EIEISO_OUT_2":
            //EIEISO_OUT_2();
            break;

        case "CPPEISO_OUT":
            //CPPEISO_OUT();
            break;

        case "DF_OUT":
            //DF_OUT();
            break;

        case "EISO_FT":
            //ProcessEISOFTData();
            break;

        case "RestartFileMonitor":
            //RestartFileMonitor();
            break;

        case "CPPEISOIN":
            //CPPEISOIN();
            break;

        case "QC3M01IN":
            //QC3M01IN();
            break;

        case "EISOIN":   //CR1208
            //CRAEISOIN();
            break;

        case "PADRXML":
            //CreatePADRXMLFiles();
            break;

        case "PADRReports":
            //CreatePADRReportFiles();
            break;

        case "PADR_PDFXLS":
            //CreatePADRFiles_PDF_XLS();
            break;

        case "PADR_PDFXLS_New":
            //CreatePADRFiles_PDF_XLSFromReport();
            break;

        case "PADR_AllDocuments":
            //ReCreatePADRDocuemnts();
            break;

        case "DebtorLetters":
            //CreateDebtorLettersPDF();
            break;

        default:
            Console.WriteLine("Unknown process: " + processName);
            break;
    }

}