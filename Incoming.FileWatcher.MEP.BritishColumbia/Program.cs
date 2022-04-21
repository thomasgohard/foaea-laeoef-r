using DBHelper;
using FileBroker.Data.DB;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using Incoming.Common;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Incoming.FileWatcher.MEP.BritishColumbia
{
    class Program
    {
        //private static readonly string appGuid = "AAD06C6C-727D-4C79-9C32-11C15F208845";

        static void Main(string[] args)
        {
            ColourConsole.WriteEmbeddedColorLine("Starting [cyan]Ontario[/cyan] BC MEP File Monitor");
            
            string aspnetCoreEnvironment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{aspnetCoreEnvironment}.json", optional: true, reloadOnChange: true)
                .AddCommandLine(args);

            IConfigurationRoot configuration = builder.Build();

            var fileBrokerDB = new DBTools(configuration.GetConnectionString("MessageBroker").ReplaceVariablesWithEnvironmentValues());
            var errorTrackingDB = new DBErrorTracking(fileBrokerDB);

            var apiRootData = configuration.GetSection("APIroot").Get<ApiConfig>();

            // process any new files

            var apiAction = new APIBrokerHelper();
            var provincialFileManager = new IncomingProvincialFile(fileBrokerDB, apiRootData, apiAction,
                                                                   defaultProvincePrefix: "BC3B",
                                                                   tracingOverridePrefix: "BC3V");

            string ftpRoot = configuration["FTProot"];
            var newFiles = provincialFileManager.GetNewFiles(ftpRoot + @"\BC3B01");
            var newFiles2 = provincialFileManager.GetNewFiles(ftpRoot + @"\BC3V01");
            foreach (var newTracingFile in newFiles2) // combine new files from both folders
                newFiles.Add(newTracingFile.Key, newTracingFile.Value);

            if (newFiles.Count > 0)
            {
                foreach (var newFile in newFiles)
                {
                    var errors = new List<string>();
                    provincialFileManager.ProcessNewFile(newFile.Key, ref errors);
                    if (errors.Any())
                        foreach (var error in errors)
                            errorTrackingDB.MessageBrokerError("BC APPIN", newFile.Key, new Exception(error), false);
                }
            }

        }
    }
}
