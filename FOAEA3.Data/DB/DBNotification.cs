using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FOAEA3.Data.DB
{
    internal class DBNotification : DBbase, INotificationRepository
    {
        public DBNotification(IDBToolsAsync mainDB) : base(mainDB)
        {

        }

        public async Task SendEmailAsync(string subject, string recipient, string body, int isHTML = 1)
        {
            var parameters = new Dictionary<string, object>
            {
                {"subjectval", subject },
                {"recipientsval", recipient },
                {"bodyval", body },
                {"ishtml", isHTML.ToString() }
            };

            _ = await MainDB.ExecProcAsync("PasswordResetSendEmail", parameters);
        }

        public async Task SendHtmlEmailAsync(string subject, string recipients, string body, string attachmentPath)
        {
            var parameters = new Dictionary<string, object>
            {
                {"sSubject", subject },
                {"sBody", body },
                {"sRecipients", recipients },
                {"sAttachmentPath", attachmentPath }
            };

            _ = await MainDB.ExecProcAsync("SendHtmlMailMessage", parameters);
        }

    }
}
