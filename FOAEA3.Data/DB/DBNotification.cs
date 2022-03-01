using DBHelper;
using FOAEA3.Data.Base;
using FOAEA3.Model.Interfaces;
using System.Collections.Generic;

namespace FOAEA3.Data.DB
{
    internal class DBNotification : DBbase, INotificationRepository
    {
        public DBNotification(IDBTools mainDB) : base(mainDB)
        {

        }

        public void SendEmail(string subject, string recipient, string body, int isHTML = 1)
        {
            var parameters = new Dictionary<string, object>
            {
                {"subjectval", subject },
                {"recipientsval", recipient },
                {"bodyval", body },
                {"ishtml", isHTML.ToString() }
            };

            _ = MainDB.ExecProc("PasswordResetSendEmail", parameters);
        }

        public void SendHtmlEmail(string subject, string recipients, string body, string attachmentPath)
        {
            var parameters = new Dictionary<string, object>
            {
                {"sSubject", subject },
                {"sBody", body },
                {"sRecipients", recipients },
                {"sAttachmentPath", attachmentPath }
            };

            _ = MainDB.ExecProc("SendHtmlMailMessage", parameters);
        }

    }
}
