using DBHelper;
using FOAEA3.Admin.Web.Filter;
using FOAEA3.Common.Helpers;
using FOAEA3.Data.Base;
using FOAEA3.Data.DB;
using FOAEA3.Model.Interfaces.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace FOAEA3.Admin.Web
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var config = new FoaeaConfigurationHelper();
            var mainDB = new DBTools(config.FoaeaConnection);

            services.AddRazorPages()
                    .AddMvcOptions(options =>
                    {
                        options.Filters.Add(new RazorPageActionFilter(mainDB));
                    });

            services.AddScoped<IDBToolsAsync>(m => ActivatorUtilities.CreateInstance<DBToolsAsync>(m, mainDB)); // used to display the database name at top of page
            services.AddScoped<IRepositories>(m => ActivatorUtilities.CreateInstance<DbRepositories>(m, mainDB));
            services.AddScoped<IRepositories_Finance>(m => ActivatorUtilities.CreateInstance<DbRepositories_Finance>(m, mainDB)); // to access database procs for finance tables
            services.AddScoped<IFoaEventsRepository>(m => ActivatorUtilities.CreateInstance<DBFoaMessage>(m, mainDB));
            services.AddScoped<IActiveStatusRepository>(m => ActivatorUtilities.CreateInstance<DBActiveStatus>(m, mainDB));
            services.AddScoped<IGenderRepository>(m => ActivatorUtilities.CreateInstance<DBGender>(m, mainDB));
            services.AddScoped<IApplicationCommentsRepository>(m => ActivatorUtilities.CreateInstance<DBApplicationComments>(m, mainDB));
            services.AddScoped<IApplicationLifeStateRepository>(m => ActivatorUtilities.CreateInstance<DBApplicationLifeState>(m, mainDB));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async Task Configure(IApplicationBuilder app, IWebHostEnvironment env, IRepositories repositories)
        {
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

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
        }
    }
}
