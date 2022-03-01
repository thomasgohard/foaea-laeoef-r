using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.API.broker
{
    public class LoginsAPI : BaseAPI
    {
        internal bool CheckPreviousPasswords(string subjectName, string encryptedNewPassword)
        {
            return GetStringAsync($"api/v1/Logins/CheckPreviousPasswords?subjectName={subjectName}&newPassword={encryptedNewPassword}").Result.ToLower() == "true";
        }

        internal SubjectData SetPassword(SubjectData subject, string encryptedPassword)
        {
            return PutDataAsync<SubjectData, SubjectData>($"api/v1/Logins/SetPassword?newPassword={encryptedPassword}", subject).Result;
        }

        internal string PostConfirmationCode(int subjectId, string gID)
        {
            return GetStringAsync($"api/v1/Logins/PostConfirmationCode?subject={subjectId}&confirmationCode={gID}").Result;
        }

        internal EmailData SendEmail(string subject, string recipient, string body, int isHTML = 1)
        {
            var emailData = new EmailData { 
                Subject = subject,
                Recipient = recipient,
                Body = body,
                IsHTML = isHTML
            };

            return PostDataAsync<EmailData, EmailData>("api/v1/Logins/SendEmail", emailData).Result;
        }

        internal string GetEmailByConfirmationCode(string confirmationCode)
        {
            return GetStringAsync($"api/v1/Logins/GetEmailByConfirmationCode?confirmationCode={confirmationCode}").Result;
        }

        internal SubjectData GetSubjectByConfirmationCode(string confirmationCode)
        {
            return GetDataAsync<SubjectData>($"api/v1/Logins/GetSubjectByConfirmationCode?confirmationCode={confirmationCode}").Result;
        }

        internal PasswordData PostPassword(string confirmationCode, string newPassword, string salt, string initial)
        {
            var passwordData = new PasswordData
            {
                ConfirmationCode = confirmationCode,
                Password = newPassword,
                Salt = salt,
                Initial = initial
            };

            return PutDataAsync<PasswordData, PasswordData>("PostPassword", passwordData).Result;
        }
    }
}
