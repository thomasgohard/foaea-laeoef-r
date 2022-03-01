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
        [HttpGet("CheckPreviousPasswords")]
        public ActionResult<string> CheckPreviousPasswords([FromQuery] string subjectName, [FromQuery] string encryptedNewPassword, [FromServices] IRepositories repositories)
        {
            var loginManager = new LoginManager(repositories);
            var result = loginManager.CheckPreviousPasswords(subjectName, encryptedNewPassword);

            return Ok(result ? "true" : "false");
        }

        [HttpPut("SetPassword")]
        public ActionResult<string> SetNewPassword([FromQuery] string encryptedNewPassword, [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var subject = APIBrokerHelper.GetDataFromRequestBody<SubjectData>(Request);

            var loginManager = new LoginManager(repositories);
            var result = loginManager.CheckPreviousPasswords(subject.SubjectName, encryptedNewPassword);

            return Ok(result ? "true" : "false");
        }

        [HttpPost("SendEmail")]
        public ActionResult<string> SendEmail([FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var emailData = APIBrokerHelper.GetDataFromRequestBody<EmailData>(Request);

            var loginManager = new LoginManager(repositories);
            loginManager.SendEmail(emailData.Subject, emailData.Recipient, emailData.Body, emailData.IsHTML);

            return Ok(emailData);

        }

        [HttpGet("PostConfirmationCode")]
        public ActionResult<string> PostConfirmationCode([FromQuery] int subjectId, [FromQuery] string confirmationCode, [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var dbLogin = new DBLogin(repositories.MainDB);

            dbLogin.PostConfirmationCode(subjectId, confirmationCode);

            return Ok(string.Empty);
        }

        [HttpGet("GetEmailByConfirmationCode")]
        public ActionResult<string> GetEmailByConfirmationCode([FromQuery] string confirmationCode, [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var dbLogin = new DBLogin(repositories.MainDB);

            return Ok(dbLogin.GetEmailByConfirmationCode(confirmationCode));
        }

        [HttpGet("GetSubjectByConfirmationCode")]
        public ActionResult<SubjectData> GetSubjectByConfirmationCode([FromQuery] string confirmationCode, [FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var dbSubject = new DBSubject(repositories.MainDB);

            return Ok(dbSubject.GetSubjectByConfirmationCode(confirmationCode));
        }

        [HttpPut("PostPassword")]
        public ActionResult<PasswordData> PostPassword([FromServices] IRepositories repositories)
        {
            APIHelper.ApplyRequestHeaders(repositories, Request.Headers);
            APIHelper.PrepareResponseHeaders(Response.Headers);

            var passwordData = APIBrokerHelper.GetDataFromRequestBody<PasswordData>(Request);

            var dbLogin = new DBLogin(repositories.MainDB);
            dbLogin.PostPassword(passwordData.ConfirmationCode, passwordData.Password, passwordData.Salt, passwordData.Initial);

            return Ok(passwordData);
        }



    }
}
