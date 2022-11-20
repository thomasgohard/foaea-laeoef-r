using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace FOAEA3.Common.Helpers
{
    public class FoaeaConfigurationHelper
    {
        public string FoaeaConnection { get; }

        public RecipientsConfig RecipientsConfig { get; }
        public TokenConfig Tokens { get; }

        public List<string> ProductionServers { get; }

        public FoaeaConfigurationHelper(string[] args = null)
        {
            string aspnetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("FileBrokerConfiguration.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"FileBrokerConfiguration.{aspnetCoreEnvironment}.json", optional: true, reloadOnChange: true);

            if (args is not null)
                builder = builder.AddCommandLine(args);

            IConfiguration configuration = builder.Build();

            configuration.GetConnectionString("FOAEAMain").ReplaceVariablesWithEnvironmentValues();

            RecipientsConfig = configuration.GetSection("CustomConfig").Get<RecipientsConfig>();
            Tokens = configuration.GetSection("Tokens").Get<TokenConfig>();

            ProductionServers = configuration.GetSection("ProductionServers").Get<List<string>>();
        }
    }
}
