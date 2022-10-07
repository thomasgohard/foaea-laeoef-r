using FileBroker.Common.Filters;
using FileBroker.Model;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBroker.Common
{
    public static class Startup
    {
        public static void ConfigureAPIServices(IServiceCollection services, IConfiguration configuration, string apiName)
        {
            LoggingHelper.SetupLogging(configuration, "FILEBROKER-API", "FileBroker", "Logs-API-FileBroker");
            
            services.Configure<TokenConfig>(configuration.GetSection("Tokens"));

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = apiName, Version = "v1" });
            });

            services.AddControllers(options =>
            {
                options.ReturnHttpNotAcceptable = true;
                options.RespectBrowserAcceptHeader = true;
                options.Filters.Add(new AuthorizeFilter());
                options.Filters.Add(new ActionAutoLoggerFilter());
            }).AddXmlSerializerFormatters();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters()
                        {
                            ValidIssuer = configuration["Tokens:Issuer"].ReplaceVariablesWithEnvironmentValues(),
                            ValidAudience = configuration["Tokens:Audience"].ReplaceVariablesWithEnvironmentValues(),
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(configuration["Tokens:Key"].ReplaceVariablesWithEnvironmentValues())),
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters) =>
                            {
                                var clonedParameters = validationParameters.Clone();
                                clonedParameters.LifetimeValidator = null;
                                bool valid = expires >= DateTime.Now;
                                return valid;
                            }
                        };
                    });
            //.AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters()
            //    {
            //        ValidIssuer = configuration["Tokens:Issuer"].ReplaceVariablesWithEnvironmentValues(),
            //        ValidAudience = configuration["Tokens:Audience"].ReplaceVariablesWithEnvironmentValues(),
            //        IssuerSigningKey = new SymmetricSecurityKey(
            //            Encoding.UTF8.GetBytes(configuration["Tokens:Key"].ReplaceVariablesWithEnvironmentValues()))
            //    };
            //});

            services.AddEndpointsApiExplorer();
            services.Configure<ProvincialAuditFileConfig>(configuration.GetSection("AuditConfig"));
            services.Configure<ApiConfig>(configuration.GetSection("APIroot"));

            string fileBrokerCON = configuration.GetConnectionString("FileBroker").ReplaceVariablesWithEnvironmentValues();

            DataHelper.ConfigureDBServices(services, fileBrokerCON);
        }

        public static void ConfigureAPI(WebApplication app, IWebHostEnvironment env, IConfiguration configuration, string apiName)
        {
            string fileBrokerCON = configuration.GetConnectionString("FileBroker").ReplaceVariablesWithEnvironmentValues();

            ColourConsole.WriteEmbeddedColorLine($"Starting [cyan]{apiName}[/cyan]...");
            ColourConsole.WriteEmbeddedColorLine($"Using .Net Code Environment = [yellow]{env.EnvironmentName}[/yellow]");
            ColourConsole.WriteEmbeddedColorLine($"Using Connection: [yellow]{fileBrokerCON}[/yellow]");

            Log.Information("Starting API {apiName}", apiName);
            Log.Information("Using .Net Code Environment = {ASPNETCORE_ENVIRONMENT}", env.EnvironmentName);
            Log.Information("Machine Name = {MachineName}", Environment.MachineName);
            Log.Information("Connection = {fileBrokerCON}", fileBrokerCON);

            string currentServer = Environment.MachineName;
            var prodServersSection = configuration.GetSection("ProductionServers");
            var prodServers = prodServersSection.Get<List<string>>();
            for (int i = 0; i < prodServers.Count; i++)
                prodServers[i] = prodServers[i].ReplaceVariablesWithEnvironmentValues();

            if (!env.IsEnvironment("Production"))
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else if (prodServers.Any(prodServer => prodServer.ToLower() == currentServer.ToLower()))
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. Try again later");
                    });
                });
                app.UseHsts();
            }
            else
            {

                Log.Fatal("Trying to use Production environment on non-production server {currentServer}. Application stopping!", currentServer);
                Console.WriteLine($"Trying to use Production environment on non-production server {currentServer}");
                Console.WriteLine("Application stopping...");

                Task.Delay(2000).Wait();

                app.Lifetime.StopApplication();
            }

            var api_url = configuration["Urls"];
            ColourConsole.WriteEmbeddedColorLine($"\n[green]Waiting for API calls...[/green][yellow]{api_url}[/yellow]\n");

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
        }
                
    }
}
