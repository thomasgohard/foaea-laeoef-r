using FOAEA3.Common.Brokers;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Web.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace FOAEA3.Web.Pages
{
    public class FoaeaPageModel : PageModel
    {
        protected readonly ApiConfig ApiRoots;
        protected readonly APIBrokerHelper APIs;

        public FoaeaPageModel(IHttpContextAccessor httpContextAccessor, ApiConfig apiConfig)
        {
            ApiRoots = apiConfig;
            
            string submitter = httpContextAccessor.HttpContext.Session.GetString(SessionValue.SUBMITTER);
            string userName = httpContextAccessor.HttpContext.Session.GetString(SessionValue.USER_NAME);

            // call API to do search and display result
            APIs = new APIBrokerHelper(ApiRoots.FoaeaRootAPI, submitter, userName, getRefreshedToken: GetRefreshedToken);
        }

        public async Task<string> GetRefreshedToken()
        {
            string currentToken = HttpContext.Session.GetString(SessionValue.TOKEN);
            string refreshToken = HttpContext.Session.GetString(SessionValue.REFRESH_TOKEN);

            string userName = TokenDataHelper.UserName(currentToken);
            string submitter = TokenDataHelper.SubmitterCode(currentToken);

            var apiHelper = new APIBrokerHelper(ApiRoots.FoaeaRootAPI, submitter, userName);
            var apiBroker = new LoginsAPIBroker(apiHelper, currentToken);

            var result = await apiBroker.RefreshTokenAsync(currentToken, refreshToken);

            HttpContext.Session.SetString(SessionValue.TOKEN, result.Token);
            HttpContext.Session.SetString(SessionValue.REFRESH_TOKEN, result.RefreshToken);

            return result.Token;
        }
    }
}
