using BackendProcess.API.Filters;
using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendProcesses.API
{
    public class Startup
    {
        private IConfiguration Config { get; }

        public Startup(IConfiguration configuration)
        {
            this.Config = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ColourConsole.WriteEmbeddedColorLine($"Starting [cyan]BackendProcesses.API[/cyan]...");

            services.AddControllers(options =>
            {
                options.ReturnHttpNotAcceptable = true;
                options.Filters.Add(new ActionAutoLoggerFilter());
            })
                .AddXmlDataContractSerializerFormatters();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BackendProcesses.API", Version = "v1" });
            });

            var mainDB = new DBToolsAsync(Config.GetConnectionString("FOAEAMain").ReplaceVariablesWithEnvironmentValues());

            services.AddScoped<IRepositories>(m => ActivatorUtilities.CreateInstance<DbRepositories>(m, mainDB)); // to access database procs
            services.AddScoped<IRepositories_Finance>(m => ActivatorUtilities.CreateInstance<DbRepositories_Finance>(m, mainDB)); // to access database procs for finance tables

            services.Configure<CustomConfig>(Config.GetSection("CustomConfig"));

            Log.Information("Using MainDB = {MainDB}", mainDB.ConnectionString);
            ColourConsole.WriteEmbeddedColorLine($"Using Connection: [yellow]{mainDB.ConnectionString}[/yellow]");

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
        {
            ColourConsole.WriteEmbeddedColorLine($"Using .Net Code Environment = [yellow]{env.EnvironmentName}[/yellow]");

            Log.Information("Using .Net Code Environment = {ASPNETCORE_ENVIRONMENT}", env.EnvironmentName);
            Log.Information("Machine Name = {MachineName}", Environment.MachineName);

            string currentServer = Environment.MachineName;
            var prodServersSection = Config.GetSection("ProductionServers");
            var prodServers = prodServersSection.Get<List<string>>();
            for (int i = 0; i < prodServers.Count; i++)
                prodServers[i] = prodServers[i].ReplaceVariablesWithEnvironmentValues();

            if (!env.IsEnvironment("Production"))
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BackendProcesses.API v1"));
            }
            else if (prodServers.Any(prodServer => prodServer.ToLower() == currentServer.ToLower()))
            {
                // return 500 if any uncaught exceptions occurs

                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. Try again later");
                    });
                });
            }
            else
            {
                // abort because the API is trying to connect to production from a non production server

                Log.Fatal("Trying to use Production environment on non-production server {currentServer}. Application stopping!", currentServer);

                Console.WriteLine($"Trying to use Production environment on non-production server {currentServer}");
                Console.WriteLine("Application stopping...");

                Task.Delay(2000).Wait();

                appLifetime.StopApplication();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var api_url = Config["Urls"];

            Console.WriteLine("");
            ColourConsole.WriteEmbeddedColorLine($"[green]Waiting for API calls...[/green] [yellow]{api_url}[/yellow]");
        }

    }
}
