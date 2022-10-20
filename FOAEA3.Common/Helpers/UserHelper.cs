using FOAEA3.Common.Models;
using FOAEA3.Model.Interfaces;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FOAEA3.Common.Helpers
{
    public static class UserHelper
    {
        public static async Task<FoaeaUser> ExtractDataFromUser(ClaimsPrincipal user, IRepositories db)
        {
            var claims = user.Claims;
            var userName = claims.Where(m => m.Type == ClaimTypes.Name).FirstOrDefault()?.Value;
            var userRoles = claims.Where(m => m.Type == ClaimTypes.Role);
            var submitterCode = claims.Where(m => m.Type == "Submitter").FirstOrDefault()?.Value;
            var submitterData = (await db.SubmitterTable.GetSubmitterAsync(submCode: submitterCode)).FirstOrDefault();
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
