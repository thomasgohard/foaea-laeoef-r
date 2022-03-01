using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Model.Interfaces
{
    public interface INotificationRepository
    {
        public string CurrentSubmitter { get; set; }
        public string UserId { get; set; }

        void SendEmail(string subject, string recipient, string body, int isHTML = 1);
        void SendHtmlEmail(string subject, string recipient, string body, string attachmentPath);
    }
}
