using DBHelper;
using FileBroker.Business.Helpers;
using FileBroker.Common;
using FOAEA3.Resources.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Incoming.FileWatcher.Fed.Tracing
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ColourConsole.WriteEmbeddedColorLine("Starting [cyan]Ontario[/cyan] Federal Tracing File Monitor");

            var config = new ConfigurationHelper(args);

            var fileBrokerDB = new DBToolsAsync(config.FileBrokerConnection);
            var db = DataHelper.SetupFileBrokerRepositories(fileBrokerDB);

            var foaeaApis = FoaeaApiHelper.SetupFoaeaAPIs(config.ApiRootData);

            var federalFileManager = new IncomingFederalTracingFile(db, foaeaApis, config);

            var allNewFiles = new List<string>();
            await federalFileManager.AddNewFilesAsync(config.FTProot + @"\EI3STS", allNewFiles); // NETP
            await federalFileManager.AddNewFilesAsync(config.FTProot + @"\HR3STS", allNewFiles); // EI Tracing
            await federalFileManager.AddNewFilesAsync(config.FTProot + @"\RC3STS", allNewFiles); // CRA Tracing

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
