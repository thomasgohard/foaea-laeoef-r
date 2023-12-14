using DBHelper;
//using FOAEA3.Common.Filters;
//using FOAEA3.Common.Helpers;
using FOAEA3.Data.Base;
using FOAEA3.Data.DB;
using FOAEA3.IVR.Helpers;
using FOAEA3.Model.Constants;
using FOAEA3.IVR.Interfaces;
using FOAEA3.IVR.Interfaces.Repository;
using FOAEA3.IVR.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Xml.Linq;

namespace FOAEA3.IVR
{
    public static class Startup
    {
        public static bool UseInMemoryData(WebApplicationBuilder builder)
        {
            return builder.Configuration["UseInMemoryData"] == "Yes";
        }

        public static void ConfigureAPIServices(IServiceCollection services, IVRConfigurationHelper configuration)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddControllers(options =>
            {
                options.Filters.Add(new AuthorizeFilter());
               // options.Filters.Add(new ActionAutoLoggerFilter());
               // options.Filters.Add(new ActionProcessHeadersFilter());
            })
                    .AddNewtonsoftJson(options =>
                            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                        );
        }

        public static void ConfigureAPI(WebApplication app, IWebHostEnvironment env, IVRConfigurationHelper configuration, string apiName)
        {
            ColourConsole.WriteEmbeddedColorLine($"Starting [cyan]{apiName}[/cyan]...");
            ColourConsole.WriteEmbeddedColorLine($"Using .Net Code Environment = [yellow]{env.EnvironmentName}[/yellow]");

            //Log.Information("Starting API {apiName}", apiName);
            //Log.Information("Using .Net Code Environment = {ASPNETCORE_ENVIRONMENT}", env.EnvironmentName);
            //Log.Information("Machine Name = {MachineName}", Environment.MachineName);

            string currentServer = Environment.MachineName;

            if (!env.IsEnvironment("Production"))
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", apiName + " v1");
                });

                IdentityModelEventSource.ShowPII = true;
            }
            else if (configuration.ProductionServers.Any(prodServer => prodServer.ToLower() == currentServer.ToLower()))
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
                //Log.Fatal($"Trying to use Production environment on non-production server {currentServer}. Application stopping!", currentServer);
                Console.WriteLine($"Trying to use Production environment on non-production server {currentServer}");
                Console.WriteLine("Application stopping...");

                Task.Delay(2000).Wait();

                app.Lifetime.StopApplication();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }

        public static void AddDBServices(IServiceCollection services, string connectionString)
        {
            var mainDB = new DBToolsAsync(connectionString);
            
            services.AddScoped<IIVRRepository>(m => ActivatorUtilities.CreateInstance<DBIVR>(m, mainDB));

            //Log.Information("Using MainDB = {MainDB}", mainDB.ConnectionString);
            ColourConsole.WriteEmbeddedColorLine($"Using Connection: [yellow]{mainDB.ConnectionString}[/yellow]");
        }

        public static async Task SetupAndRun(string[] args, Action<IServiceCollection> SetupDataOverride = null)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.ClearProviders();

            var localConfig = builder.Configuration;
            var env = builder.Environment;
            var apiName = env.ApplicationName;

            var config = new IVRConfigurationHelper(args);

            //LoggingHelper.SetupLogging(config.FoaeaConnection);

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = apiName, Version = "v1" });
            });

            if (!Startup.UseInMemoryData(builder))
                Startup.AddDBServices(builder.Services, config.FoaeaConnection);
            else if (SetupDataOverride is not null)
                SetupDataOverride(builder.Services);

            Startup.ConfigureAPIServices(builder.Services, config);

            var app = builder.Build();

            Startup.ConfigureAPI(app, env, config, apiName);

  
            var api_url = localConfig["Urls"];

            ColourConsole.WriteEmbeddedColorLine($"[green]Waiting for API calls...[/green] [yellow]{api_url}[/yellow]\n");

            app.Run();
        }
    }
}
