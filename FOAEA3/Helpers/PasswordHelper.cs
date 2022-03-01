using FOAEA3.API.broker;
using FOAEA3.API.Helpers;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace FOAEA3.Helpers
{
    public class PasswordHelper
    {
        //private readonly IRepositories Repositories;
        public const string ConfirmationCode = "ConfirmationCode";
        // private static RNGCryptoServiceProvider m_cryptoServiceProvider = null;
        private const int SALT_SIZE = 24;


        private static string GenerateConfirmationCode()
        {
            return Guid.NewGuid().ToString();
        }
        public static void EncryptPassword(string pwd, out string newPassword, out string salt)
        {
            salt = GenerateSalt();
            newPassword = EncodePassword(pwd, salt);
        }
        //public static string GetHashedPassword(string salt, string password)
        //{
        //    return GetPasswordHashAndSalt(password + salt);
        //}
        //public static string GeneratePasswordHash(out string plainTextPassword, out string salt, string password = "")
        //{
        //    if (string.IsNullOrEmpty(password))
        //    {
        //        PasswordGenerator generator = new PasswordGenerator();
        //        plainTextPassword = generator.Generate();
        //    }
        //    else
        //    {
        //        plainTextPassword = password;
        //    }
        //    salt = GenerateSalt();

        //    return GetPasswordHashAndSalt(plainTextPassword + salt);
        //}
        //private static string GetPasswordHashAndSalt(string message)
        //{

        //    SHA256 sha = new SHA256CryptoServiceProvider();
        //    byte[] dataBytes = StringHelper.GetBytes(message);
        //    byte[] resultBytes = sha.ComputeHash(dataBytes);

        //    // return the hash string to the caller
        //    return Convert.ToBase64String(resultBytes);
        //}
        //private static string GetSaltString()
        //{
        //    m_cryptoServiceProvider = new RNGCryptoServiceProvider();

        //    // Lets create a byte array to store the salt bytes
        //    byte[] saltBytes = new byte[SALT_SIZE];

        //    // lets generate the salt in the byte array
        //    m_cryptoServiceProvider.GetNonZeroBytes(saltBytes);

        //    // Let us get some string representation for this salt
        //    string saltString = Convert.ToBase64String(saltBytes);

        //    // Now we have our salt string ready lets return it to the caller
        //    return saltString;
        //}
        public static bool IsValidPassword(string enteredPassword, string salt, string existingEncryptedPassword)
        {
            string enteredPasswordEncrypted = EncodePassword(enteredPassword, salt);

            return (enteredPasswordEncrypted.Equals(existingEncryptedPassword, StringComparison.OrdinalIgnoreCase));
        }

        internal static string EncodePassword(string pass, string salt)
        {
            //if (passwordFormat == 0)
            //{
            //    // MembershipPasswordFormat.Clear()
            //    return pass;
            //}

            var bIn = Encoding.Unicode.GetBytes(pass);
            var bSalt = Convert.FromBase64String(salt);
            var bAll = new byte[bSalt.Length + (bIn.Length - 1) + 1];
            byte[] bRet = null;
            Buffer.BlockCopy(bSalt, 0, bAll, 0, bSalt.Length);
            Buffer.BlockCopy(bIn, 0, bAll, bSalt.Length, bIn.Length);
            //if (passwordFormat == 1)
            //{
            //MembershipPasswordFormat.Hashed
            //Dim s As HashAlgorithm = HashAlgorithm.Create(Membership.HashAlgorithmType)
            // Dim s As SHA1 = SHA1.Create()
            using (SHA256 s = SHA256CryptoServiceProvider.Create())
            {
                bRet = s.ComputeHash(bAll);
            }
            //}
            //else
            //{
            //    bRet = EncryptPassword(bAll);
            //}

            return Convert.ToBase64String(bRet);
        }

        internal static string GenerateSalt()
        {
            var buf = new byte[SALT_SIZE];
            {
                using var withBlock = new RNGCryptoServiceProvider();
                withBlock.GetBytes(buf);
            }

            return Convert.ToBase64String(buf);
        }

        public void GenerateNewPassword(string subjectName, ref string newPassword, ref string newSalt)
        {
            string pass = string.IsNullOrEmpty(newPassword) ? subjectName.Substring(0, subjectName.IndexOf("_", StringComparison.Ordinal) - 1) : newPassword;
            string salt = GenerateSalt();
            newSalt = salt;
            newPassword = EncodePassword(pass, salt);
        }

        //public static string SendUserCreatedEmail(HttpRequestBase Request, string SubmCd, string EMail)
        //{
        //    string Password;
        //    string clearPassword;
        //    string Salt;
        //    string HashPassword = GeneratePasswordHash(out Password, out Salt);
        //    clearPassword = Password;

        //    string body = "";
        //    body += "<p><i>Le texte français suit</i></p>";
        //    body += "<p><b>PLEASE DO NOT REPLY TO THIS EMAIL – THIS EMAIL IS NOT CLOSELY MONITORED </b><br /></p>";
        //    body += "<p>An account was created in the CRDP system for you: " + SubmCd + "<br /></p>";
        //    body += "<p>Your temporary password is: " + clearPassword + "<br /></p>";
        //    body += "Please access the application with this temporary password. You will then be asked to change such password. <br />";

        //    body += "For any questions regarding the content of this email, please contact Divorce@justice.gc.ca <br />";
        //    body += "<p><br /><br /><b>SVP NE PAS RÉPONDRE À CE MESSAGE – CETTE BOÎTE COURRIEL N'EST PAS SURVEILLÉE DE PRÈS</b><br /></p>";

        //    body += "<p>Un compte a été créé dans le système de BEAD en votre nom: " + SubmCd + "<br /></p>";
        //    body += "<p>Votre mot de passe temporaire est: " + clearPassword + "<br /></p>";
        //    body += "Veuillez suivre le lien plus bas afin d'accéder au système avec votre mot de passe temporaire. Vous devrez ensuite changer ce mot de passe.<br />";
        //    body += "Pour toutes questions concernant le contenu de ce courriel, SVP envoyez un courriel à Divorce@justice.gc.ca <br />";

        //    LoginSecurityHelper.SendEmail("CRDP Account Created / Compte créé pour le système de BEAD", EMail, body);
        //    LoginSecurityHelper.PostPassword(SubmCd, HashPassword, Salt);

        //    return clearPassword;
        //}

        public static string GetEmailByConfirmationCode(string confirmationCode)
        {
            var loginsAPI = new LoginsAPI();

            return loginsAPI.GetEmailByConfirmationCode(confirmationCode);
        }

        public static string SendUserResetEmail(string subjectName, int subjectId, string emailAddress)
        {
            // Reset Password
            string body = "";

            string gID = GenerateConfirmationCode();
            body += "<p><i>Le texte français suit</i></p>";
            body += "<p><b>PLEASE DO NOT REPLY TO THIS EMAIL – THIS EMAIL IS NOT CLOSELY MONITORED</b><br /></p>";
            body += "A request was received to reset your FOAEA password.<br />";
            body += "Please use the confirmation code below to complete your request.<br />";
            body += "<p>For any questions regarding the content of this email, please contact FOAEA_AEOEF@justice.gc.ca</p>";
            body += "<p><br /><br /><b>SVP NE PAS RÉPONDRE À CE MESSAGE – CETTE BOÎTE COURRIEL N'EST PAS SURVEILLÉE DE PRÈS</b><br /></p>";
            body += "Une demande a été reçue pour réinitialiser votre mot de passe pour le système d'AEOEF.<br />";
            body += "Veuillez utiliser le code de confirmation plus bas pour compléter votre demande.<br />";
            body += "<p>Pour toutes questions concernant le contenu de ce courriel, SVP envoyez un courriel à FOAEA_AEOEF@justice.gc.ca<br /></p> ";
            body += gID;

            var loginsAPI = new LoginsAPI();

            loginsAPI.PostConfirmationCode(subjectId, gID);
            loginsAPI.SendEmail("Confirmation of request for FOAEA Password Reset / Confirmation de demande pour réinitialisation de votre mot de passe du système d'AEOEF",
                                emailAddress, body);

            return $"{Resources.LanguageResource.RESET_EMAIL_SENT}: {subjectName}";
        }

        public static string SendUserConfirmResetEmail(string confirmationCode, string emailAddress)
        {
            // Confirm Reset Password

            string password = PasswordGenerator.Generate();
            EncryptPassword(password, out string newPassword, out string salt);

            string body = "";
            //string passWord = PasswordGenerator.Generate();
            body += "<p><i>Le texte français suit</i></p>";
            body += "<p><b>PLEASE DO NOT REPLY TO THIS EMAIL – THIS EMAIL IS NOT CLOSELY MONITORED</b><br /></p>";
            body += "Your password has been reset.<br />";
            body += "Your temporary password is: " + password + "<br />";
            body += "<br />Please follow the link below to login with this temporary password. You will then be asked to change such password.<br /> ";
            body += "You must login within 48 business hours; otherwise, you will have to submit a new request to reset your password.<br />";
            body += "Your password should never be shared with anyone.<br />";
            body += "<p>For any questions regarding the content of this email, please contact FOAEA_AEOEF@justice.gc.ca </p>";
            body += "<p><br /><br /><b>SVP NE PAS RÉPONDRE À CE MESSAGE – CETTE BOÎTE COURRIEL N'EST PAS SURVEILLÉE DE PRÈS</b><br /></p>";
            body += "Votre mot de passe a été réinitialisé.<br />";
            body += "Votre mot de passe temporaire est: " + password + "<br />";
            body += "<br />Veuillez suivre le lien plus bas afin d'accéder au système avec votre mot de passe temporaire. Vous devrez ensuite changer ce mot de passe.<br />";
            body += "Vous devez accéder au système dans les 48 heures (2 jours ouvrables); sinon, vous devrez soumettre une nouvelle demande de réinitialisation.<br />";
            body += "Votre mot de passe ne doit pas être partagé.<br />";
            body += "<p>Pour toutes questions concernant le contenu de ce courriel, SVP envoyez un courriel à FOAEA_AEOEF@justice.gc.ca</p> ";
            ;

            var loginsAPI = new LoginsAPI();
            loginsAPI.SendEmail("New Temporary FOAEA Password / Nouveau mot de passe temporaire pour le système d'AEOEF", emailAddress, body);
            loginsAPI.PostPassword(confirmationCode, newPassword, salt, "1");

            return password;
        }

        public static bool IsValidPasswordCharacters(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            // Return Regex.IsMatch(password, "(?=^.{8,}$)(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*()_+}{":;'?/>.<,])(?!.*\s).*$")
            // Return Regex.IsMatch(password, "(?=^.{8,}$)((?=.*\d)(?=.*[a-z])(?=.*[A-Z]))|((?=.*\d)(?=.*[a-z])(?=.*[!@#$%^&*()_+}{":;'?/>.<,]))|((?=.*\d)(?=.*[A-Z])(?=.*[!@#$%^&*()_+}{":;'?/>.<,]))|((?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*()_+}{":;'?/>.<,]))(?!.*\s).*$")
            // 3 of 5 is 10 combinations
            if (Regex.IsMatch(password, "(?=^.{8,}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\\s).*$"))
            {
                // any number, any lower case letter, any upper case letter
                return true;
            }
            else if (Regex.IsMatch(password, "(?=^.{8,}$)(?=.*\\d)(?=.*[a-z])(?=.*[!@#$%^&*()_+}{\x022:;\'?/>.<,])(?!.*\\s).*$"))
            {
                // any number, any lower case letter, any special character
                return true;
            }
            else if (Regex.IsMatch(password, "(?=^.{8,}$)(?=.*\\d)(?=.*[a-z])(?=.*\\p{Lo})(?!.*\\s).*$"))
            {
                // any number, any lower case letter, any unicode non upper or lower case
                return true;
            }
            else if (Regex.IsMatch(password, "(?=^.{8,}$)(?=.*\\d)(?=.*[A-Z])(?=.*[!@#$%^&*()_+}{\x022:;\'?/>.<,])(?!.*\\s).*$"))
            {
                // any number, any upper case letter, any special character
                return true;
            }
            else if (Regex.IsMatch(password, "(?=^.{8,}$)(?=.*\\d)(?=.*[A-Z])(?=.*\\p{Lo})(?!.*\\s).*$"))
            {
                // any number, any upper case letter, any unicode non upper or lower case
                return true;
            }
            else if (Regex.IsMatch(password, "(?=^.{8,}$)(?=.*\\d)(?=.*[!@#$%^&*()_+}{\x022:;\'?/>.<,])(?=.*\\p{Lo})(?!.*\\s).*$"))
            {
                // any number, any special character, any unicode non upper or lower case
                return true;
            }
            else if (Regex.IsMatch(password, "(?=^.{8,}$)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*()_+}{\x022:;\'?/>.<,])(?!.*\\s).*$"))
            {
                //  any lower case letter, any upper case letter, any special character
                return true;
            }
            else if (Regex.IsMatch(password, "(?=^.{8,}$)(?=.*[a-z])(?=.*[A-Z])(?=.*\\p{Lo})(?!.*\\s).*$"))
            {
                // any lower case letter, any upper case letter, any unicode non upper or lower case
                return true;
            }
            else if (Regex.IsMatch(password, "(?=^.{8,}$)(?=.*[a-z])(?=.*[!@#$%^&*()_+}{\x022:;\'?/>.<,])(?=.*\\p{Lo})(?!.*\\s).*$"))
            {
                // any lower case letter, any special character, any unicode non upper or lower case
                return true;
            }
            else if (Regex.IsMatch(password, "(?=^.{8,}$)(?=.*[A-Z])(?=.*[!@#$%^&*()_+}{\x022:;\'?/>.<,])(?=.*\\p{Lo})(?!.*\\s).*$"))
            {
                // any upper case letter, any special character, any unicode non upper or lower case
                return true;
            }
            else
            {
                return false;
            }

        }

        public static bool ValidatePasswordWithUserName(string username, string password)
        {
            bool valid = true;
            if (username.Contains("_"))
            {
                string[] usernamearray;
                usernamearray = username.Split('_');
                foreach (string splitName in usernamearray)
                {
                    if ((splitName.Length > 2))
                    {
                        if (password.ToUpperInvariant().Contains(splitName.ToUpperInvariant())) // https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca1308?view=vs-2019
                        {
                            valid = false;
                        }
                    }
                }
            }
            return valid;
        }
    }
}
