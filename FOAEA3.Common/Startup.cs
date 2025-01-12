﻿using DBHelper;
using FOAEA3.Common.Filters;
using FOAEA3.Common.Helpers;
using FOAEA3.Data.Base;
using FOAEA3.Data.DB;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Repository;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Security.Claims;
using System.Text;

namespace FOAEA3.Common
{
    public static class Startup
    {
        public static bool UseInMemoryData(WebApplicationBuilder builder)
        {
            return builder.Configuration["UseInMemoryData"] == "Yes";
        }

        public static void ConfigureAPIServices(IServiceCollection services, IFoaeaConfigurationHelper configuration)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddControllers(options =>
                        {
                            options.Filters.Add(new AuthorizeFilter());
                            options.Filters.Add(new ActionAutoLoggerFilter());
                            options.Filters.Add(new ActionProcessHeadersFilter());
                        })
                    .AddNewtonsoftJson(options =>
                            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                        );

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                        {
                            options.TokenValidationParameters = new TokenValidationParameters()
                            {
                                ValidIssuer = configuration.Tokens.Issuer,
                                ValidAudience = configuration.Tokens.Audience,
                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.Tokens.Key)),
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

            services.AddAuthorization(options =>
                        {
                            options.AddPolicy(Policies.AdminOnlyAccess,
                                        policy => policy.RequireClaim(ClaimTypes.Role,
                                                                        Roles.Admin));

                            options.AddPolicy(Policies.ApplicationReadAccess,
                                        policy => policy.RequireClaim(ClaimTypes.Role,
                                                                        Roles.Admin,
                                                                        Roles.FlasUser,
                                                                        Roles.EnforcementOffice,
                                                                        Roles.EnforcementOfficeReadOnly,
                                                                        Roles.EnforcementService,
                                                                        Roles.EnforcementServiceReadOnly,
                                                                        Roles.CourtUser));

                            options.AddPolicy(Policies.ApplicationModifyAccess,
                                        policy => policy.RequireClaim(ClaimTypes.Role,
                                                                        Roles.Admin,
                                                                        Roles.FlasUser,
                                                                        Roles.EnforcementOffice,
                                                                        Roles.EnforcementService,
                                                                        Roles.CourtUser));
                        });
        }

        public static void ConfigureAPI(WebApplication app, IWebHostEnvironment env, IFoaeaConfigurationHelper configuration, string apiName)
        {
            ColourConsole.WriteEmbeddedColorLine($"Starting [cyan]{apiName}[/cyan]...");
            ColourConsole.WriteEmbeddedColorLine($"Using .Net Code Environment = [yellow]{env.EnvironmentName}[/yellow]");

            Log.Information("Starting API {apiName}", apiName);
            Log.Information("Using .Net Code Environment = {ASPNETCORE_ENVIRONMENT}", env.EnvironmentName);
            Log.Information("Machine Name = {MachineName}", Environment.MachineName);

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

        }

        public static void AddDBServices(IServiceCollection services, string connectionString)
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
            services.AddScoped<IIVRRepository>(m => ActivatorUtilities.CreateInstance<DBIVR>(m, mainDB));

            Log.Information("Using MainDB = {MainDB}", mainDB.ConnectionString);
            ColourConsole.WriteEmbeddedColorLine($"Using Connection: [yellow]{mainDB.ConnectionString}[/yellow]");
        }

        public static async Task AddReferenceDataFromDB(IServiceProvider services)
        {
            Console.WriteLine("Loading Reference Data");

            using IServiceScope serviceScope = services.CreateScope();
            var provider = serviceScope.ServiceProvider;
            var repositories = provider.GetRequiredService<IRepositories>();

            var loadRefFoaEvents = ReferenceData.Instance().LoadFoaEvents(new DBFoaMessage(repositories.MainDB));
            var loadRefActiveStatus = ReferenceData.Instance().LoadActiveStatuses(new DBActiveStatus(repositories.MainDB));
            var loadRefGenders = ReferenceData.Instance().LoadGenders(new DBGender(repositories.MainDB));
            var loadRefProvinces = ReferenceData.Instance().LoadProvinces(new DBProvince(repositories.MainDB));
            var loadRefMediums = ReferenceData.Instance().LoadMediums(new DBMedium(repositories.MainDB));
            var loadRefLanguages = ReferenceData.Instance().LoadLanguages(new DBLanguage(repositories.MainDB));
            var loadRefDocTypes = ReferenceData.Instance().LoadDocumentTypes(new DBDocumentType(repositories.MainDB));
            var loadRefCountries = ReferenceData.Instance().LoadCountries(new DBCountry(repositories.MainDB));
            var loadRefAppReasons = ReferenceData.Instance().LoadApplicationReasons(new DBApplicationReason(repositories.MainDB));
            var loadRefAppCategories = ReferenceData.Instance().LoadApplicationCategories(new DBApplicationCategory(repositories.MainDB));
            var loadRefAppLifeStates = ReferenceData.Instance().LoadApplicationLifeStates(new DBApplicationLifeState(repositories.MainDB));
            var loadRefAppComments = ReferenceData.Instance().LoadApplicationComments(new DBApplicationComments(repositories.MainDB));

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

        }

        public static async Task SetupAndRun(string[] args, Action<IServiceCollection> SetupDataOverride = null)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.ClearProviders();

            var localConfig = builder.Configuration;
            var env = builder.Environment;
            var apiName = env.ApplicationName;

            var config = new FoaeaConfigurationHelper(args);

            LoggingHelper.SetupLogging(config.FoaeaConnection);

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

            if (!Startup.UseInMemoryData(builder))
                await Startup.AddReferenceDataFromDB(app.Services);

            var api_url = localConfig["Urls"];

            ColourConsole.WriteEmbeddedColorLine($"[green]Waiting for API calls...[/green] [yellow]{api_url}[/yellow]\n");

            app.Run();
        }
    }
}
