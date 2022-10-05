using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
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

            var token = new JwtSecurityToken(issuer,
                                             audience,
                                             claims, signingCredentials: creds,
                                             expires: DateTime.Now.AddMinutes(expireMinutes));
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
