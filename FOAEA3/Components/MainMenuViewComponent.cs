using FOAEA3.Business.Security;
using FOAEA3.Data.Base;
using FOAEA3.Helpers;
using FOAEA3.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.Components
{
    public class MainMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            // get current user
            // if no current user, redirect to login page

            // get security from DB or session
            var userSecurity = new UserSecurity()
            {
                IsFinancialOfficer = true,
                CanCreateT01 = true
            };
                       
            var model = new MainMenuViewModel
            {
                UserSecurityData = userSecurity,
                SubmitterCode = SessionData.CurrentSubmitter
            };

            // adjust menu based on security
            return View(model);
        }

    }
}
