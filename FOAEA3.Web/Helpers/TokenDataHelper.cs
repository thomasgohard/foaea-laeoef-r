using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace FOAEA3.Web.Helpers
{
    public static class TokenDataHelper
    {
        public static string UserName(string currentToken)
        {
            var jwt = currentToken;
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);

            var claims = token.Claims;

            return claims.Where(m => m.Type == ClaimTypes.Name).FirstOrDefault()?.Value;
        }

        public static string SubmitterCode(string currentToken)
        {
            var jwt = currentToken;
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);

            var claims = token.Claims;

            return claims.Where(m => m.Type == "Submitter").FirstOrDefault()?.Value;
        }
    }
}
