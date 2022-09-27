using Microsoft.AspNetCore.Identity.UI.Services;

namespace FOAEA3.IdentityManager.Areas.Identity
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            throw new NotImplementedException();
        }
    }
}
