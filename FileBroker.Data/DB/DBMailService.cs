using DBHelper;
using FileBroker.Model.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace FileBroker.Data.DB
{

    public class DBMailService : IMailServiceRepository
    {
        private IDBTools MainDB { get; }

        public DBMailService(IDBTools mainDB)
        {
            MainDB = mainDB;
        }

        public string SendEmail(string subject, string recipients, string body, string attachmentPath = null)
        {
         
            var parameters = new Dictionary<string, object>
            {
                {"subject", subject },
                {"body", body },
                {"recipients", recipients }
            };

            if (!string.IsNullOrEmpty(attachmentPath))
            {
                int attachmentId = UploadAttachmentToDatabase(attachmentPath);

                if (!string.IsNullOrEmpty(MainDB.LastError))
                    return MainDB.LastError;

                parameters.Add("attachmentId", attachmentId);
            }

            MainDB.ExecProc("SendHtmlMailMessageWithAttachment", parameters);

            if (!string.IsNullOrEmpty(MainDB.LastError))
                return MainDB.LastError;

            return "";
        }

        private int UploadAttachmentToDatabase(string attachmentPath)
        {

            string attachmentFileName = Path.GetFileName(attachmentPath);
            string attachmentContent = File.ReadAllText(attachmentPath);

            var parameters = new Dictionary<string, object>
            {
                {"AttachmentFileName", attachmentFileName},
                {"AttachmentContent", attachmentContent }
            };

            decimal returnValue = MainDB.GetDataFromProcSingleValue<decimal>("EmailAttachment_Insert", parameters);

            return (int)returnValue;

        }

    }
}
