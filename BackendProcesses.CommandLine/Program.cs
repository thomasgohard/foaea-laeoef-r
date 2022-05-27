using BackendProcesses.Business;
using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Data.DB;
using FOAEA3.Resources.Helpers;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace BackendProcesses.CommandLine
{
    class Program
    {
        static void Main(string[] args)
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

            var mainDB = new DBTools(configuration.GetConnectionString("FOAEAMain").ReplaceVariablesWithEnvironmentValues());

            var repositories = new DbRepositories(mainDB);
            var repositoriesFinance = new DbRepositories_Finance(mainDB);

            // preload reference data

            ReferenceData.Instance().LoadFoaEvents(new DBFoaMessage(mainDB));
            ReferenceData.Instance().LoadActiveStatuses(new DBActiveStatus(mainDB));
            ReferenceData.Instance().LoadGenders(new DBGender(mainDB));
            ReferenceData.Instance().LoadProvinces(new DBProvince(mainDB));
            ReferenceData.Instance().LoadMediums(new DBMedium(mainDB));
            ReferenceData.Instance().LoadLanguages(new DBLanguage(mainDB));
            ReferenceData.Instance().LoadDocumentTypes(new DBDocumentType(mainDB));
            ReferenceData.Instance().LoadCountries(new DBCountry(mainDB));
            ReferenceData.Instance().LoadApplicationReasons(new DBApplicationReason(mainDB));
            ReferenceData.Instance().LoadApplicationCategories(new DBApplicationCategory(mainDB));
            ReferenceData.Instance().LoadApplicationLifeStates(new DBApplicationLifeState(mainDB));
            ReferenceData.Instance().LoadApplicationComments(new DBApplicationComments(mainDB));

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
                amountOwedProcess.Run();
            }

            Console.WriteLine("Exiting... Press any key to continue...");

            if (args.Length == 0)
                Console.ReadKey();
        }
    }
}
