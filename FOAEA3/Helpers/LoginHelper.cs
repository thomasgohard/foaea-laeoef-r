using FOAEA3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FOAEA3.Helpers
{
    public class LoginHelper
    {
        static internal string ProcessLogin(SubjectData subject, string sessionID, out string tempAccess)
        {
            tempAccess = "";

            if (!subject.IsTrusted)
            {
                tempAccess = subject.SubjectName;

                return "ChangePasswordVerification";
            }
            else
            {
                //Session[LoginData.CRDP_User] = userName.ToUpper();

                //subject.LastLogin = DateTime.Now;
                //UserManager.UpdateSubmNoValidation(user);

                //ConnectionLog(userName.ToUpper(), Session);

                //Session[SessionHelper.CRDP_USER_NAME] = userName.ToUpper();
                //Session[SessionHelper.SESSION_KEY] = Guid.NewGuid().ToString();
                //Session[SessionHelper.SESSION_ID] = Session.SessionID;
                //Session[SessionHelper.SESSION_TIMED_OUT] = false;
                //Session[SessionHelper.CONNECTION_STRING] = DBHelper.ConnectionSetting().ConnectionString;

                //SessionHelper.CreateLoginSession(Session[SessionHelper.CRDP_USER_NAME].ToString(), Session[SessionHelper.SESSION_KEY].ToString(), Session[SessionHelper.SESSION_ID].ToString());

                return "TermsOfReference";
            }

        }
    }
}
