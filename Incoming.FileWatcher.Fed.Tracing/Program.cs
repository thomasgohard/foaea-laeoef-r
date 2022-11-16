using DBHelper;
using FileBroker.Business.Helpers;
using FileBroker.Data.DB;
using FOAEA3.Common.Brokers;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Incoming.FileWatcher.Fed.Tracing
{
    class Program
    {
        private static IncomingFederalTracingFile FederalFileManager;

        static async Task Main(string[] args)
        {
            ColourConsole.WriteEmbeddedColorLine("Starting [cyan]Ontario[/cyan] Federal Tracing File Monitor");

            string aspnetCoreEnvironment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{aspnetCoreEnvironment}.json", optional: true, reloadOnChange: true)
                .AddCommandLine(args);

            IConfiguration configuration = builder.Build();

            var fileBrokerDB = new DBToolsAsync(configuration.GetConnectionString("FileBroker").ReplaceVariablesWithEnvironmentValues());
            var errorTrackingDB = new DBErrorTracking(fileBrokerDB);
            var apiRootForFiles = configuration.GetSection("APIroot").Get<ApiConfig>();
            var apiAction = new APIBrokerHelper(currentSubmitter: LoginsAPIBroker.SYSTEM_SUBMITTER, currentUser: LoginsAPIBroker.SYSTEM_SUBJECT);

            FederalFileManager = new(fileBrokerDB, apiRootForFiles, apiAction);

            string ftpRoot = configuration["FTProot"];

            var allNewFiles = new List<string>();
            await FederalFileManager.AddNewFilesAsync(ftpRoot + @"\EI3STS", allNewFiles); // NETP
            await FederalFileManager.AddNewFilesAsync(ftpRoot + @"\HR3STS", allNewFiles); // EI Tracing
            await FederalFileManager.AddNewFilesAsync(ftpRoot + @"\RC3STS", allNewFiles); // CRA Tracing

            if (allNewFiles.Count > 0)
            {
                ColourConsole.WriteEmbeddedColorLine($"Found [green]{allNewFiles.Count}[/green] file(s)");
                foreach (var newFile in allNewFiles)
                {
                    var errors = new List<string>();
                    ColourConsole.WriteEmbeddedColorLine($"Processing [green]{newFile}[/green]...");
                    await FederalFileManager.ProcessNewFileAsync(newFile);
                    if (FederalFileManager.Errors.Any())
                        foreach (var error in FederalFileManager.Errors)
                            await errorTrackingDB.MessageBrokerErrorAsync("TRCIN", newFile, new Exception(error), displayExceptionError: true);

                }
            }
            else
                ColourConsole.WriteEmbeddedColorLine("[yellow]No new files found.[/yellow]");
        }

    }
}
