using DBHelper;
using FOAEA3.Business.BackendProcesses;
using FOAEA3.Data.Base;
using FOAEA3.Data.DB;
using FOAEA3.Resources.Helpers;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BackendProcesses.CommandLine
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // read configuration
            string aspnetCoreEnvironment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{aspnetCoreEnvironment}.json", optional: true, reloadOnChange: true)
                .AddCommandLine(args);

            IConfigurationRoot configuration = builder.Build();

            // set up access to database and load some reference tables into memory

            var mainDB = new DBToolsAsync(configuration.GetConnectionString("FOAEAMain").ReplaceVariablesWithEnvironmentValues());

            var repositories = new DbRepositories(mainDB);
            var repositoriesFinance = new DbRepositories_Finance(mainDB);

            // preload reference data
            var loadRefFoaEvents = ReferenceData.Instance().LoadFoaEventsAsync(new DBFoaMessage(repositories.MainDB));
            var loadRefActiveStatus = ReferenceData.Instance().LoadActiveStatusesAsync(new DBActiveStatus(repositories.MainDB));
            var loadRefGenders = ReferenceData.Instance().LoadGendersAsync(new DBGender(repositories.MainDB));
            var loadRefProvinces = ReferenceData.Instance().LoadProvincesAsync(new DBProvince(repositories.MainDB));
            var loadRefMediums = ReferenceData.Instance().LoadMediumsAsync(new DBMedium(repositories.MainDB));
            var loadRefLanguages = ReferenceData.Instance().LoadLanguagesAsync(new DBLanguage(repositories.MainDB));
            var loadRefDocTypes = ReferenceData.Instance().LoadDocumentTypesAsync(new DBDocumentType(repositories.MainDB));
            var loadRefCountries = ReferenceData.Instance().LoadCountriesAsync(new DBCountry(repositories.MainDB));
            var loadRefAppReasons = ReferenceData.Instance().LoadApplicationReasonsAsync(new DBApplicationReason(repositories.MainDB));
            var loadRefAppCategories = ReferenceData.Instance().LoadApplicationCategoriesAsync(new DBApplicationCategory(repositories.MainDB));
            var loadRefAppLifeStates = ReferenceData.Instance().LoadApplicationLifeStatesAsync(new DBApplicationLifeState(repositories.MainDB));
            var loadRefAppComments = ReferenceData.Instance().LoadApplicationCommentsAsync(new DBApplicationComments(repositories.MainDB));

            await Task.WhenAll(loadRefFoaEvents, loadRefActiveStatus, loadRefGenders,
                               loadRefProvinces, loadRefMediums, loadRefLanguages,
                               loadRefDocTypes, loadRefCountries, loadRefAppReasons,
                               loadRefAppCategories, loadRefAppLifeStates, loadRefAppComments);

            ReferenceData.Instance().Configuration.TryAdd("emailRecipients", configuration.GetSection("emailRecipients").Value);

            // for testing
            //UserData.Instance().SubmitterCode = "ON2D68";
            //UserData.Instance().EnforcementServiceCode = "ON01";

            // main code
            Console.WriteLine("Message Broker Command Line Tool");
            Console.WriteLine("");
            Console.WriteLine("MessageBroker DB: ");
            Console.WriteLine("FOAEA DB: " + mainDB.ConnectionString);
            Console.WriteLine("Email Recipients: " + ReferenceData.Instance().Configuration["emailRecipients"]);
            Console.WriteLine("");
            Console.WriteLine("Options:");
            Console.WriteLine("  1 - Amount Owed");
            Console.WriteLine("  X - Exit");
            Console.WriteLine("");

            string option;
            if (args.Length == 0)
                option = Console.ReadLine();
            else
                option = args[0];

            if (option == "1")
            {
                var amountOwedProcess = new AmountOwedProcess(repositories, repositoriesFinance);
                await amountOwedProcess.RunAsync();
            }

            Console.WriteLine("Exiting... Press any key to continue...");

            if (args.Length == 0)
                Console.ReadKey();
        }
    }
}
