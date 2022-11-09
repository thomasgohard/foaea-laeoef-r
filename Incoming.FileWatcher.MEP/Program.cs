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
using System.Threading.Tasks;

namespace Incoming.FileWatcher.MEP;

internal class Program
{
    static async Task Main(string[] args)
    {
        string aspnetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{aspnetCoreEnvironment}.json", optional: true, reloadOnChange: true)
            .AddCommandLine(args);

        IConfiguration configuration = builder.Build();

        var fileBrokerDB = new DBToolsAsync(configuration.GetConnectionString("FileBroker").ReplaceVariablesWithEnvironmentValues());
        var requestLogDB = new DBRequestLog(fileBrokerDB);
        await requestLogDB.DeleteAllAsync();

        string provinceCode = GetProvinceCodeBasedOnArgs(args);
        var fileTableData = await GetFileTableDataBasedOnArgsAsync(args, new DBFileTable(fileBrokerDB));

        var errorTrackingDB = new DBErrorTracking(fileBrokerDB);

        if (!fileTableData.Any())
        {
            string error = $"No items found in FileTable?";
            ColourConsole.WriteEmbeddedColorLine(error);
            await errorTrackingDB.MessageBrokerErrorAsync($"Incoming MEP File Processing", "Error starting MEP File Monitor",
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
            await errorTrackingDB.MessageBrokerErrorAsync($"Incoming MEP File Processing", "Error starting MEP File Monitor",
                                                          new Exception(error), displayExceptionError: true);
            return;
        }

        DateTime start = DateTime.Now;
        ColourConsole.WriteEmbeddedColorLine($"Starting [cyan]{provinceCode}[/cyan] MEP File Monitor");
        ColourConsole.WriteEmbeddedColorLine($"Starting time [orange]{start}[/orange]");

        var provinceFilesData = fileTableData.Where(f => (f.Name[..2].ToUpper() == provinceCode) || (provinceCode == "ALL"))
                                             .ToList();
        var provinces = (from data in provinceFilesData
                         select data.Name[..2]).Distinct().ToList();

        var apiBroker = new APIBrokerHelper(currentSubmitter: "MSGBRO", currentUser: "MSGBRO");
        var apiRootData = configuration.GetSection("APIroot").Get<ApiConfig>();

        string tracingName = null;
        string interceptionName = null;
        string licenceName = null;

        int totalFilesFound = 0;
        foreach (var itemProvince in provinces)
        {
            var searchPaths = new List<string>();

            var thisProvinceFilesData = provinceFilesData.Where(f => f.Name[..2].ToUpper() == itemProvince.ToUpper());
            foreach (var provinceFileData in thisProvinceFilesData)
            {
                if (provinceFileData.Category.ToUpper() == "TRCAPPIN") tracingName = provinceFileData.Name.Trim().ToUpper();
                if (provinceFileData.Category.ToUpper() == "INTAPPIN") interceptionName = provinceFileData.Name.Trim().ToUpper();
                if (provinceFileData.Category.ToUpper() == "LICAPPIN") licenceName = provinceFileData.Name.Trim().ToUpper();

                string thisPath = provinceFileData.Path.ToUpper();
                if (!searchPaths.Contains(thisPath))
                    searchPaths.Add(thisPath);
            }

            var fileTable = new DBFileTable(fileBrokerDB);
            var provincialFileManager = new IncomingProvincialFile(fileTable, apiRootData, apiBroker,
                                                                   interceptionBaseName: interceptionName,
                                                                   tracingBaseName: tracingName,
                                                                   licencingBaseName: licenceName);

            bool finishedForProvince = false;
            bool displayFoundZero = true;
            while (!finishedForProvince)
            {
                var allNewFiles = new List<string>();
                foreach (string searchPath in searchPaths)
                    await provincialFileManager.AddNewFilesAsync(searchPath, allNewFiles);
                totalFilesFound += allNewFiles.Count;

                if (allNewFiles.Count > 0)
                {
                    displayFoundZero = false;
                    string moreThanOne = allNewFiles.Count > 1 ? "s" : "";
                    ColourConsole.WriteEmbeddedColorLine($"Found [green]{allNewFiles.Count}[/green] file{moreThanOne} in [green]{itemProvince}[/green]");

                    foreach (var newFile in allNewFiles)
                    {
                        ColourConsole.WriteEmbeddedColorLine($"Processing [green]{newFile}[/green]...");

                        var errors = new List<string>();
                        string userName = configuration["FILE_BROKER:userName"].ReplaceVariablesWithEnvironmentValues();
                        string userPassword = configuration["FILE_BROKER:userPassword"].ReplaceVariablesWithEnvironmentValues();
                        
                        await provincialFileManager.ProcessNewFileAsync(newFile, errors, userName, userPassword);
                        
                        if (errors.Any())
                        {
                            foreach (var error in errors)
                                await errorTrackingDB.MessageBrokerErrorAsync($"{provinceCode} APPIN", newFile, new Exception(error), displayExceptionError: true);
                            finishedForProvince = true;
                        }
                    }
                }
                else
                {
                    finishedForProvince = true;
                    if (displayFoundZero)
                        ColourConsole.WriteEmbeddedColorLine($"Found [green]0[/green] file in [green]{itemProvince}[/green]");
                }
            }

        }

        ColourConsole.WriteEmbeddedColorLine($"Completed. [yellow]{totalFilesFound}[/yellow] processed.");

        DateTime end = DateTime.Now;
        var duration = end - start;

        ColourConsole.WriteEmbeddedColorLine($"Completion time [orange]{end}[/orange] (duration: [yellow]{duration.Minutes}[/yellow] minutes)");

        Console.ReadKey();

    }

    private static string GetProvinceCodeBasedOnArgs(string[] args) => (args.Length > 0) ? args[0]?.ToUpper() : "ALL";

    private static async Task<List<FileTableData>> GetFileTableDataBasedOnArgsAsync(string[] args, DBFileTable fileTable)
    {
        var fileTableData = new List<FileTableData>();

        bool loadAllCategories = true;
        if (args.Length > 1)
        {
            string option = args[1].ToUpper();
            switch (option)
            {
                case "TRACE_ONLY":
                    fileTableData.AddRange(await fileTable.GetFileTableDataForCategoryAsync("TRCAPPIN"));
                    loadAllCategories = false;
                    break;
                case "INTERCEPTION_ONLY":
                    fileTableData.AddRange(await fileTable.GetFileTableDataForCategoryAsync("INTAPPIN"));
                    loadAllCategories = false;
                    break;
                case "LICENCE_ONLY":
                    fileTableData.AddRange(await fileTable.GetFileTableDataForCategoryAsync("LICAPPIN"));
                    loadAllCategories = false;
                    break;
                default:
                    break;
            }
        }
        if (loadAllCategories)
        {
            fileTableData.AddRange(await fileTable.GetFileTableDataForCategoryAsync("TRCAPPIN"));
            fileTableData.AddRange(await fileTable.GetFileTableDataForCategoryAsync("INTAPPIN"));
            fileTableData.AddRange(await fileTable.GetFileTableDataForCategoryAsync("LICAPPIN"));
        }

        fileTableData = fileTableData.Where(f => f.Active.HasValue && f.Active.Value).ToList();

        return fileTableData;
    }
}
