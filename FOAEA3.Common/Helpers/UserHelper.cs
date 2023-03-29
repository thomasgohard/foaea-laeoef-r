using FOAEA3.Common.Models;
using FOAEA3.Model;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Interfaces.Repository;
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
                var submitterData = (await db.SubmitterTable.GetSubmitterAsync(submCode: submitterCode)).FirstOrDefault();

                if ((submitterData is null) || (!submitterData.Subm_SubmCd.Equals(submitterCode, StringComparison.InvariantCultureIgnoreCase)))
                    submitterData = new SubmitterData();

                var enfOffData = (await db.EnfOffTable.GetEnfOffAsync(enfOffCode: submitterData?.EnfOff_City_LocCd)).FirstOrDefault();

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
    }
}
