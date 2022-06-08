using CompareOldAndNewData.CommandLine;
using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Resources.Helpers;
using Microsoft.Extensions.Configuration;

Console.WriteLine("Comparing Data From Old System with new System");
Console.WriteLine("----------------------------------------------");

string? aspnetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

IConfiguration configuration = new ConfigurationBuilder()
        .AddJsonFile($"appsettings.json", optional: true)
        .AddJsonFile($"appsettings.{aspnetCoreEnvironment}.json", optional: true)
        .Build();

var foaea2DB = new DBTools(configuration.GetConnectionString("Foaea2DB").ReplaceVariablesWithEnvironmentValues());
var foaea3DB = new DBTools(configuration.GetConnectionString("Foaea3DB").ReplaceVariablesWithEnvironmentValues());
var repositories2 = new DbRepositories(foaea2DB);
var repositories3 = new DbRepositories(foaea3DB);
var repositories2Finance = new DbRepositories_Finance(foaea2DB);
var repositories3Finance = new DbRepositories_Finance(foaea3DB);

var foaea2RunDate = (new DateTime(2022, 5, 25)).Date;
var foaea3RunDate = (new DateTime(2022, 6, 6)).Date;

CompareAll.Run(repositories2, repositories2Finance, repositories3, repositories3Finance, "ON01", "P02862", foaea2RunDate, foaea3RunDate);
CompareAll.Run(repositories2, repositories2Finance, repositories3, repositories3Finance, "ON01", "O88291", foaea2RunDate, foaea3RunDate);
CompareAll.Run(repositories2, repositories2Finance, repositories3, repositories3Finance, "ON01", "P02001", foaea2RunDate, foaea3RunDate);
CompareAll.Run(repositories2, repositories2Finance, repositories3, repositories3Finance, "ON01", "P85061", foaea2RunDate, foaea3RunDate);

Console.WriteLine("\nFinished");

