using DBHelper;
using FOAEA3.Admin.Web.Filter;
using FOAEA3.Common.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace FOAEA3.Admin.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            LocalConfiguration = configuration;
        }

        public IConfiguration LocalConfiguration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var config = new FoaeaConfigurationHelper();
            var mainDB = new DBToolsAsync(config.FoaeaConnection);

            services.AddRazorPages()
                    .AddMvcOptions(options =>
                    {
                        options.Filters.Add(new RazorPageActionFilter(mainDB));
                    });
            services.AddHttpContextAccessor();

            LoggingHelper.SetupLogging(config.FoaeaConnection);

            Common.Startup.AddDBServices(services, config.FoaeaConnection);

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(15);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsEnvironment("Production"))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            Common.Startup.AddReferenceDataFromDB(app.ApplicationServices).Wait();
        }
    }
}
