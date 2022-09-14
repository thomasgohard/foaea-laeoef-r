using DBHelper;
using FOAEA3.Common.Filters;
using FOAEA3.Data.Base;
using FOAEA3.Data.DB;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.Common
{
    public static class Startup
    {
        public static void ConfigureAPIServices(IServiceCollection services, IConfiguration configuration)
        {
            AddDBServices(services, configuration.GetConnectionString("FOAEAMain").ReplaceVariablesWithEnvironmentValues());

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.Configure<CustomConfig>(configuration.GetSection("CustomConfig"));

            services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo(@"c:\FOAEA"))
                    .SetApplicationName("SharedCookieApp");

            services.AddAuthentication("Identity.Application")
                    .AddCookie("Identity.Application", options =>
                        {
                            options.Cookie.Name = ".AspNet.SharedCookie";
                        });

            // TODO: replace with JWT authentication or something similar
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //        .AddCookie(); 

            services.AddControllers(options =>
                        {
                            options.ReturnHttpNotAcceptable = true;
                            options.RespectBrowserAcceptHeader = true;
                            options.Filters.Add(new AuthorizeFilter());
                            options.Filters.Add(new ActionAutoLoggerFilter());
                            options.Filters.Add(new ActionProcessHeadersFilter());
                        })
                    .AddXmlSerializerFormatters()
                    .AddNewtonsoftJson(options =>
                            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                        );
        }

        public static async Task ConfigureAPI(WebApplication app, IWebHostEnvironment env, IConfiguration configuration, string apiName)
        {
            ColourConsole.WriteEmbeddedColorLine($"Starting [cyan]{apiName}[/cyan]...");
            ColourConsole.WriteEmbeddedColorLine($"Using .Net Code Environment = [yellow]{env.EnvironmentName}[/yellow]");

            Log.Information("Starting API {apiName}", apiName);
            Log.Information("Using .Net Code Environment = {ASPNETCORE_ENVIRONMENT}", env.EnvironmentName);
            Log.Information("Machine Name = {MachineName}", Environment.MachineName);

            string currentServer = Environment.MachineName;
            var prodServersSection = configuration.GetSection("ProductionServers");
            var prodServers = prodServersSection.Get<List<string>>();
            for (int i = 0; i < prodServers.Count; i++)
                prodServers[i] = prodServers[i].ReplaceVariablesWithEnvironmentValues();

            if (!env.IsEnvironment("Production"))
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", apiName + " v1");
                });
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
                Log.Fatal($"Trying to use Production environment on non-production server {currentServer}. Application stopping!", currentServer);
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

            Console.WriteLine("Loading Reference Data");

            using IServiceScope serviceScope = app.Services.CreateScope();
            var provider = serviceScope.ServiceProvider;
            var repositories = provider.GetRequiredService<IRepositories>();

            var loadRefFoaEvents = ReferenceData.Instance().LoadFoaEventsAsync(new DBFoaMessage(repositories.MainDB));
            var loadRefActiveStatus = ReferenceData.Instance().LoadActiveStatusesAsync(new DBActiveStatus(repositories.MainDB));
            var loadRefGenders = ReferenceData.Instance().LoadGendersAsync(new DBGender(repositories.MainDB));
            var loadRefProvinces = ReferenceData.Instance().LoadProvincesAsync(new DBProvince(repositories.MainDB));
            var loadRefMediums = ReferenceData.Instance().LoadMediumsAsync(new DBMedium(repositories.MainDB));
            var loadRefLanguages = ReferenceData.Instance().LoadLanguagesAsync(new DBLanguage(repositories.MainDB));
            var loadRefDocTypes = ReferenceData.Instance().LoadDocumentTypesAsync(new DBDocumentType(repositories.MainDB));
            var loadRefCountries = ReferenceData.Instance().LoadCountriesAsync(new DBCountry(repositories.MainDB));
            var loadRefAppReasons = ReferenceData.Instance().LoadApplicationReasonsAsync(new DBApplicationReason(repositories.MainDB));
            var loadRefAppCategories = ReferenceData.Instance().LoadApplicationCategoriesAsync(new DBApplicationCategory(repositories.MainDB));
            var loadRefAppLifeStates = ReferenceData.Instance().LoadApplicationLifeStatesAsync(new DBApplicationLifeState(repositories.MainDB));
            var loadRefAppComments = ReferenceData.Instance().LoadApplicationCommentsAsync(new DBApplicationComments(repositories.MainDB));

            await Task.WhenAll(loadRefFoaEvents, loadRefActiveStatus, loadRefGenders,
                               loadRefProvinces, loadRefMediums, loadRefLanguages,
                               loadRefDocTypes, loadRefCountries, loadRefAppReasons,
                               loadRefAppCategories, loadRefAppLifeStates, loadRefAppComments);

            if (ReferenceData.Instance().Messages.Count == 0)
                ColourConsole.WriteLine("Reference Data Loaded Successfully.");
            else
            {
                ColourConsole.WriteLine("Reference Data Failed to Load !!!", ConsoleColor.Red);
                foreach (var message in ReferenceData.Instance().Messages)
                    ColourConsole.WriteLine($"  {message.Description}", ConsoleColor.Red);
            }

            // var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS").Split(";");
            var api_url = configuration["Urls"];

            ColourConsole.WriteEmbeddedColorLine($"[green]Waiting for API calls...[/green] [yellow]{api_url}[/yellow]\n");

        }

        private static void AddDBServices(IServiceCollection services, string connectionString)
        {
            var mainDB = new DBToolsAsync(connectionString);

            services.AddScoped<IDBToolsAsync>(m => ActivatorUtilities.CreateInstance<DBToolsAsync>(m, mainDB)); // used to display the database name at top of page
            services.AddScoped<IRepositories>(m => ActivatorUtilities.CreateInstance<DbRepositories>(m, mainDB));
            services.AddScoped<IRepositories_Finance>(m => ActivatorUtilities.CreateInstance<DbRepositories_Finance>(m, mainDB)); // to access database procs for finance tables
            services.AddScoped<IFoaEventsRepository>(m => ActivatorUtilities.CreateInstance<DBFoaMessage>(m, mainDB));
            services.AddScoped<IActiveStatusRepository>(m => ActivatorUtilities.CreateInstance<DBActiveStatus>(m, mainDB));
            services.AddScoped<IGenderRepository>(m => ActivatorUtilities.CreateInstance<DBGender>(m, mainDB));
            services.AddScoped<IApplicationCommentsRepository>(m => ActivatorUtilities.CreateInstance<DBApplicationComments>(m, mainDB));
            services.AddScoped<IApplicationLifeStateRepository>(m => ActivatorUtilities.CreateInstance<DBApplicationLifeState>(m, mainDB));

            Log.Information("Using MainDB = {MainDB}", mainDB.ConnectionString);
            ColourConsole.WriteEmbeddedColorLine($"Using Connection: [yellow]{mainDB.ConnectionString}[/yellow]");
        }
    }
}
