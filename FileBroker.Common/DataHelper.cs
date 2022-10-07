using DBHelper;
using FileBroker.Data.DB;
using FileBroker.Model.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FileBroker.Common
{
    public static class DataHelper
    {
        public static string ConfigureDBServices(IServiceCollection services, string fileBrokerConnectionString)
        {
            var fileBrokerDB = new DBToolsAsync(fileBrokerConnectionString);

            services.AddScoped<IFlatFileSpecificationRepository>(m => ActivatorUtilities.CreateInstance<DBFlatFileSpecification>(m, fileBrokerDB));
            services.AddScoped<IFileTableRepository>(m => ActivatorUtilities.CreateInstance<DBFileTable>(m, fileBrokerDB));
            services.AddScoped<IFileAuditRepository>(m => ActivatorUtilities.CreateInstance<DBFileAudit>(m, fileBrokerDB));
            services.AddScoped<IRequestLogRepository>(m => ActivatorUtilities.CreateInstance<DBRequestLog>(m, fileBrokerDB));
            services.AddScoped<ITranslationRepository>(m => ActivatorUtilities.CreateInstance<DBTranslation>(m, fileBrokerDB));
            services.AddScoped<IProcessParameterRepository>(m => ActivatorUtilities.CreateInstance<DBProcessParameter>(m, fileBrokerDB));
            services.AddScoped<IOutboundAuditRepository>(m => ActivatorUtilities.CreateInstance<DBOutboundAudit>(m, fileBrokerDB));
            services.AddScoped<IErrorTrackingRepository>(m => ActivatorUtilities.CreateInstance<DBErrorTracking>(m, fileBrokerDB));
            services.AddScoped<IMailServiceRepository>(m => ActivatorUtilities.CreateInstance<DBMailService>(m, fileBrokerDB));
            services.AddScoped<ILoadInboundAuditRepository>(m => ActivatorUtilities.CreateInstance<DBLoadInboundAudit>(m, fileBrokerDB));
            services.AddScoped<IUserRepository>(m => ActivatorUtilities.CreateInstance<DBUser>(m, fileBrokerDB));
            services.AddScoped<ISecurityTokenRepository>(m => ActivatorUtilities.CreateInstance<DBSecurityToken>(m, fileBrokerDB));

            return fileBrokerDB.ConnectionString;
        }
    }
}
