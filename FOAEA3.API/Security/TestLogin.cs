using FOAEA3.Common.Helpers;
using FOAEA3.Helpers;
using FOAEA3.Model.Interfaces;
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

            string userRole = submitterData.Subm_Class switch
            {
                "AM" => "Admin",
                _ => "User",
            };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, userRole),
                new Claim("Submitter", submitter)
            };

            var identity = new ClaimsIdentity(claims, LoggingHelper.COOKIE_ID);
            var principal = new ClaimsPrincipal(identity);

            return principal;
        }

    }
}
