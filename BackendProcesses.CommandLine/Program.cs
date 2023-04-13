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
            DB = new DbRepositories(mainDB);
            DBfinance = new DbRepositories_Finance(mainDB);

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

            Console.WriteLine("Backend Processes Command Line Tool");
            Console.WriteLine("");
            Console.WriteLine("FOAEA DB: " + mainDB.ConnectionString);
            Console.WriteLine("Email Recipients: " + Config.Recipients.EmailRecipients);
            Console.WriteLine("");
            Console.WriteLine("OPTION 1 - Nightly Process");
            Console.WriteLine("OPTION 2 - Process Bring Forwards");
            Console.WriteLine("OPTION 3 - Close Completed Applications");
            Console.WriteLine("OPTION 4 - Run Auto Swear");
            Console.WriteLine("OPTION 5 - Run Auto Accept Variations for QC");
            Console.WriteLine("OPTION 6 - ESD Event Processing");
            Console.WriteLine("OPTION 7 - Update Fixed Amount Recalc Date");
            Console.WriteLine("OPTION 8 - Create ESDC (NEPT) Trace Events");
            Console.WriteLine("OPTION 9 - FTP Batches Without Transactions Notifications");
            Console.WriteLine("OPTION 10 - Run Amount Owed Calculations");
            Console.WriteLine("OPTION 11 - Delete CRA Outgoing History Data");
            Console.WriteLine("");
            Console.WriteLine("OPTION X - Exit");
            Console.WriteLine("");
            Console.Write("Enter OPTION number: ");
            Console.WriteLine("");

            string option;
            if (args.Length == 0)
                option = Console.ReadLine() ?? string.Empty;
            else
                option = args[0] ?? string.Empty;

            var user = UserHelper.CreateSystemAdminUser();

            if ((option.ToUpper() != "X") && (ValidationHelper.IsValidInteger(option)))
            {
                switch (option)
                {
                    case "1":
                        var nightlyProcess = new NightlyProcess(DB, DBfinance, Config, user);
                        await nightlyProcess.RunAsync();
                        break;

                    case "2":
                        var bringForwardProcess = new BringForwardEventProcess(DB, DBfinance, Config, user);
                        await bringForwardProcess.RunAsync();
                        break;

                    case "3":
                        var completeI01process = new CompletedInterceptionsProcess(DB, DBfinance, Config, user);
                        await completeI01process.RunAsync();
                        break;

                    case "4":
                        var autoSwearProcess = new AutoSwearProcess(DB, DBfinance, Config, user);
                        await autoSwearProcess.RunAsync();
                        break;

                    case "5":
                        var autoAcceptProcess = new AutoAcceptProcess(DB, DBfinance, Config, user);
                        await autoAcceptProcess.RunAsync("QC");
                        break;

                    case "6":
                        var esdEventProcess = new ESDEventProcess(DB, DBfinance, Config, user);
                        await esdEventProcess.RunAsync();
                        break;

                    case "7":
                        var updateFixedAmountRecalcDate = new UpdateFixedAmountRecalcDate(DB, DBfinance, Config, user);
                        await updateFixedAmountRecalcDate.RunAsync();
                        break;

                    case "8":
                        var createESDC_NETP_eventsProcess = new CreateESDC_NETP_eventsProcess(DB, Config, user);
                        await createESDC_NETP_eventsProcess.RunAsync();
                        break;

                    case "9":
                        var ftBatchWithoutTransactionNotice = new FTBatchWithoutTransactionNoticeProcess(DB, DBfinance, Config, user);
                        await ftBatchWithoutTransactionNotice.RunAsync();
                        break;

                    case "10":
                        var amountOwedProcess = new AmountOwedProcess(DB, DBfinance);
                        await amountOwedProcess.RunAsync();
                        break;

                    case "11":
                        var deleteReconciliationDataProcess = new DeleteReconciliationDataProcess(DB, DBfinance, Config, user);
                        await deleteReconciliationDataProcess.RunAsync();
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

        }

    }
}
