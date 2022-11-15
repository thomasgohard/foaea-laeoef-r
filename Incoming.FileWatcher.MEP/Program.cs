using DBHelper;
using FileBroker.Data.DB;
using FileBroker.Model;
using FOAEA3.Common.Brokers;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Structs;
using FOAEA3.Resources.Helpers;
using Incoming.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
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

        string provinceCode = args.Any() ? args.First()?.ToUpper() : "ALL";
        var filesToProcess = await GetFileTableDataForArgsOrAllAsync(args, new DBFileTable(fileBrokerDB));

        var errorTrackingDB = new DBErrorTracking(fileBrokerDB);

        if (!filesToProcess.Any())
        {
            await GenerateError(errorTrackingDB, "No items found in FileTable?");
            return;
        }

        var provinces = GetSelectedProvinceCodes(filesToProcess);

        if ((string.IsNullOrEmpty(provinceCode) || !provinces.Contains(provinceCode)) && (provinceCode != "ALL"))
        {
            await GenerateError(errorTrackingDB, $"Invalid province argument on command line: [{provinceCode}]\nMust be one of: " +
                                                 string.Join(", ", provinces + " or ALL"));
            return;
        }

        DateTime start = DateTime.Now;
        ColourConsole.WriteEmbeddedColorLine($"Starting [cyan]{provinceCode}[/cyan] MEP File Monitor");
        ColourConsole.WriteEmbeddedColorLine($"Starting time [orange]{start}[/orange]");

        var apiBroker = new APIBrokerHelper(currentSubmitter: "MSGBRO", currentUser: "System_Support");
        var apiRootData = configuration.GetSection("APIroot").Get<ApiConfig>();

        string token = "";
        var apiApplHelper = new APIBrokerHelper(apiRootData.FoaeaApplicationRootAPI, currentSubmitter: "MSGBRO", currentUser: "System_Support");
        var applicationApplicationAPIs = new ApplicationAPIBroker(apiApplHelper, token);
        var productionAuditAPIs = new ProductionAuditAPIBroker(apiApplHelper, token);
        var loginAPIs = new LoginsAPIBroker(apiApplHelper, token);

        var apiInterceptionApplHelper = new APIBrokerHelper(apiRootData.FoaeaInterceptionRootAPI, currentSubmitter: "MSGBRO", currentUser: "System_Support");
        var interceptionApplicationAPIs = new InterceptionApplicationAPIBroker(apiInterceptionApplHelper, token);

        var foaeaApis = new APIBrokerList
        {
            Applications = applicationApplicationAPIs,
            InterceptionApplications = interceptionApplicationAPIs,
            ProductionAudits = productionAuditAPIs,
            Accounts = loginAPIs
        };

        int totalFilesFound = 0;
        foreach (var itemProvince in provinces)
        {
            string provCode = itemProvince.ToUpper();
            var searchPaths = GetSearchPaths(filesToProcess, out FileBaseName fileBaseName, provCode);

            var fileTable = new DBFileTable(fileBrokerDB);
            var provincialFileManager = new IncomingProvincialFile(fileTable, apiRootData, apiBroker, fileBaseName, foaeaApis);

            bool foundZero = true;

            var processedFiles = new List<string>();

            bool finishedForProvince = false;
            while (!finishedForProvince)
            {
                var foaeaLoginData = new FoaeaLoginData
                {
                    UserName = configuration["FOAEA:userName"].ReplaceVariablesWithEnvironmentValues(),
                    Password = configuration["FOAEA:userPassword"].ReplaceVariablesWithEnvironmentValues(),
                    Submitter = configuration["FOAEA:submitter"].ReplaceVariablesWithEnvironmentValues()
                };

                var allNewFiles = await provincialFileManager.GetWaitingFiles(searchPaths, foaeaLoginData);

                if (allNewFiles.Any())
                {
                    foundZero = false;
                    string moreThanOne = allNewFiles.Count > 1 ? "s" : "";

                    if (!processedFiles.Contains(allNewFiles.First()))
                    {
                        ColourConsole.WriteEmbeddedColorLine($"Found [green]{allNewFiles.Count}[/green] file{moreThanOne} in [green]{itemProvince}[/green]");
                        totalFilesFound += allNewFiles.Count;
                    }

                    foreach (var newFile in allNewFiles)
                    {
                        if (processedFiles.Contains(newFile))
                        {
                            finishedForProvince = true;
                            break;
                        }
                        processedFiles.Add(newFile);

                        ColourConsole.WriteEmbeddedColorLine($"Processing [green]{newFile}[/green]...");

                        var errors = new List<string>();
                        string fileBrokerUserName = configuration["FILE_BROKER:userName"].ReplaceVariablesWithEnvironmentValues();
                        string fileBrokerUserPassword = configuration["FILE_BROKER:userPassword"].ReplaceVariablesWithEnvironmentValues();

                        await provincialFileManager.ProcessWaitingFile(newFile, errors, fileBrokerUserName, fileBrokerUserPassword);

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
                    if (foundZero)
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


    private static async Task GenerateError(DBErrorTracking errorTrackingDB, string error)
    {
        ColourConsole.WriteEmbeddedColorLine(error);
        await errorTrackingDB.MessageBrokerErrorAsync($"Incoming MEP File Processing", "Error starting MEP File Monitor",
                                                      new Exception(error), displayExceptionError: true);
    }

    private static List<string> GetSearchPaths(List<FileTableData> filesToProcess, out FileBaseName fileBaseName, string provinceCode)
    {
        var searchPaths = new List<string>();
        fileBaseName = new FileBaseName();

        var thisProvinceFilesData = filesToProcess.Where(f => f.Name[..2].ToUpper() == provinceCode);
        string interceptionPath = string.Empty;
        foreach (var provinceFileData in thisProvinceFilesData)
        {
            string category = provinceFileData.Category.ToUpper().Trim();

            switch (category)
            {
                case "TRCAPPIN":
                    fileBaseName.Tracing = provinceFileData.Name.Trim().ToUpper();
                    break;
                case "INTAPPIN":
                    fileBaseName.Interception = provinceFileData.Name.Trim().ToUpper();
                    interceptionPath = provinceFileData.Path.ToUpper();
                    break;
                case "ESD":
                    fileBaseName.ESD = provinceFileData.Name.Trim().ToUpper();
                    break;
                case "LICAPPIN":
                    fileBaseName.Licencing = provinceFileData.Name.Trim().ToUpper();
                    break;
                default:
                    // ignore all other categories
                    break;
            }

            string thisPath = provinceFileData.Path.ToUpper();
            if ((!searchPaths.Contains(thisPath)) && (category != "ESD"))
                searchPaths.Add(thisPath);
        }

        if (!string.IsNullOrEmpty(interceptionPath))
            searchPaths.Add(interceptionPath.AppendPath("ESD"));

        return searchPaths;
    }

    private static List<string> GetSelectedProvinceCodes(List<FileTableData> fileTableData)
    {
        return fileTableData.Where(f => f.Active.HasValue && f.Active.Value)
                            .GroupBy(f => f.Name[..2].ToUpper())
                            .Select(group => group.First().Name[..2].ToUpper())
                            .ToList();
    }

    private static async Task<List<FileTableData>> GetFileTableDataForArgsOrAllAsync(string[] args, DBFileTable fileTable)
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
                    fileTableData.AddRange(await fileTable.GetFileTableDataForCategoryAsync("ESD"));
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
            fileTableData.AddRange(await fileTable.GetFileTableDataForCategoryAsync("ESD"));
            fileTableData.AddRange(await fileTable.GetFileTableDataForCategoryAsync("LICAPPIN"));
        }

        fileTableData = fileTableData.Where(f => f.Active.HasValue && f.Active.Value).ToList();

        return fileTableData;
    }

}


