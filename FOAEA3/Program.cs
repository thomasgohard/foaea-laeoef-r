using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog.Events;
using System.Data;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using Serilog.Sinks.MSSqlServer.Sinks.MSSqlServer.Options;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using FOAEA3.Resources.Helpers;

namespace FOAEA3
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            string aspnetCoreEnvironment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{aspnetCoreEnvironment}.json", optional: true, reloadOnChange: true)
                .Build();

            string logPath = config["FileLogPath"];
            LoggerConfiguration logConfig = SetupLogConfiguration(config);

            // also log to a text file (if path is specified in appsettings

            if (!string.IsNullOrEmpty(logPath))
                logConfig = logConfig.WriteTo.File(logPath,
                                                   rollingInterval: RollingInterval.Day,
                                                   outputTemplate: "{Timestamp} {Message}{NewLine:1}{Exception:1}");

            Log.Logger = logConfig.CreateLogger();

            try
            {
                Log.Information("Application Starting.");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "The Application failed to start.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static LoggerConfiguration SetupLogConfiguration(IConfigurationRoot config)
        {
            var logConfig = new LoggerConfiguration()
                            .MinimumLevel.Information()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                            .MinimumLevel.Override("System", LogEventLevel.Warning)
                            .Enrich.FromLogContext()
                            .Enrich.WithMachineName()
                            .Enrich.WithEnvironmentUserName();

            var sqlSinkOpts = new MSSqlServerSinkOptions
            {
                TableName = "Logs",
                SchemaName = "dbo",
                AutoCreateSqlTable = false
            };

            var sqlColumnOpts = new ColumnOptions();
            sqlColumnOpts.Store.Remove(StandardColumn.MessageTemplate);
            sqlColumnOpts.AdditionalColumns = new Collection<SqlColumn>
            {
                new SqlColumn {ColumnName = "SubmitterCode", DataType = SqlDbType.Char, DataLength = 6},
                new SqlColumn {ColumnName = "MachineName", DataType = SqlDbType.VarChar, DataLength = 50},
                new SqlColumn {ColumnName = "ActionName", DataType = SqlDbType.VarChar, DataLength = 1024},
                new SqlColumn {ColumnName = "EnvironmentUserName", DataType = SqlDbType.VarChar, DataLength = 1024}
            };

            // log to SQL Server table

            logConfig = logConfig.WriteTo.MSSqlServer(
                           connectionString: config["ConnectionStrings:FOAEAMain"].ReplaceVariablesWithEnvironmentValues(),
                           restrictedToMinimumLevel: LogEventLevel.Warning,
                           sinkOptions: sqlSinkOpts,
                           columnOptions: sqlColumnOpts);

            // also log to the Windows Event Viewer

            logConfig = logConfig.WriteTo.EventLog(source: "FOAEA3",
                                  logName: "Application",
                                  manageEventSource: true,
                                  restrictedToMinimumLevel: LogEventLevel.Warning);
            return logConfig;
        }
    }
}
