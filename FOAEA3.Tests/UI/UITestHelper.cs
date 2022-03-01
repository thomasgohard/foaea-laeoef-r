using FOAEA3.Tests.UI.Pages;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Tests.UI
{
    public class UITestHelper
    {
        public static readonly TimeSpan MAX_WAIT = new TimeSpan(hours: 0, minutes: 0, seconds: 10);

        public static void LoginToFOAEA(IWebDriver driver)
        {
            var loginPage = new HomeLoginPage(driver);
            var torPage = new HomeTermsOfReferencePage(driver);
            var submitterSelectionPage = new HomeSelectSubmitterPage(driver);

            loginPage.Go("http://%FOAEA_API_SERVER%:12020/Home/Login");
            loginPage.Login("system_support", "shared");

            torPage.Accept();

            submitterSelectionPage.SelectSubmitter("ON2D68");
        }

    }


}
