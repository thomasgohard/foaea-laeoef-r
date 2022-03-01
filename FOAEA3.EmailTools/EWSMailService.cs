using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.Exchange.WebServices.Data;

namespace FOAEA3.EmailTools
{
    public class EWSMailService
    {
        private readonly string MailServer;

        public EWSMailService(string mailServer)
        {
            MailServer = mailServer;
        }

        public string SendMail(string message, string subject, string emails, string filePath = null, bool deleteFile = false)
        {

            var recipients = new List<string>();

            if (emails.Contains(";"))
                recipients.AddRange(emails.Split(';'));
            else
                recipients.Add(emails);

            try
            {
                var Exchange = new ExchangeService(ExchangeVersion.Exchange2016)
                {
                    UseDefaultCredentials = true
                };
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                Exchange.Url = new Uri(MailServer);

                var msg = new EmailMessage(Exchange)
                {
                    Subject = subject,
                    Body = message
                };
                if (!string.IsNullOrEmpty(filePath))
                    msg.Attachments.AddFileAttachment(filePath);

                foreach (string address in recipients)
                    msg.ToRecipients.Add(address);

                msg.SendAndSaveCopy();

                if ((!string.IsNullOrEmpty(filePath)) && deleteFile)
                    File.Delete(filePath);

                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public static bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

    }
}
