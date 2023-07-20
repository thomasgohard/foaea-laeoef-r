using FOAEA3.Common.Models;
using FOAEA3.Data.DB;
using FOAEA3.Model;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Interfaces.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Security;
using System.Security.Claims;

namespace FOAEA3.Common.Helpers
{
    public static class UserHelper
    {
        public static async Task<FoaeaUser> ExtractDataFromUser(ClaimsPrincipal user, IRepositories db)
        {
            var claims = user.Claims;
            var userName = claims.Where(m => m.Type == ClaimTypes.Name).FirstOrDefault()?.Value;

            if ((userName is not null) && (userName.ToUpper().StartsWith("JUSTICE")))
            {
                // windows authentication, make an admin
                var currentUser = new FoaeaUser
                {
                    SubjectName = userName,
                    Submitter = new SubmitterData
                    {
                        Subm_SubmCd = "SYSTEM"
                    },
                    OfficeCode = string.Empty
                };

                currentUser.UserRoles.Add(Roles.Admin);

                return currentUser;
            }
            else
            {
                var userRoles = claims.Where(m => m.Type == ClaimTypes.Role);
                var submitterCode = claims.Where(m => m.Type == "Submitter").FirstOrDefault()?.Value;
                var submitterData = (await db.SubmitterTable.GetSubmitter(submCode: submitterCode)).FirstOrDefault();

                if ((submitterData is null) || (!submitterData.Subm_SubmCd.Equals(submitterCode, StringComparison.InvariantCultureIgnoreCase)))
                    submitterData = new SubmitterData();

                var enfOffData = (await db.EnfOffTable.GetEnfOff(enfOffCode: submitterData?.EnfOff_City_LocCd)).FirstOrDefault();

                var currentUser = new FoaeaUser
                {
                    SubjectName = userName,
                    Submitter = submitterData,
                    OfficeCode = enfOffData?.EnfOff_AbbrCd
                };

                if (userRoles != null)
                    foreach (var userRole in userRoles)
                        currentUser.UserRoles.Add(userRole.Value);

                return currentUser;
            }
        }

        public static ClaimsPrincipal CreateSystemAdminUser()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "System"),
                new Claim("Submitter", "MSGBRO"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.Add(new Claim(ClaimTypes.Role, "AM"));
            claims.Add(new Claim(ClaimTypes.Role, Roles.Admin));

            var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            return principal;
        }
    }
}
