using FOAEA3.API.broker;
using FOAEA3.Data.Base;
using FOAEA3.Helpers;
using FOAEA3.Model;
using FOAEA3.Resources;
using FOAEA3.Resources.Helpers;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using NJsonSchema;
using Serilog;
using System;

namespace FOAEA3.Controllers
{

    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            if (ReferenceData.Instance().Messages.Count > 0)
            {
                ViewBag.FailureMessage = ReferenceData.Instance().Messages;
                return View("FailureToStart");
            }
            else
            {
                //return View("Index");
                if (string.IsNullOrEmpty(SessionData.FOAEAUser))
                {
                    return base.RedirectToAction("Login");
                }
                else
                {
                    var schema = JsonSchema.FromType<TracingApplicationData>();
                    var schemaJson = schema.ToJson();

                    ViewBag.jsonSchemaTestData = schemaJson;

                    return base.View("Index");
                }
            }

        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            return View("Dashboard");
        }

        public IActionResult SwitchLanguage(string returnUrl)
        {
            string newCulture = LanguageHelper.ENGLISH_LANGUAGE;
            if (LanguageHelper.IsEnglish())
                newCulture = LanguageHelper.FRENCH_LANGUAGE;

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(newCulture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        public string GetDBInfo()
        {

            static string GetDBValue(string info)
            {
                string result = info;

                string[] values = info.Split('=');
                if (values.Length == 2)
                    result = values[1];

                return result;
            }

            string database = HttpContext.Session.Get<string>("database");
            string server = HttpContext.Session.Get<string>("server");

            if (string.IsNullOrEmpty(database))
            {
                var infoAPI = new InfoAPI();
                string connectionString = infoAPI.GetMainDBConnectionString().ToLower();
                //string apiVersion = infoAPI.GetVersion();
                string[] dbInfo = connectionString.Split(';');

                foreach (string info in dbInfo)
                {
                    if (info.StartsWith("server", StringComparison.OrdinalIgnoreCase))
                        server = GetDBValue(info).ToUpper();
                    else if (info.StartsWith("database", StringComparison.OrdinalIgnoreCase))
                        database = GetDBValue(info).ToUpper();
                }

                HttpContext.Session.Set<string>("database", database);
                HttpContext.Session.Set<string>("server", server);
            }

            string result = $@"{server}/{database}";

            return result;

        }

        [HttpGet]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            ILogger log = Log.ForContext("EnfSrvCd", SessionData.CurrentEnforcementServiceCode)
                             .ForContext("SubmitterCode", SessionData.CurrentSubmitter);
            if (exceptionHandlerPathFeature != null)
            {
                log.Error(exceptionHandlerPathFeature.Error, "Exception in {path}", exceptionHandlerPathFeature.Path);
            }

            return View();
        }

        [HttpGet]
        public IActionResult ErrorDev()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            ILogger log = Log.ForContext("EnfSrvCd", SessionData.CurrentEnforcementServiceCode)
                             .ForContext("SubmitterCode", SessionData.CurrentSubmitter);
            if (exceptionHandlerPathFeature != null)
            {
                log.Error(exceptionHandlerPathFeature.Error, "Exception in {path}", exceptionHandlerPathFeature.Path);
            }

            return View(exceptionHandlerPathFeature);
        }

        [HttpGet]
        public IActionResult SystemInfo()
        {
            return View();
        }

        public IActionResult Login()
        {
            var loginInfo = new LoginData();

            if (!string.IsNullOrEmpty(SessionData.ReturnMessage))
            {
                loginInfo.Messages.AddError(SessionData.ReturnMessage);

                SessionData.ReturnMessage = string.Empty;
            }
            return View(loginInfo);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string userName, string password)
        {

            userName = userName.Trim();

            var subjectsAPI = new SubjectsAPI();
            var subject = subjectsAPI.GetSubject(userName);

            if (PasswordHelper.IsValidPassword(password, subject.PasswordSalt, subject.Password))
            {
                if (SubjectHelper.IsPasswordExpired(subject))
                {
                    SessionData.ReturnMessage = ErrorResource.ACCOUNT_PASSWORD_CHANGE_REQUIRED;

                    SessionData.TempAccess = userName.ToUpper();

                    return RedirectToAction("ChangePasswordEntry");
                }

            }
            else
            {
                SessionData.ReturnMessage = ErrorResource.INVALID_USERNAME_OR_PASSWORD;

                return Login();
            }

            if (ModelState.IsValid)
            {
                string redirectAction = LoginHelper.ProcessLogin(subject, HttpContext.Session.Id, out string user);

                if (!string.IsNullOrEmpty(user))
                    SessionData.TempAccess = user;

                SessionData.FOAEAUser = subject.SubjectName;
                SessionData.DefaultRoleName = subject.defaultRoleName;
                SessionData.Subject = subject;

                var subjectRolesAPI = new SubjectRolesAPI();

                SessionData.AssumedRoles = subjectRolesAPI.GetAssumedRolesForSubject(subject.SubjectName);

                return RedirectToAction(redirectAction);
            }
            else
            {
                return View();
            }

        }

        public IActionResult TermsOfReference()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TermsViewed()
        {
            var subjectsAPI = new SubjectsAPI();

            SubjectData subject = subjectsAPI.GetSubject(SessionData.FOAEAUser);

            if (subject.HasAcceptedNewTermsOfRef == false)
            {
                subject.HasAcceptedNewTermsOfRef = true;

                _ = subjectsAPI.AcceptNewTermsOfReference(subject);
            }

            SessionData.TermViewed = "TRUE";

            return RedirectToAction("AssumeRole");
        }

        public IActionResult AssumeRole()
        {
            var model = new AssumeRoleData();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AssumeRole(IFormCollection collection)
        {
            SessionData.CurrentSubmitter = collection["AssumedRole"];

            var submitterAPI = new SubmittersAPI();

            var submitterData = submitterAPI.GetSubmitter(collection["AssumedRole"]);

            submitterData.Subm_LstLgn_Dte = DateTime.Now;

            submitterAPI.UpdateSubmitter(submitterData);

            SessionData.LastLogin = submitterData.Subm_LstLgn_Dte;
            SessionData.CurrentEnforcementServiceCode = submitterData.EnfSrv_Cd;

            return RedirectToAction("Index");
        }

        public IActionResult ChangePasswordVerification()
        {
            if (!string.IsNullOrEmpty(SessionData.ReturnMessage))
            {
                ViewBag.Message = SessionData.ReturnMessage;
                SessionData.ReturnMessage = string.Empty;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePasswordVerification(IFormCollection collection)
        {
            string oldPassword = collection["oldPassword"];
            string subjectName = SessionData.FOAEAUser;

            if (string.IsNullOrEmpty(subjectName))
                subjectName = SessionData.TempAccess;

            var subjectsAPI = new SubjectsAPI();
            SubjectData subjectChangePassword = subjectsAPI.GetSubject(subjectName);

            if (PasswordHelper.IsValidPassword(oldPassword, subjectChangePassword.PasswordSalt, subjectChangePassword.Password))
            {
                return RedirectToAction("ChangePasswordEntry");
            }
            else
            {
                SessionData.ReturnMessage = ErrorResource.INVALID_PASSWORD;

                return RedirectToAction("ChangePasswordVerification");
            }
        }
        public IActionResult ChangePasswordEntry()
        {
            if (!string.IsNullOrEmpty(SessionData.ReturnMessage))
            {
                ViewBag.Message = SessionData.ReturnMessage;
                SessionData.ReturnMessage = string.Empty;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePasswordEntry(IFormCollection collection)
        {
            string newPassword = collection["newPassword"];
            string confirmPassword = collection["confirmPassword"];
            string subjectName = SessionData.FOAEAUser;
            if (string.IsNullOrEmpty(subjectName)) subjectName = SessionData.TempAccess;

            var subjectsAPI = new SubjectsAPI();
            SubjectData subject = subjectsAPI.GetSubject(subjectName);

            string encryptedNewPassword = PasswordHelper.EncodePassword(newPassword, subject.PasswordSalt);

            if (newPassword.Equals(confirmPassword, StringComparison.OrdinalIgnoreCase))
            {
                var loginsAPI = new LoginsAPI();

                if (!PasswordHelper.IsValidPasswordCharacters(newPassword))
                {
                    SessionData.ReturnMessage = LanguageResource.CHANGE_PASSWORD_FAILS_REQUIREMENTS;
                    return RedirectToAction("ChangePasswordEntry");

                }
                else if (loginsAPI.CheckPreviousPasswords(subjectName, encryptedNewPassword))
                {
                    SessionData.ReturnMessage = LanguageResource.PASSWORD_USED;
                    return RedirectToAction("ChangePasswordEntry");

                }
                else
                {
                    if (!PasswordHelper.ValidatePasswordWithUserName(subjectName, newPassword))
                    {
                        SessionData.ReturnMessage = LanguageResource.CHANGE_PASSWORD_FAILS_REQUIREMENTS;
                        return RedirectToAction("ChangePasswordEntry");
                    }
                    else
                    {
                        if (ModelState.IsValid)
                        {
                            PasswordHelper.EncryptPassword(newPassword, out string encryptedPassword, out string Salt);

                            subject.PasswordSalt = Salt;
                            _ = loginsAPI.SetPassword(subject, encryptedPassword);

                            SessionData.FOAEAUser = subject.SubjectName.ToUpper();

                            return RedirectToAction("TermsOfReference");

                        }
                        else
                        {
                            return View();
                        }
                    }
                }
            }
            else
            {
                SessionData.ReturnMessage = Resources.LanguageResource.CHANGE_PASSWORD_MATCH_ERROR;
                return RedirectToAction("ChangePasswordEntry");
            }
        }
        public IActionResult ResetPassword()
        {

            if (!string.IsNullOrEmpty(SessionData.ReturnMessage))
            {
                ViewBag.Message = SessionData.ReturnMessage;
                SessionData.ReturnMessage = string.Empty;
            }

            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(IFormCollection collection)
        {
            string subjectName = (string)collection["SubjectName"];
            string confirmationCode = (string)collection["confirmationCode"];

            if (string.IsNullOrEmpty(subjectName))
            {
                SessionData.ReturnMessage = ErrorResource.INVALID_LOGIN;

                ViewBag.Exception = SessionData.ReturnMessage;
                return View();
            }
            else
            {
                var subjectsAPI = new SubjectsAPI();

                SubjectData subject = subjectsAPI.GetSubject(subjectName);

                if (string.IsNullOrEmpty(subject.SubjectName))
                {
                    SessionData.ReturnMessage = ErrorResource.INVALID_LOGIN;
                    ViewBag.Exception = SessionData.ReturnMessage;
                    return View();

                }
                else if (!subject.AllowedAccess)
                {
                    SessionData.ReturnMessage = ErrorResource.INVALID_LOGIN;
                    ViewBag.Exception = SessionData.ReturnMessage;
                    return View();

                }
                else if (!ValidationHelper.IsValidEmail(subject.EMailAddress))
                {
                    SessionData.ReturnMessage = $"{ErrorResource.INVALID_LOGIN_EMAIL}:  {subject.EMailAddress}.  {ErrorResource.ERROR_EMAIL_FORMAT}.";
                    ViewBag.Exception = SessionData.ReturnMessage;
                    return View();

                }
                else if (!string.IsNullOrEmpty(confirmationCode) && (subject.ConfirmationCode != confirmationCode))
                {
                    SessionData.ReturnMessage = ErrorResource.INVALID_CONFIRMATION;
                    ViewBag.Exception = SessionData.ReturnMessage;
                    return View();
                }
                else
                {
                    if (string.IsNullOrEmpty(confirmationCode))
                    {
                        PasswordHelper.SendUserResetEmail(subject.SubjectName, subject.SubjectId, subject.EMailAddress);
                        subject.ConfirmationCode = "";
                        return View(subject);
                    }
                    else
                    {
                        SessionData.ConfirmationCode = confirmationCode;
                        return RedirectToAction("ConfirmReset");

                    }

                }
            }


        }
        public IActionResult ConfirmReset()
        {

            string id = SessionData.ConfirmationCode;

            ViewBag.Email = PasswordHelper.GetEmailByConfirmationCode(id);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmReset(IFormCollection collection)
        {
            string confirmationCode = SessionData.ConfirmationCode;

            var loginAPI = new LoginsAPI();
            SubjectData subjectToConfirm = loginAPI.GetSubjectByConfirmationCode(confirmationCode);

            PasswordHelper.SendUserConfirmResetEmail(subjectToConfirm.ConfirmationCode, subjectToConfirm.EMailAddress);

            return RedirectToAction("Index");
        }

        public IActionResult Logout(string id)
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index");
        }

        public string GetFOAEAVersion()
        {
            return GetType().Assembly.GetName().Version.ToString();
        }
    }
}
