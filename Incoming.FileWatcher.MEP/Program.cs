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

namespace Incoming.FileWatcher.MEP;

internal class Program
{
    static void Main(string[] args)
    {
        string aspnetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{aspnetCoreEnvironment}.json", optional: true, reloadOnChange: true)
            .AddCommandLine(args);

        IConfiguration configuration = builder.Build();

        var fileBrokerDB = new DBTools(configuration.GetConnectionString("MessageBroker").ReplaceVariablesWithEnvironmentValues());

        string provinceCode = GetProvinceCodeBasedOnArgs(args);
        var fileTableData = GetFileTableDataBasedOnArgs(args, new DBFileTable(fileBrokerDB));

        var errorTrackingDB = new DBErrorTracking(fileBrokerDB);

        if (!fileTableData.Any())
        {
            string error = $"No items found in FileTable?";
            ColourConsole.WriteEmbeddedColorLine(error);
            errorTrackingDB.MessageBrokerError($"Incoming MEP File Processing", "Error starting MEP File Monitor",
                                               new Exception(error), displayExceptionError: true);
            return;
        }

        var validProvinces = fileTableData.Where(f => f.Active.HasValue && f.Active.Value)
                                          .GroupBy(f => f.Name[..2].ToUpper())
                                          .Select(group => group.First().Name[..2].ToUpper())
                                          .ToList();

        if ((string.IsNullOrEmpty(provinceCode) || !validProvinces.Contains(provinceCode)) && (provinceCode != "ALL"))
        {
            string error = $"Invalid province argument on command line: [{provinceCode}]\nMust be one of: " +
                             string.Join(", ", validProvinces + " or ALL");
            ColourConsole.WriteEmbeddedColorLine(error);
            errorTrackingDB.MessageBrokerError($"Incoming MEP File Processing", "Error starting MEP File Monitor",
                                               new Exception(error), displayExceptionError: true);
            return;
        }

        ColourConsole.WriteEmbeddedColorLine($"Starting [cyan]{provinceCode}[/cyan] MEP File Monitor");

        var provinceFilesData = fileTableData.Where(f => (f.Name[..2].ToUpper() == provinceCode) || (provinceCode == "ALL"))
                                             .ToList();

        string tracingName = null;
        string interceptionName = null;
        string licenceName = null;

        var searchPaths = new List<string>();
        foreach (var provinceFileData in provinceFilesData)
        {
            if (provinceFileData.Category.ToUpper() == "TRCAPPIN") tracingName = provinceFileData.Name.Trim().ToUpper();
            if (provinceFileData.Category.ToUpper() == "INTAPPIN") interceptionName = provinceFileData.Name.Trim().ToUpper();
            if (provinceFileData.Category.ToUpper() == "LICAPPIN") licenceName = provinceFileData.Name.Trim().ToUpper();

            string thisPath = provinceFileData.Path.ToUpper();
            if (!searchPaths.Contains(thisPath))
                searchPaths.Add(thisPath);
        }

        var apiAction = new APIBrokerHelper(currentSubmitter: "MSGBRO", currentUser: "MSGBRO");
        var apiRootData = configuration.GetSection("APIroot").Get<ApiConfig>();
        var provincialFileManager = new IncomingProvincialFile(fileBrokerDB, apiRootData, apiAction,
                                                               interceptionBaseName: interceptionName,
                                                               tracingBaseName: tracingName,
                                                               licencingBaseName: licenceName);

        var allNewFiles = new List<string>();
        foreach (string searchPath in searchPaths)
            provincialFileManager.AddNewFiles(searchPath, ref allNewFiles);

        if (allNewFiles.Count > 0)
        {
            ColourConsole.WriteEmbeddedColorLine($"Found [green]{allNewFiles.Count}[/green] file(s)");

            foreach (var newFile in allNewFiles)
            {
                ColourConsole.WriteEmbeddedColorLine($"Processing [green]{newFile}[/green]...");

                var errors = new List<string>();
                provincialFileManager.ProcessNewFile(newFile, ref errors);
                if (errors.Any())
                    foreach (var error in errors)
                        errorTrackingDB.MessageBrokerError($"{provinceCode} APPIN", newFile, new Exception(error), displayExceptionError: true);
            }
        }
        else
            ColourConsole.WriteEmbeddedColorLine("[yellow]No new files found.[/yellow]");

        // Console.ReadKey();

    }

    private static string GetProvinceCodeBasedOnArgs(string[] args) => (args.Length > 0) ? args[0]?.ToUpper() : "ALL";

    private static List<FileTableData> GetFileTableDataBasedOnArgs(string[] args, DBFileTable fileTable)
    {
        var fileTableData = new List<FileTableData>();

        bool loadAllCategories = true;
        if (args.Length > 1)
        {
            string option = args[1].ToUpper();
            switch (option)
            {
                case "TRACE_ONLY":
                    fileTableData.AddRange(fileTable.GetFileTableDataForCategory("TRCAPPIN"));
                    loadAllCategories = false;
                    break;
                case "INTERCEPTION_ONLY":
                    fileTableData.AddRange(fileTable.GetFileTableDataForCategory("INTAPPIN"));
                    loadAllCategories = false;
                    break;
                case "LICENCE_ONLY":
                    fileTableData.AddRange(fileTable.GetFileTableDataForCategory("LICAPPIN"));
                    loadAllCategories = false;
                    break;
                default:
                    break;
            }
        }
        if (loadAllCategories)
        {
            fileTableData.AddRange(fileTable.GetFileTableDataForCategory("TRCAPPIN"));
            fileTableData.AddRange(fileTable.GetFileTableDataForCategory("INTAPPIN"));
            fileTableData.AddRange(fileTable.GetFileTableDataForCategory("LICAPPIN"));
        }

        fileTableData = fileTableData.Where(f => f.Active.HasValue && f.Active.Value).ToList();

        return fileTableData;
    }
}
