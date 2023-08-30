using FOAEA3.API.Security;
using FOAEA3.Business.Areas.Administration;
using FOAEA3.Business.Security;
using FOAEA3.Common;
using FOAEA3.Common.Helpers;
using FOAEA3.Model;
using FOAEA3.Model.Constants;
using FOAEA3.Model.Interfaces.Repository;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SecurityTokenData = FOAEA3.Model.SecurityTokenData;

namespace FOAEA3.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize()]
    public class LoginsController : FoaeaControllerBase
    {
        [AllowAnonymous]
        [HttpGet("Version")]
        public ActionResult<string> GetVersion() => Ok("Logins API Version 1.0");

        [HttpGet("DB")]
        [Authorize(Roles = Roles.Admin)]
        public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

        [AllowAnonymous]
        [HttpPost("SubjectLogin")]
        public async Task<ActionResult> SubjectLoginAction([FromBody] FoaeaLoginData loginData,
                                                           [FromServices] IRepositories db)
        {
            var configHelper = new FoaeaConfigurationHelper();

            var tokenConfig = configHelper.Tokens;
            if (tokenConfig == null)
                return StatusCode(500);

            var principal = await LoginHandler.LoginSubject(loginData.UserName, loginData.Password, db);
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
                    Subm_SubmCd = "",
                    // fix this to handle multiple roles
                    Subm_Class = principal.Claims.Where(m => m.Type == ClaimTypes.Role).FirstOrDefault()?.Value
                };
                await db.SecurityTokenTable.Create(securityToken);

                return Ok(tokenData);
            }
            else
            {
                return BadRequest("Login failed.");
            }
        }

        [HttpPut("AcceptTerms")]
        [Authorize(Roles = Roles.AcceptTermsOfReference)]
        public async Task<ActionResult> AcceptTermsOfReference([FromServices] IRepositories db)
        {
            var user = User;
            string subjectName = user?.Identity?.Name;

            var configHelper = new FoaeaConfigurationHelper();

            var tokenConfig = configHelper.Tokens;
            if (tokenConfig == null)
                return StatusCode(500);

            await db.LoginTable.AcceptNewTermsOfReferernce(subjectName);

            var principal = LoginHandler.AcceptTermsOfReference(subjectName, db);

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
                    SubjectName = subjectName,
                    Subm_SubmCd = "",
                    // fix this to handle multiple roles
                    Subm_Class = principal.Claims.Where(m => m.Type == ClaimTypes.Role).FirstOrDefault()?.Value
                };
                await db.SecurityTokenTable.Create(securityToken);

                return Ok(tokenData);
            }
            else
            {
                return BadRequest("Login failed.");
            }
        }

        [HttpGet("Submitters")]
        [Authorize(Roles = Roles.SelectSubmitter)]
        public async Task<ActionResult> GetAvailableSubmitters([FromServices] IRepositories db)
        {
            var user = User;
            string subjectName = user?.Identity?.Name;

            var configHelper = new FoaeaConfigurationHelper();

            var tokenConfig = configHelper.Tokens;
            if (tokenConfig == null)
                return StatusCode(500);

            var roles = await db.SubjectRoleTable.GetSubjectRoles(subjectName);
            var availableSubmitters = (from role in roles select role.RoleName).ToList();

            return Ok(availableSubmitters);
        }

        [HttpPut("SelectSubmitter")]
        [Authorize(Roles = Roles.SelectSubmitter)]
        public async Task<ActionResult> SelectSubmitterAction([FromQuery] string submitter,
                                                              [FromServices] IRepositories db)
        {
            var user = User;
            string subjectName = user?.Identity?.Name;

            var configHelper = new FoaeaConfigurationHelper();

            var tokenConfig = configHelper.Tokens;
            if (tokenConfig == null)
                return StatusCode(500);

            var roles = await db.SubjectRoleTable.GetSubjectRoles(subjectName);
            var availableSubmitters = (from role in roles select role.RoleName).ToList();
            if (!availableSubmitters.Contains(submitter))
            {
                return BadRequest("Select submitter failed.");
            }

            var principal = await LoginHandler.SelectSubmitter(subjectName, submitter, db);
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
                    SubjectName = subjectName,
                    Subm_SubmCd = submitter,
                    // fix this to handle multiple roles
                    Subm_Class = principal.Claims.Where(m => m.Type == ClaimTypes.Role).FirstOrDefault()?.Value
                };
                await db.SecurityTokenTable.Create(securityToken);

                return Ok(tokenData);
            }
            else
            {
                return BadRequest("Select submitter failed.");
            }
        }

        [AllowAnonymous]
        [HttpPost("SingleStepLogin")]
        public async Task<ActionResult> SingleStepLoginAction([FromBody] FoaeaLoginData loginData,
                                                              [FromServices] IRepositories db)
        {
            var configHelper = new FoaeaConfigurationHelper();

            var tokenConfig = configHelper.Tokens;
            if (tokenConfig == null)
                return StatusCode(500);

            var principal = await SingleStepLogin.FoaeaLogin(loginData.UserName, loginData.Password, loginData.Submitter, db);
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
                await db.SecurityTokenTable.Create(securityToken);

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
                                                         [FromServices] IRepositories db)
        {
            var configHelper = new FoaeaConfigurationHelper();

            var tokenConfig = configHelper.Tokens;
            if (tokenConfig == null)
                return StatusCode(500);

            string oldToken = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(oldToken) || oldToken.Length < 8)
                return BadRequest();

            oldToken = oldToken[7..]; // get rid of the word Bearer that is at the beginning

            var lastSecurityToken = await db.SecurityTokenTable.GetTokenData(oldToken);

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
                // TODO: Log Error
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
            await db.SecurityTokenTable.Create(securityToken);

            return Ok(tokenData);
        }

        [HttpPost("TestLogout")]
        public async Task<ActionResult> TestLogout([FromServices] IRepositories db)
        {
            string oldToken = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(oldToken) || oldToken.Length < 8)
                return BadRequest();

            oldToken = oldToken[7..]; // get rid of the word Bearer that is at the beginning

            await db.SecurityTokenTable.MarkTokenAsExpired(oldToken);

            return Ok();
        }

        [HttpGet("CheckPreviousPasswords")]
        public async Task<ActionResult<string>> CheckPreviousPasswords([FromQuery] string subjectName, [FromQuery] string encryptedNewPassword, [FromServices] IRepositories repositories)
        {
            var loginManager = new LoginManager(repositories);
            var result = await loginManager.CheckPreviousPasswords(subjectName, encryptedNewPassword);

            return Ok(result ? "true" : "false");
        }

        [HttpPut("SetPassword")]
        public async Task<ActionResult<string>> SetNewPassword([FromQuery] string encryptedNewPassword, [FromServices] IRepositories repositories)
        {
            var subject = await APIBrokerHelper.GetDataFromRequestBody<SubjectData>(Request);

            var loginManager = new LoginManager(repositories);
            var result = await loginManager.CheckPreviousPasswords(subject.SubjectName, encryptedNewPassword);

            return Ok(result ? "true" : "false");
        }

        [HttpPost("SendEmail")]
        [Authorize(Roles = "System")]
        public async Task<ActionResult<string>> SendEmail([FromServices] IRepositories repositories)
        {
            var emailData = await APIBrokerHelper.GetDataFromRequestBody<EmailData>(Request);

            var loginManager = new LoginManager(repositories);
            await loginManager.SendEmail(emailData.Subject, emailData.Recipient, emailData.Body, emailData.IsHTML);

            return Ok(emailData);

        }

        [HttpGet("PostConfirmationCode")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<string>> PostConfirmationCode([FromQuery] int subjectId, [FromQuery] string confirmationCode, [FromServices] IRepositories repositories)
        {
            var loginManager = new LoginManager(repositories);
            await loginManager.PostConfirmationCode(subjectId, confirmationCode);

            return Ok(string.Empty);
        }

        [HttpGet("GetEmailByConfirmationCode")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<string>> GetEmailByConfirmationCode([FromQuery] string confirmationCode, [FromServices] IRepositories repositories)
        {
            var loginManager = new LoginManager(repositories);

            return Ok(await loginManager.GetEmailByConfirmationCode(confirmationCode));
        }

        [HttpGet("GetSubjectByConfirmationCode")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<SubjectData>> GetSubjectByConfirmationCode([FromQuery] string confirmationCode, [FromServices] IRepositories repositories)
        {
            var subjectManager = new SubjectManager(repositories);

            return Ok(await subjectManager.GetSubjectByConfirmationCode(confirmationCode));
        }

        [HttpPut("PostPassword")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<PasswordData>> PostPassword([FromServices] IRepositories repositories)
        {
            var passwordData = await APIBrokerHelper.GetDataFromRequestBody<PasswordData>(Request);

            var loginManager = new LoginManager(repositories);
            await loginManager.PostPassword(passwordData.ConfirmationCode, passwordData.Password, passwordData.Salt, passwordData.Initial);

            return Ok(passwordData);
        }

    }
}
