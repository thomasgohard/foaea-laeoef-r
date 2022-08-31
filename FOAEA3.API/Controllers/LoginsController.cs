using FOAEA3.Business.Security;
using FOAEA3.Common.Helpers;
using FOAEA3.Data.DB;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class LoginsController : ControllerBase
    {
        [HttpGet("Version")]
        public ActionResult<string> GetVersion() => Ok("Logins API Version 1.0");

        [HttpGet("DB")]
        public ActionResult<string> GetDatabase([FromServices] IRepositories repositories) => Ok(repositories.MainDB.ConnectionString);

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
        public async Task<ActionResult<string>> SendEmail([FromServices] IRepositories repositories)
        {
            var emailData = await APIBrokerHelper.GetDataFromRequestBodyAsync<EmailData>(Request);

            var loginManager = new LoginManager(repositories);
            await loginManager.SendEmailAsync(emailData.Subject, emailData.Recipient, emailData.Body, emailData.IsHTML);

            return Ok(emailData);

        }

        [HttpGet("PostConfirmationCode")]
        public async Task<ActionResult<string>> PostConfirmationCode([FromQuery] int subjectId, [FromQuery] string confirmationCode, [FromServices] IRepositories repositories)
        {
            var dbLogin = new DBLogin(repositories.MainDB);

            await dbLogin.PostConfirmationCodeAsync(subjectId, confirmationCode);

            return Ok(string.Empty);
        }

        [HttpGet("GetEmailByConfirmationCode")]
        public async Task<ActionResult<string>> GetEmailByConfirmationCode([FromQuery] string confirmationCode, [FromServices] IRepositories repositories)
        {
            var dbLogin = new DBLogin(repositories.MainDB);

            return Ok(await dbLogin.GetEmailByConfirmationCodeAsync(confirmationCode));
        }

        [HttpGet("GetSubjectByConfirmationCode")]
        public async Task<ActionResult<SubjectData>> GetSubjectByConfirmationCode([FromQuery] string confirmationCode, [FromServices] IRepositories repositories)
        {
            var dbSubject = new DBSubject(repositories.MainDB);

            return Ok(await dbSubject.GetSubjectByConfirmationCodeAsync(confirmationCode));
        }

        [HttpPut("PostPassword")]
        public async Task<ActionResult<PasswordData>> PostPassword([FromServices] IRepositories repositories)
        {
            var passwordData = await APIBrokerHelper.GetDataFromRequestBodyAsync<PasswordData>(Request);

            var dbLogin = new DBLogin(repositories.MainDB);
            await dbLogin.PostPasswordAsync(passwordData.ConfirmationCode, passwordData.Password, passwordData.Salt, passwordData.Initial);

            return Ok(passwordData);
        }



    }
}
