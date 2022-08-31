using DBHelper;
using FOAEA3.Admin.Web.Filter;
using FOAEA3.Data.Base;
using FOAEA3.Data.DB;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace FOAEA3.Admin.Web
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
            var mainDB = new DBTools(Configuration.GetConnectionString("FOAEAMain").ReplaceVariablesWithEnvironmentValues());

            services.AddRazorPages()
                    .AddMvcOptions(options =>
                    {
                        options.Filters.Add(new RazorPageActionFilter(Configuration, mainDB));
                    });

            services.AddScoped<IDBToolsAsync>(m => ActivatorUtilities.CreateInstance<DBToolsAsync>(m, mainDB)); // used to display the database name at top of page
            services.AddScoped<IRepositories>(m => ActivatorUtilities.CreateInstance<DbRepositories>(m, mainDB));
            services.AddScoped<IRepositories_Finance>(m => ActivatorUtilities.CreateInstance<DbRepositories_Finance>(m, mainDB)); // to access database procs for finance tables
            services.AddScoped<IFoaEventsRepository>(m => ActivatorUtilities.CreateInstance<DBFoaMessage>(m, mainDB));
            services.AddScoped<IActiveStatusRepository>(m => ActivatorUtilities.CreateInstance<DBActiveStatus>(m, mainDB));
            services.AddScoped<IGenderRepository>(m => ActivatorUtilities.CreateInstance<DBGender>(m, mainDB));
            services.AddScoped<IApplicationCommentsRepository>(m => ActivatorUtilities.CreateInstance<DBApplicationComments>(m, mainDB));
            services.AddScoped<IApplicationLifeStateRepository>(m => ActivatorUtilities.CreateInstance<DBApplicationLifeState>(m, mainDB));

            services.Configure<ApiConfig>(Configuration.GetSection("APIroot"));

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

            await ReferenceData.Instance().LoadFoaEventsAsync(new DBFoaMessage(repositories.MainDB));
            await ReferenceData.Instance().LoadActiveStatusesAsync(new DBActiveStatus(repositories.MainDB));
            await ReferenceData.Instance().LoadGendersAsync(new DBGender(repositories.MainDB));
            await ReferenceData.Instance().LoadProvincesAsync(new DBProvince(repositories.MainDB));
            await ReferenceData.Instance().LoadMediumsAsync(new DBMedium(repositories.MainDB));
            await ReferenceData.Instance().LoadLanguagesAsync(new DBLanguage(repositories.MainDB));
            await ReferenceData.Instance().LoadDocumentTypesAsync(new DBDocumentType(repositories.MainDB));
            await ReferenceData.Instance().LoadCountriesAsync(new DBCountry(repositories.MainDB));
            await ReferenceData.Instance().LoadApplicationReasonsAsync(new DBApplicationReason(repositories.MainDB));
            await ReferenceData.Instance().LoadApplicationCategoriesAsync(new DBApplicationCategory(repositories.MainDB));
            await ReferenceData.Instance().LoadApplicationLifeStatesAsync(new DBApplicationLifeState(repositories.MainDB));
            await ReferenceData.Instance().LoadApplicationCommentsAsync(new DBApplicationComments(repositories.MainDB));
        }
    }
}
