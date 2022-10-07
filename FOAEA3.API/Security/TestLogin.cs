﻿using FOAEA3.Helpers;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FOAEA3.API.Security
{
    public class TestLogin
    {
        public static async Task<ClaimsPrincipal> AutoLogin(string user, string password,
                                                                                string submitter,
                                                                                IRepositories db)
        {
            var subject = await db.SubjectTable.GetSubjectAsync(user);

            if (subject is null || subject.IsAccountLocked is true)
                return new ClaimsPrincipal();

            string encryptedPassword = subject.Password;
            string salt = subject.PasswordSalt;

            if (!PasswordHelper.IsValidPassword(password, salt, encryptedPassword))
                return new ClaimsPrincipal();

            string userName = subject.SubjectName;

            var submitterData = (await db.SubmitterTable.GetSubmitterAsync(submitter)).FirstOrDefault();

            if (submitterData is null || submitterData.ActvSt_Cd != "A")
                return new ClaimsPrincipal();

            string userRole = submitterData.Subm_Class;

            if (string.Equals(userName, "system_support", StringComparison.InvariantCultureIgnoreCase))
                userRole += ", Admin";

            if (submitterData.Subm_SubmCd.IsInternalUser())
                userRole += ", FO";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim("Submitter", submitter),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            SetupRoleClaims(claims, userRole);

            var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            return principal;
        }

        private static void SetupRoleClaims(List<Claim> claims, string securityRole)
        {
            string[] roles = securityRole.Split(",");
            foreach (string role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role.Trim()));
        }

    }
}
