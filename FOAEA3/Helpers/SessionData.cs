using FOAEA3.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FOAEA3.Helpers
{
    public static class SessionData
    {
        private const string FOAEA_USER = "FOAEA_User";
        private const string FOAEA_USER_DEFAULT_ROLE = "FOAEA_UserDefaultRole";
        private const string TERMS_VIEWED = "TermsOfReferenceViewed";
        private const string FOAEA_SUBJECT_DATA = "FOAEA_Subject_Data";
        private const string CURRENT_SUBMITTER = "CurrentSubmitter";
        private const string CURRENT_ENF_CODE = "CurrentEnforcementCode";
        private const string LAST_LOGIN = "LastLogin";
        private const string TEMP_ACCESS = "TempAccess";
        private const string ASSUMED_ROLES = "AssumedRoles";
        private const string RETURN_MESSAGE = "ReturnMessage";
        private const string CONFIRMATION_CODE = "ConfirmationCode";

        public const string SESSION_KEY = "SessionKey";
        public const string SESSION_ID = "SessionID";
        public const string SESSION_TIMED_OUT = "SessionTimedOut";
        public const string CONNECTION_STRING = "ConnectionString";
        public const string FOAEA_USER_NAME = "FOAEAUserName";

        public static string CurrentSubmitter
        {
            get
            {
                if ((SessionHelper.Current != null) && (SessionHelper.Current.Session.Keys.Contains(CURRENT_SUBMITTER)))
                    return SessionHelper.Current.Session.GetString(CURRENT_SUBMITTER);
                else
                    return string.Empty;
            }
            internal set
            {
                SessionHelper.Current.Session.SetString(CURRENT_SUBMITTER, value);
            }
        }

        public static SubjectData Subject
        {
            get
            {
                if ((SessionHelper.Current != null) && (SessionHelper.Current.Session.Keys.Contains(FOAEA_SUBJECT_DATA)))
                    return SessionHelper.Current.Session.Get<SubjectData>(FOAEA_SUBJECT_DATA);
                else
                    return default;
            }

            internal set
            {
                SessionHelper.Current.Session.Set<SubjectData>(FOAEA_SUBJECT_DATA, value);
            }
        }

        public static string CurrentEnforcementServiceCode
        {
            get
            {
                if ((SessionHelper.Current != null) && (SessionHelper.Current.Session.Keys.Contains(CURRENT_ENF_CODE)))
                    return SessionHelper.Current.Session.GetString(CURRENT_ENF_CODE);
                else
                    return string.Empty;
            }
            internal set
            {
                SessionHelper.Current.Session.SetString(CURRENT_ENF_CODE, value);
            }
        }

        public static DateTime? LastLogin
        {
            get
            {
                if ((SessionHelper.Current != null) && (SessionHelper.Current.Session.Keys.Contains(LAST_LOGIN)))
                    return SessionHelper.Current.Session.Get<DateTime?>(LAST_LOGIN);
                else
                    return null;
            }
            internal set
            {
                SessionHelper.Current.Session.Set<DateTime?>(LAST_LOGIN, value);
            }
        }

        public static string FOAEAUser
        {
            get
            {
                if ((SessionHelper.Current != null) && (SessionHelper.Current.Session.Keys.Contains(FOAEA_USER)))
                    return SessionHelper.Current.Session.GetString(FOAEA_USER);
                else
                    return string.Empty;
            }
            internal set
            {
                SessionHelper.Current.Session.SetString(FOAEA_USER, value);
            }
        }

        public static string DefaultRoleName
        {
            get
            {
                if ((SessionHelper.Current != null) && (SessionHelper.Current.Session.Keys.Contains(FOAEA_USER_DEFAULT_ROLE)))
                    return SessionHelper.Current.Session.GetString(FOAEA_USER_DEFAULT_ROLE);
                else
                    return string.Empty;
            }
            internal set
            {
                SessionHelper.Current.Session.SetString(FOAEA_USER_DEFAULT_ROLE, value);
            }
        }

        public static string TempAccess
        {
            get
            {
                if ((SessionHelper.Current != null) && (SessionHelper.Current.Session.Keys.Contains(TEMP_ACCESS)))
                    return SessionHelper.Current.Session.GetString(TEMP_ACCESS);
                else
                    return string.Empty;
            }
            internal set
            {
                SessionHelper.Current.Session.SetString(TEMP_ACCESS, value);
            }
        }

        public static string TermViewed
        {
            get
            {
                if ((SessionHelper.Current != null) && (SessionHelper.Current.Session.Keys.Contains(TERMS_VIEWED)))
                    return SessionHelper.Current.Session.GetString(TERMS_VIEWED);
                else
                    return string.Empty;
            }
            internal set
            {
                SessionHelper.Current.Session.SetString(TERMS_VIEWED, value);
            }
        }

        public static string ReturnMessage
        {
            get
            {
                if ((SessionHelper.Current != null) && (SessionHelper.Current.Session.Keys.Contains(RETURN_MESSAGE)))
                    return SessionHelper.Current.Session.GetString(RETURN_MESSAGE);
                else
                    return string.Empty;
            }
            internal set
            {
                SessionHelper.Current.Session.SetString(RETURN_MESSAGE, value);
            }
        }

        public static string ConfirmationCode
        {
            get
            {
                if ((SessionHelper.Current != null) && (SessionHelper.Current.Session.Keys.Contains(CONFIRMATION_CODE)))
                    return SessionHelper.Current.Session.GetString(CONFIRMATION_CODE);
                else
                    return string.Empty;
            }
            internal set
            {
                SessionHelper.Current.Session.SetString(CONFIRMATION_CODE, value);
            }
        }

        public static List<string> AssumedRoles
        {
            get
            {
                if ((SessionHelper.Current != null) && (SessionHelper.Current.Session.Keys.Contains(ASSUMED_ROLES)))
                    return SessionHelper.Current.Session.Get<List<string>>(ASSUMED_ROLES);
                else
                    return default;
            }
            internal set
            {
                SessionHelper.Current.Session.Set<List<string>>(ASSUMED_ROLES, value);
            }
        }

    }
}
