using FOAEA3.API.broker;
using FOAEA3.Data.Base;
using FOAEA3.Helpers;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Globalization;

namespace FOAEA3
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddLocalization(options => options.ResourcesPath = "FOAEA3.Resources");

            services.Configure<RequestLocalizationOptions>(options =>
            {
                IList<CultureInfo> supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en"),
                    new CultureInfo("fr"),
                };

                options.DefaultRequestCulture = new RequestCulture(culture: "en", uiCulture: "en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation()
                .AddNewtonsoftJson();

            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IHttpContextAccessor>(m => ActivatorUtilities.CreateInstance<HttpContextAccessor>(m));

            services
                .AddSession(session =>
                {
                    //session.IdleTimeout = TimeSpan.FromMinutes(5);
                    session.Cookie.IsEssential = true;
                })
                .AddMvc(config =>
                {
                    //config.Filters.Add(new Filters.SessionEndFilterAttribute());
                    config.Filters.Add(new Filters.LoginCheckActionFilterAttribute());
                    config.Filters.Add(new Filters.NoCacheAttribute());
                    config.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                })
                .AddViewLocalization();

            services.AddHealthChecks();

            BaseAPI.APIroot = Config.GetValue<string>("APIRoot").ReplaceVariablesWithEnvironmentValues();
            BaseAPI.APIroot_Interception = Config.GetValue<string>("APIRoot_Interception").ReplaceVariablesWithEnvironmentValues();
            BaseAPI.APIroot_LicenceDenial = Config.GetValue<string>("APIRoot_LicenceDenial").ReplaceVariablesWithEnvironmentValues();
            BaseAPI.APIroot_Tracing = Config.GetValue<string>("APIRoot_Tracing").ReplaceVariablesWithEnvironmentValues();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.EnvironmentName != "Production" && env.EnvironmentName != "Staging")
            {

                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/Home/ErrorDev");

            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseHsts();

            app.UseRequestLocalization();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                if (env.EnvironmentName != "Production" && env.EnvironmentName != "Staging")
                {
                    endpoints.MapHealthChecks("/health");
                }
            });

            SessionHelper.Services = app.ApplicationServices;

            // preload reference data

            var foaEventsAPI = new FoaEventsAPI();
            var activeStatusesAPI = new ActiveStatusesAPI();
            var gendersAPI = new GendersAPI();
            var applicationLifeStatesAPI = new ApplicationLifeStatesAPI();
            var applicationCommentsAPI = new ApplicationCommentsAPI();

            ReferenceData.Instance().FoaEvents = foaEventsAPI.GetFoaEvents();
            ReferenceData.Instance().ActiveStatuses = activeStatusesAPI.GetActiveStatuses();
            ReferenceData.Instance().Genders = gendersAPI.GetGenders();
            ReferenceData.Instance().ApplicationLifeStates = applicationLifeStatesAPI.GetApplicationLifeStates();
            ReferenceData.Instance().ApplicationComments = applicationCommentsAPI.GetApplicationComments();

        }

    }
}
