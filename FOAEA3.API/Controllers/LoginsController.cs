using FOAEA3.API.Security;
using FOAEA3.Business.Security;
using FOAEA3.Common.Helpers;
using FOAEA3.Data.DB;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SecurityTokenData = FOAEA3.Model.SecurityTokenData;

namespace FOAEA3.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize()]
    public class LoginsController : ControllerBase
    {
        [HttpGet("Version")]
        public ActionResult<string> GetVersion() => Ok("Logins API Version 1.0");

        [HttpGet("DB")]
        [Authorize(Roles = Roles.Admin)]
        public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

        [AllowAnonymous]
        [HttpPost("TestLogin")]
        public async Task<ActionResult> TestLoginAction([FromBody] FoaeaLoginData loginData,
                                                        [FromServices] IOptions<TokenConfig> tokenConfigOptions,
                                                        [FromServices] IRepositories db)
        {
            // WARNING: not for production use!
            var tokenConfig = tokenConfigOptions.Value;
            if (tokenConfig == null)
                return StatusCode(500);

            var principal = await TestLogin.AutoLogin(loginData.UserName, loginData.Password, loginData.Submitter, db);
            if (principal is not null && principal.Identity is not null)
            {
                string apiKey = tokenConfig.Key.ReplaceVariablesWithEnvironmentValues();
                string issuer = tokenConfig.Issuer.ReplaceVariablesWithEnvironmentValues();
                string audience = tokenConfig.Audience.ReplaceVariablesWithEnvironmentValues();
                int expireMinutes = tokenConfig.ExpireMinutes;
                int refreshExpireMinutes = tokenConfig.RefreshTokenExpireMinutes;

                JwtSecurityToken token = SecurityTokenHelper.GenerateToken(issuer, audience, expireMinutes, apiKey,
                                                                           claims: principal.Claims.ToList());
                string refreshToken = SecurityTokenHelper.GenerateRefreshToken();

                DateTime refreshTokenExpiration = DateTime.Now.AddMinutes(refreshExpireMinutes);

                var tokenData = new TokenData
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    TokenExpiration = token.ValidTo,
                    RefreshToken = refreshToken,
                    RefreshTokenExpiration = refreshTokenExpiration
                };

                var securityToken = new SecurityTokenData
                {
                    Token = tokenData.Token,
                    TokenExpiration = tokenData.TokenExpiration,
                    RefreshToken = tokenData.RefreshToken,
                    RefreshTokenExpiration = tokenData.RefreshTokenExpiration,
                    SubjectName = loginData.UserName,
                    Subm_SubmCd = loginData.Submitter,
                    // fix this to handle multiple roles
                    Subm_Class = principal.Claims.Where(m => m.Type == ClaimTypes.Role).FirstOrDefault()?.Value
                };
                await db.SecurityTokenTable.CreateAsync(securityToken);

                return Ok(tokenData);
            }
            else
            {
                return BadRequest("Login failed.");
            }
        }

        [HttpPost("TestVerify")]
        public ActionResult TestVerify()
        {
            // WARNING: not for production use!
            var user = User.Identity;
            if (user is not null && user.IsAuthenticated)
            {
                var claims = User.Claims;
                var userName = claims.Where(m => m.Type == ClaimTypes.Name).FirstOrDefault()?.Value;
                var userRoles = claims.Where(m => m.Type == ClaimTypes.Role);
                var submitter = claims.Where(m => m.Type == "Submitter").FirstOrDefault()?.Value;

                string roles = string.Empty;
                if (userRoles != null)
                    foreach (var userRole in userRoles)
                    {
                        roles += userRole.Value;
                        if (userRole != userRoles.Last())
                            roles += ",";
                    }

                return Ok($"Logged in user: {userName} [{submitter} ({roles})]");
            }
            else
            {
                return Ok("No user logged in. Please login.");
            }
        }

        [AllowAnonymous]
        [HttpPost("TestRefreshToken")]
        public async Task<ActionResult> TestRefreshToken([FromBody] TokenRefreshData refreshData,
                                                         [FromServices] IOptions<TokenConfig> tokenConfigOptions,
                                                         [FromServices] IRepositories db)
        {
            var tokenConfig = tokenConfigOptions.Value;
            if (tokenConfig == null)
                return StatusCode(500);

            string oldToken = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(oldToken) || oldToken.Length < 8)
                return BadRequest();

            oldToken = oldToken[7..]; // get rid of the word Bearer that is at the beginning

            var lastSecurityToken = await db.SecurityTokenTable.GetTokenDataAsync(oldToken);

            if (lastSecurityToken is null ||
                !string.Equals(lastSecurityToken.RefreshToken, refreshData.RefreshToken) ||
                lastSecurityToken.RefreshTokenExpiration < DateTime.Now)
            {
                return BadRequest();
            }

            await db.SecurityTokenTable.MarkTokenAsExpired(oldToken);

            List<Claim> roleClaims;
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken2 = (JwtSecurityToken)tokenHandler.ReadToken(oldToken);
                roleClaims = securityToken2.Claims.Where(c => c.Type == ClaimTypes.Role)?.ToList();
            }
            catch (Exception)
            {
                //TODO: Log Error
                return null;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, lastSecurityToken.SubjectName),
                new Claim("Submitter", lastSecurityToken.Subm_SubmCd),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            if (roleClaims is not null)
                claims.AddRange(roleClaims);

            string apiKey = tokenConfig.Key.ReplaceVariablesWithEnvironmentValues();
            string issuer = tokenConfig.Issuer.ReplaceVariablesWithEnvironmentValues();
            string audience = tokenConfig.Audience.ReplaceVariablesWithEnvironmentValues();
            int expireMinutes = tokenConfig.ExpireMinutes;
            int refreshExpireMinutes = tokenConfig.RefreshTokenExpireMinutes;

            JwtSecurityToken token = SecurityTokenHelper.GenerateToken(issuer, audience, expireMinutes,
                                                                       apiKey, claims: claims.ToList());
            string newRefreshToken = SecurityTokenHelper.GenerateRefreshToken();

            DateTime refreshTokenExpiration = DateTime.Now.AddMinutes(refreshExpireMinutes);

            var tokenData = new TokenData
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                TokenExpiration = token.ValidTo,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiration = refreshTokenExpiration
            };

            var securityToken = new SecurityTokenData
            {
                Token = tokenData.Token,
                TokenExpiration = tokenData.TokenExpiration,
                RefreshToken = tokenData.RefreshToken,
                RefreshTokenExpiration = tokenData.RefreshTokenExpiration,
                SubjectName = lastSecurityToken.SubjectName,
                Subm_SubmCd = lastSecurityToken.Subm_SubmCd,
                Subm_Class = lastSecurityToken.Subm_Class,
                FromRefreshToken = lastSecurityToken.RefreshToken
            };
            await db.SecurityTokenTable.CreateAsync(securityToken);

            return Ok(tokenData);
        }

        [HttpPost("TestLogout")]
        public async Task<ActionResult> TestLogout([FromServices] IRepositories db)
        {
            // WARNING: not for production use!
            string oldToken = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(oldToken) || oldToken.Length < 8)
                return BadRequest();

            oldToken = oldToken[7..]; // get rid of the word Bearer that is at the beginning

            await db.SecurityTokenTable.MarkTokenAsExpired(oldToken);

            return Ok();
        }

        [HttpGet("CheckPreviousPasswords")]
        public async Task<ActionResult<string>> CheckPreviousPasswordsAsync([FromQuery] string subjectName, [FromQuery] string encryptedNewPassword, [FromServices] IRepositories repositories)
        {
            var loginManager = new LoginManager(repositories);
            var result = await loginManager.CheckPreviousPasswordsAsync(subjectName, encryptedNewPassword);

            return Ok(result ? "true" : "false");
        }

        [HttpPut("SetPassword")]
        public async Task<ActionResult<string>> SetNewPassword([FromQuery] string encryptedNewPassword, [FromServices] IRepositories repositories)
        {
            var subject = await APIBrokerHelper.GetDataFromRequestBodyAsync<SubjectData>(Request);

            var loginManager = new LoginManager(repositories);
            var result = await loginManager.CheckPreviousPasswordsAsync(subject.SubjectName, encryptedNewPassword);

            return Ok(result ? "true" : "false");
        }

        [HttpPost("SendEmail")]
        [Authorize(Roles = "System")]
        public async Task<ActionResult<string>> SendEmail([FromServices] IRepositories repositories)
        {
            var emailData = await APIBrokerHelper.GetDataFromRequestBodyAsync<EmailData>(Request);

            var loginManager = new LoginManager(repositories);
            await loginManager.SendEmailAsync(emailData.Subject, emailData.Recipient, emailData.Body, emailData.IsHTML);

            return Ok(emailData);

        }

        [HttpGet("PostConfirmationCode")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<string>> PostConfirmationCode([FromQuery] int subjectId, [FromQuery] string confirmationCode, [FromServices] IRepositories repositories)
        {
            var dbLogin = new DBLogin(repositories.MainDB);

            await dbLogin.PostConfirmationCodeAsync(subjectId, confirmationCode);

            return Ok(string.Empty);
        }

        [HttpGet("GetEmailByConfirmationCode")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<string>> GetEmailByConfirmationCode([FromQuery] string confirmationCode, [FromServices] IRepositories repositories)
        {
            var dbLogin = new DBLogin(repositories.MainDB);

            return Ok(await dbLogin.GetEmailByConfirmationCodeAsync(confirmationCode));
        }

        [HttpGet("GetSubjectByConfirmationCode")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<SubjectData>> GetSubjectByConfirmationCode([FromQuery] string confirmationCode, [FromServices] IRepositories repositories)
        {
            var dbSubject = new DBSubject(repositories.MainDB);

            return Ok(await dbSubject.GetSubjectByConfirmationCodeAsync(confirmationCode));
        }

        [HttpPut("PostPassword")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<PasswordData>> PostPassword([FromServices] IRepositories repositories)
        {
            var passwordData = await APIBrokerHelper.GetDataFromRequestBodyAsync<PasswordData>(Request);

            var dbLogin = new DBLogin(repositories.MainDB);
            await dbLogin.PostPasswordAsync(passwordData.ConfirmationCode, passwordData.Password, passwordData.Salt, passwordData.Initial);

            return Ok(passwordData);
        }

    }
}
