using DBHelper;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using Incoming.Common;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Incoming.FileWatcher.MEP.BritishColumbia
{
    class Program
    {
        //private static readonly string appGuid = "AAD06C6C-727D-4C79-9C32-11C15F208845";

        static void Main(string[] args)
        {
            // expects to be called by scheluder on a regular basis

            //using var mutex = new Mutex(false, appGuid);

            //if (!mutex.WaitOne(0, false))
            //{
            //    Console.WriteLine("Instance already running");
            //    return;
            //}

            string aspnetCoreEnvironment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{aspnetCoreEnvironment}.json", optional: true, reloadOnChange: true)
                .AddCommandLine(args);

            IConfigurationRoot configuration = builder.Build();

            var mainDB = new DBTools(configuration.GetConnectionString("MessageBroker").ReplaceVariablesWithEnvironmentValues());

            var apiRootData = configuration.GetSection("APIroot").Get<ApiConfig>();

            // process any new files

            var apiAction = new APIBrokerHelper();
            var provincialFileManager = new IncomingProvincialFile(mainDB, apiRootData, apiAction,
                                                                   defaultProvincePrefix: "BC3B",
                                                                   tracingOverridePrefix: "BC3V");

            string ftpRoot = configuration["FTProot"];
            var newFiles = provincialFileManager.GetNewFiles(ftpRoot + @"\BC3B01");
            var newTracingFiles = provincialFileManager.GetNewFiles(ftpRoot + @"\BC3V01");
            foreach (var newTracingFile in newTracingFiles) // combine new files from both folders
                newFiles.Add(newTracingFile.Key, newTracingFile.Value);

            if (newFiles.Count > 0)
            {
                foreach (var newFile in newFiles)
                    provincialFileManager.ProcessNewFile(newFile.Key);
            }

        }
    }
}
