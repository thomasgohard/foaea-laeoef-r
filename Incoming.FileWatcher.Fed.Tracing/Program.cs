using DBHelper;
using FileBroker.Data.DB;
using FileBroker.Model;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using Incoming.Common;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Incoming.FileWatcher.Fed.Tracing
{
    class Program
    {
        private static IncomingFederalTracingFile FederalFileManager;

        static void Main(string[] args)
        {
            ColourConsole.WriteEmbeddedColorLine("Starting [cyan]Ontario[/cyan] Federal Tracing File Monitor");

            string aspnetCoreEnvironment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{aspnetCoreEnvironment}.json", optional: true, reloadOnChange: true)
                .AddCommandLine(args);

            IConfiguration configuration = builder.Build();

            var fileBrokerDB = new DBTools(configuration.GetConnectionString("FileBroker").ReplaceVariablesWithEnvironmentValues());
            var errorTrackingDB = new DBErrorTracking(fileBrokerDB);
            var apiRootForFiles = configuration.GetSection("APIroot").Get<ApiConfig>();
            var apiAction = new APIBrokerHelper(currentSubmitter: "MSGBRO", currentUser: "MSGBRO");

            FederalFileManager = new(fileBrokerDB, apiRootForFiles, apiAction);

            string ftpRoot = configuration["FTProot"];

            var allNewFiles = new List<string>();
            FederalFileManager.AddNewFiles(ftpRoot + @"\EI3STS", ref allNewFiles); // NETP
            FederalFileManager.AddNewFiles(ftpRoot + @"\HR3STS", ref allNewFiles); // EI Tracing
            FederalFileManager.AddNewFiles(ftpRoot + @"\RC3STS", ref allNewFiles); // CRA Tracing

            if (allNewFiles.Count > 0)
            {
                ColourConsole.WriteEmbeddedColorLine($"Found [green]{allNewFiles.Count}[/green] file(s)");
                foreach (var newFile in allNewFiles)
                {
                    var errors = new List<string>();
                    ColourConsole.WriteEmbeddedColorLine($"Processing [green]{newFile}[/green]...");
                    FederalFileManager.ProcessNewFile(newFile, ref errors);
                    if (errors.Any())
                        foreach(var error in errors)
                            errorTrackingDB.MessageBrokerError("TRCIN", newFile, new Exception(error), displayExceptionError: true);

                }
            }
            else
                ColourConsole.WriteEmbeddedColorLine("[yellow]No new files found.[/yellow]");
        }

    }
}
