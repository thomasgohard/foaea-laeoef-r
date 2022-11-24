using DBHelper;
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
Console.WriteLine("OPTION 1 - Check for New Files");
Console.WriteLine("OPTION 2 - Disable File Process");
Console.WriteLine("OPTION 3 - Enable File Process");
Console.WriteLine("OPTION 4 - Send PADR Summary Email");
Console.WriteLine("OPTION 5 - Create Debtor Letters");
Console.WriteLine("OPTION X - Exit");
Console.WriteLine("");
Console.Write("Enter OPTION number: ");
Console.WriteLine("");

string option;
if (args.Length == 0)
    option = Console.ReadLine();
else
    option = args[0];

if (ValidationHelper.IsValidInteger(option))
{
    string processName;
    string result;

    switch (option)
    {
        case "1":
            //CheckForNewFiles();
            break;
        case "2":
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
        case "3":
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
        case "4":
            //SendPADREmail();
            break;
        case "5":
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
