using FOAEA3.Helpers;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FOAEA3.API.Security
{
    public class TestLogin
    {
        public static async Task<(ClaimsPrincipal, JwtSecurityToken)> AutoLogin(string user, string password, 
                                                                                string submitter, 
                                                                                IRepositories db, 
                                                                                string apiKey)
        {
            var subject = await db.SubjectTable.GetSubjectAsync(user);

            if (subject is null || subject.IsAccountLocked is true)
                return (new ClaimsPrincipal(), null);

            string encryptedPassword = subject.Password;
            string salt = subject.PasswordSalt;

            if (!PasswordHelper.IsValidPassword(password, salt, encryptedPassword))
                return (new ClaimsPrincipal(), null);

            string userName = subject.SubjectName;

            var submitterData = (await db.SubmitterTable.GetSubmitterAsync(submitter)).FirstOrDefault();

            if (submitterData is null || submitterData.ActvSt_Cd != "A")
                return (new ClaimsPrincipal(), null);

            string userRole = submitterData.Subm_Class switch
            {
                "AM" => "Admin",
                _ => "User",
            };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, userRole),
                new Claim("Submitter", submitter),
                new Claim(JwtRegisteredClaimNames.Sub, subject.EMailAddress),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, userName)
            };

            var encodedApiKey = Encoding.UTF8.GetBytes(apiKey);
            var securityKey = new SymmetricSecurityKey(encodedApiKey);
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken("Justice", "Justice", claims, signingCredentials: creds,
                                             expires: DateTime.UtcNow.AddMinutes(20));

            var identity = new ClaimsIdentity(claims, "Identity.Application");
            var principal = new ClaimsPrincipal(identity);

            return (principal, token);
        }

    }
}
