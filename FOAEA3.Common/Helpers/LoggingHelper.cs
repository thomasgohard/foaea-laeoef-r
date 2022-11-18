using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;

namespace FOAEA3.Common.Helpers
{
    public static class LoggingHelper
    {
        public const string COOKIE_ID = "Identity.Application";

        public static void SetupLogging(string logConnectionString, string apiTableName = "Logs-API")
        {
            LoggerConfiguration logConfig = SetupLogConfiguration(logConnectionString, apiTableName);

            Log.Logger = logConfig.CreateLogger();
        }

        private static LoggerConfiguration SetupLogConfiguration(string logConnectionString, string apiTableName)
        {
            var logConfig = new LoggerConfiguration()
                            .MinimumLevel.Information()
                            .Enrich.FromLogContext()
                            .Enrich.WithMachineName()
                            .Enrich.WithEnvironmentUserName();

            var sqlSinkOpts = new MSSqlServerSinkOptions
            {
                TableName = apiTableName,
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
                new SqlColumn {ColumnName = "EnvironmentUserName", DataType = SqlDbType.VarChar, DataLength = 1024},
                new SqlColumn {ColumnName = "APIpath", DataType = SqlDbType.VarChar, DataLength = 2048}
            };

            // log to SQL Server table

            logConfig = logConfig.WriteTo.MSSqlServer(
                           connectionString: logConnectionString,
                           restrictedToMinimumLevel: LogEventLevel.Information,
                           sinkOptions: sqlSinkOpts,
                           columnOptions: sqlColumnOpts);

            return logConfig;
        }
    }
}
