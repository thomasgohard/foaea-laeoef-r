using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FOAEA3.Common.Helpers
{
    public class SecurityTokenHelper
    {
        public static JwtSecurityToken GenerateToken(string issuer, string audience, int expireMinutes, string apiKey, List<Claim> claims = null)
        {
            var encodedApiKey = Encoding.UTF8.GetBytes(apiKey);
            var securityKey = new SymmetricSecurityKey(encodedApiKey);
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            DateTime expirationDateTime = DateTime.Now.AddMinutes(expireMinutes);

            // adjust time due to possible bug in JwtSecurityToken?
            TimeSpan delta = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
            double offset = delta.TotalHours;
            var expirationDateTimeAdjusted = expirationDateTime.AddHours(offset);

            var token = new JwtSecurityToken(issuer,
                                             audience,
                                             claims, signingCredentials: creds,
                                             expires: expirationDateTimeAdjusted);

            if (token.ValidTo < DateTime.Now) // do this in case the bug gets fixed
                token = new JwtSecurityToken(issuer,
                                             audience,
                                             claims, signingCredentials: creds,
                                             expires: expirationDateTime);

            return token;
        }

        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
