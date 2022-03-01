using DBHelper;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using Incoming.Common;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Incoming.FileWatcher.MEP.Ontario
{
    class Program
    {
        //private static readonly string appGuid = "26FA35A8-3AF8-4FBD-A9E4-EFD79483855F";

        static void Main(string[] args)
        {
            // expects to be called by scheluder on a regular basis

            //using var mutex = new Mutex(false, appGuid);

            //if (!mutex.WaitOne(0, false))
            //{
            //    Console.WriteLine("Instance already running");
            //    return;
            //}

            ColourConsole.WriteEmbeddedColorLine("Starting [cyan]Ontario[/cyan] File Monitor");

            string aspnetCoreEnvironment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{aspnetCoreEnvironment}.json", optional: true, reloadOnChange: true)
                .AddCommandLine(args);

            IConfiguration configuration = builder.Build();

            var mainDB = new DBTools(configuration.GetConnectionString("MessageBroker").ReplaceVariablesWithEnvironmentValues());

            var apiRootForFiles = configuration.GetSection("APIroot").Get<ApiConfig>();

            // process any new files
            var apiAction = new APIBrokerHelper();
            var provincialFileManager = new IncomingProvincialFile(mainDB, apiRootForFiles, apiAction, "ON3D01");

            string ftpRoot = configuration["FTProot"];
            var newFiles = provincialFileManager.GetNewFiles(ftpRoot + @"\ON3D01");
            if (newFiles.Count > 0)
            {
                ColourConsole.WriteEmbeddedColorLine($"Found [green]{newFiles.Count}[/green] file(s)");
                foreach (var newFile in newFiles)
                {
                    ColourConsole.WriteEmbeddedColorLine($"Processing [green]{newFile.Key}[/green]...");
                    provincialFileManager.ProcessNewFile(newFile.Key);
                }
            }
            else
                ColourConsole.WriteEmbeddedColorLine("[yellow]No new files found.[/yellow]");

        }
    }
}
