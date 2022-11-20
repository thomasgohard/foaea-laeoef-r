using FileBroker.Common.Filters;
using FOAEA3.Common.Helpers;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileBroker.Common
{
    public static class Startup
    {
        public static void ConfigureAPIServices(IServiceCollection services, FileBrokerConfigurationHelper config, string apiName)
        {
            LoggingHelper.SetupLogging(config.FileBrokerConnection, "Logs-API-FileBroker");

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
                            ValidIssuer = config.Tokens.Issuer,
                            ValidAudience = config.Tokens.Audience,
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(config.Tokens.Key)),
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

            services.AddEndpointsApiExplorer();

            DataHelper.ConfigureDBServices(services, config.FileBrokerConnection);
        }

        public static void ConfigureAPI(WebApplication app, IWebHostEnvironment env, FileBrokerConfigurationHelper config, string apiName, string url)
        {
            ColourConsole.WriteEmbeddedColorLine($"Starting [cyan]{apiName}[/cyan]...");
            ColourConsole.WriteEmbeddedColorLine($"Using .Net Code Environment = [yellow]{env.EnvironmentName}[/yellow]");
            ColourConsole.WriteEmbeddedColorLine($"Using Connection: [yellow]{config.FileBrokerConnection}[/yellow]");

            Log.Information("Starting API {apiName}", apiName);
            Log.Information("Using .Net Code Environment = {ASPNETCORE_ENVIRONMENT}", env.EnvironmentName);
            Log.Information("Machine Name = {MachineName}", Environment.MachineName);
            Log.Information("Connection = {fileBrokerCON}", config.FileBrokerConnection);

            string currentServer = Environment.MachineName;
            var prodServers = config.ProductionServers;

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

            ColourConsole.WriteEmbeddedColorLine($"\n[green]Waiting for API calls...[/green][yellow]{url}[/yellow]\n");

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
        }

    }
}
