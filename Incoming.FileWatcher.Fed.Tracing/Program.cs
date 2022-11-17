using DBHelper;
using FileBroker.Business.Helpers;
using FileBroker.Common;
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
            var db = DataHelper.SetupFileBrokerRepositories(fileBrokerDB);
            var apiRootData = configuration.GetSection("APIroot").Get<ApiConfig>();

            var foaeaApis = FoaeaApiHelper.SetupFoaeaAPIs(apiRootData);

            var federalFileManager = new IncomingFederalTracingFile(db, foaeaApis, configuration);

            string ftpRoot = configuration["FTProot"];

            var allNewFiles = new List<string>();
            await federalFileManager.AddNewFilesAsync(ftpRoot + @"\EI3STS", allNewFiles); // NETP
            await federalFileManager.AddNewFilesAsync(ftpRoot + @"\HR3STS", allNewFiles); // EI Tracing
            await federalFileManager.AddNewFilesAsync(ftpRoot + @"\RC3STS", allNewFiles); // CRA Tracing

            if (allNewFiles.Count > 0)
            {
                ColourConsole.WriteEmbeddedColorLine($"Found [green]{allNewFiles.Count}[/green] file(s)");
                foreach (var newFile in allNewFiles)
                {
                    var errors = new List<string>();
                    ColourConsole.WriteEmbeddedColorLine($"Processing [green]{newFile}[/green]...");
                    await federalFileManager.ProcessNewFileAsync(newFile);
                    if (federalFileManager.Errors.Any())
                        foreach (var error in federalFileManager.Errors)
                            await db.ErrorTrackingTable.MessageBrokerErrorAsync("TRCIN", newFile, new Exception(error), displayExceptionError: true);

                }
            }
            else
                ColourConsole.WriteEmbeddedColorLine("[yellow]No new files found.[/yellow]");
        }

    }
}
