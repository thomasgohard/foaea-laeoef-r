using DBHelper;
using FOAEA3.Business.Areas.BackendProcesses;
using FOAEA3.Business.BackendProcesses;
using FOAEA3.Common.Helpers;
using FOAEA3.Data.Base;
using FOAEA3.Data.DB;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Repository;
using FOAEA3.Resources.Helpers;
using System;
using System.Threading.Tasks;

namespace BackendProcesses.CommandLine
{
    class Program
    {
        private static IFoaeaConfigurationHelper Config { get; set; }
        private static IRepositories DB { get; set; }
        private static IRepositories_Finance DBfinance { get; set; }

        static async Task Main(string[] args)
        {
            Config = new FoaeaConfigurationHelper(args);

            var mainDB = new DBToolsAsync(Config.FoaeaConnection);
            var DB = new DbRepositories(mainDB);
            var DBfinance = new DbRepositories_Finance(mainDB);

            var loadRefFoaEvents = ReferenceData.Instance().LoadFoaEventsAsync(new DBFoaMessage(mainDB));
            var loadRefActiveStatus = ReferenceData.Instance().LoadActiveStatusesAsync(new DBActiveStatus(mainDB));
            var loadRefGenders = ReferenceData.Instance().LoadGendersAsync(new DBGender(mainDB));
            var loadRefProvinces = ReferenceData.Instance().LoadProvincesAsync(new DBProvince(mainDB));
            var loadRefMediums = ReferenceData.Instance().LoadMediumsAsync(new DBMedium(mainDB));
            var loadRefLanguages = ReferenceData.Instance().LoadLanguagesAsync(new DBLanguage(mainDB));
            var loadRefDocTypes = ReferenceData.Instance().LoadDocumentTypesAsync(new DBDocumentType(mainDB));
            var loadRefCountries = ReferenceData.Instance().LoadCountriesAsync(new DBCountry(mainDB));
            var loadRefAppReasons = ReferenceData.Instance().LoadApplicationReasonsAsync(new DBApplicationReason(mainDB));
            var loadRefAppCategories = ReferenceData.Instance().LoadApplicationCategoriesAsync(new DBApplicationCategory(mainDB));
            var loadRefAppLifeStates = ReferenceData.Instance().LoadApplicationLifeStatesAsync(new DBApplicationLifeState(mainDB));
            var loadRefAppComments = ReferenceData.Instance().LoadApplicationCommentsAsync(new DBApplicationComments(mainDB));

            await Task.WhenAll(loadRefFoaEvents, loadRefActiveStatus, loadRefGenders, loadRefProvinces, loadRefMediums,
                               loadRefLanguages, loadRefDocTypes, loadRefCountries, loadRefAppReasons,
                               loadRefAppCategories, loadRefAppLifeStates, loadRefAppComments);

            Console.WriteLine("Message Broker Command Line Tool");
            Console.WriteLine("");
            Console.WriteLine("MessageBroker DB: ");
            Console.WriteLine("FOAEA DB: " + mainDB.ConnectionString);
            Console.WriteLine("Email Recipients: " + Config.Recipients.EmailRecipients);
            Console.WriteLine("");
            Console.WriteLine("OPTION 1 - Run FOAEA Job");
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
                option = Console.ReadLine();
            else
                option = args[0];

            if (ValidationHelper.IsValidInteger(option))
            {
                switch (option)
                {
                    case "1":
                        string processName;
                        if (args.Length > 1)
                            processName = args[1];
                        else
                        {
                            Console.WriteLine("");
                            Console.WriteLine("Run FOAEA job:");
                            processName = Console.ReadLine();
                        }
                        await RunFOAEAJob(processName);
                        break;
                    case "2":
                        //CheckForNewFiles();
                        break;
                    case "3":
                        //DisableFileProcess(args[1]);
                        break;
                    case "4":
                        //EnableFileProcess(args[1]);
                        break;
                    case "5":
                        //SendPADREmail();
                        break;
                    case "6":
                        //CreateDebtorLetters();
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

        }

        private static async Task RunFOAEAJob(string processName)
        {

            if (processName.ToUpper().IndexOf("CRA") > -1)
            {
                // EISO_OUT();
                return;
            }
            switch (processName)
            {
                case "daily":
                    //dailyJob()
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
                case "nightly_Process":
                    //nightly_Process();
                    break;
                case "read_bf":
                    var bringForwardProcess = new BringForwardEventProcess(DB, DBfinance, Config);
                    await bringForwardProcess.RunAsync();
                    break;
                case "app_daily":
                    //Appl_Daily();
                    break;
                case "auto_swear":
                    //Auto_Swear();
                    break;
                case "EISO_FT":
                    //ProcessEISOFTData();
                    break;
                case "RestartFileMonitor":
                    //RestartFileMonitor();
                    break;
                case "auto_swearQC":
                    //Auto_SwearQC();
                    break;
                case "ESDEventProcessing":
                    var esdEventProcess = new ESDEventProcess(DB, DBfinance, Config);
                    await esdEventProcess.RunAsync();
                    break;
                case "CPPEISOIN":
                    //CPPEISOIN();
                    break;
                case "QC3M01IN":
                    //QC3M01IN();
                    break;
                case "FixedAmount":
                    //UpdateFixedAmountRecalcDate();
                    break;
                case "NETP":
                    //Create_ESDCEvents();
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
                case "FTBatchWithoutTransactionNotice":
                    //FTBatchWithoutTransactionNotice();
                    break;
                case "amountowed":
                    var amountOwedProcess = new AmountOwedProcess(DB, DBfinance);
                    await amountOwedProcess.RunAsync();
                    break;
                case "Recon":
                    //DeleteReconcilationData();
                    break;
                default:
                    Console.WriteLine("Unknown process: " + processName);
                    break;
            }

        }
    }
}
