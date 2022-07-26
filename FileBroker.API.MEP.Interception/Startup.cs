using FileBroker.Common;
using FileBroker.Model;
using FOAEA3.Common.Filters;
using FOAEA3.Model;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace FileBroker.API.MEP.Interception
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.ReturnHttpNotAcceptable = true;
                options.Filters.Add(new ActionAutoLoggerFilter());
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FileBroker.API.MEP.Interception", Version = "v1" });
            });
            services.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                o.ReportApiVersions = true;
                o.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("api-version"),
                    new HeaderApiVersionReader("X-Version"),
                    new MediaTypeApiVersionReader("ver"));
            });

            string fileBrokerCON = Configuration.GetConnectionString("FileBroker").ReplaceVariablesWithEnvironmentValues();

            ColourConsole.WriteEmbeddedColorLine($"Starting [cyan]FileBroker.API.MEP.Interception[/cyan]...");

            string actualConnection = DataHelper.ConfigureDBServices(services, fileBrokerCON);

            ColourConsole.WriteEmbeddedColorLine($"Using Connection: [yellow]{actualConnection}[/yellow]");

            services.Configure<ProvincialAuditFileConfig>(Configuration.GetSection("AuditConfig"));
            services.Configure<ApiConfig>(Configuration.GetSection("APIroot"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FileBroker.API.MEP.Interception v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var api_url = Configuration["Urls"];

            ColourConsole.WriteEmbeddedColorLine($"Using .Net Code Environment = [yellow]{env.EnvironmentName}[/yellow]");
            ColourConsole.WriteLine($"");
            ColourConsole.WriteEmbeddedColorLine($"[green]Waiting for API calls...[/green] [yellow]{api_url}[/yellow]");
            ColourConsole.WriteLine($"");
        }
    }
}
