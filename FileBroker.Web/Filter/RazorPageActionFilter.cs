using DBHelper;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileBroker.Web.Filter
{
    public class RazorPageActionFilter : IAsyncPageFilter
    {
        private readonly IConfiguration _config;
        private readonly IDBTools _mainDB;

        public RazorPageActionFilter(IConfiguration config, IDBTools mainDB)
        {
            _config = config;
            _mainDB = mainDB;
        }

        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            static string GetDBValue(string info)
            {
                string result = info;

                string[] values = info.Split('=');
                if (values.Length == 2)
                    result = values[1];

                return result;
            }

            var instance = context.HandlerInstance;
            if (instance is PageModel thisPage)
            {
                var connectionString = _mainDB.ConnectionString;
                string[] dbInfo = connectionString.Split(';');
                string server = string.Empty;
                string database = string.Empty;
                foreach (string info in dbInfo)
                {
                    if (info.StartsWith("server", StringComparison.OrdinalIgnoreCase))
                        server = GetDBValue(info);
                    else if (info.StartsWith("database", StringComparison.OrdinalIgnoreCase))
                        database = GetDBValue(info);
                }

                thisPage.ViewData["dbInfo"] = $@"{database.ToUpper()} on {server.ToUpper()}";
            }

            return Task.CompletedTask;
        }

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context,
                                                      PageHandlerExecutionDelegate next)
        {
            await next.Invoke();
        }

    }
}
