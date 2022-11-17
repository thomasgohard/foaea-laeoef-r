using DBHelper;
using FileBroker.Business.Helpers;
using FileBroker.Common;
using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using Microsoft.Extensions.Configuration;

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

var federalFileManager = new IncomingFederalSinFile(db, foaeaApis, configuration);

string ftpRoot = configuration["FTProot"];

var allNewFiles = new List<string>();
await federalFileManager.AddNewFilesAsync(ftpRoot + @"\Hr3svs", allNewFiles);

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
                await db.ErrorTrackingTable.MessageBrokerErrorAsync("SININ", newFile, new Exception(error), displayExceptionError: true);

    }
}
else
    ColourConsole.WriteEmbeddedColorLine("[yellow]No new files found.[/yellow]");
