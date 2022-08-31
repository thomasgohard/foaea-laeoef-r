using BackendProcesses.Business;
using DBHelper;
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

            await ReferenceData.Instance().LoadFoaEventsAsync(new DBFoaMessage(mainDB));
            await ReferenceData.Instance().LoadActiveStatusesAsync(new DBActiveStatus(mainDB));
            await ReferenceData.Instance().LoadGendersAsync(new DBGender(mainDB));
            await ReferenceData.Instance().LoadProvincesAsync(new DBProvince(mainDB));
            await ReferenceData.Instance().LoadMediumsAsync(new DBMedium(mainDB));
            await ReferenceData.Instance().LoadLanguagesAsync(new DBLanguage(mainDB));
            await ReferenceData.Instance().LoadDocumentTypesAsync(new DBDocumentType(mainDB));
            await ReferenceData.Instance().LoadCountriesAsync(new DBCountry(mainDB));
            await ReferenceData.Instance().LoadApplicationReasonsAsync(new DBApplicationReason(mainDB));
            await ReferenceData.Instance().LoadApplicationCategoriesAsync(new DBApplicationCategory(mainDB));
            await ReferenceData.Instance().LoadApplicationLifeStatesAsync(new DBApplicationLifeState(mainDB));
            await ReferenceData.Instance().LoadApplicationCommentsAsync(new DBApplicationComments(mainDB));

            ReferenceData.Instance().Configuration.Add("emailRecipients", configuration.GetSection("emailRecipients").Value);

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
