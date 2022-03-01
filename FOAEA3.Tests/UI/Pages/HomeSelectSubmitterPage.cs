using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Tests.UI.Pages
{
    public class HomeSelectSubmitterPage
    {
        private readonly IWebDriver driver;

        public HomeSelectSubmitterPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        public void SelectSubmitter(string submitter)
        {
            var submitterSelect = new SelectElement(driver.FindElement(By.Name("Roles")));
            submitterSelect.SelectByText(submitter);

            var selectSubmitterButton = driver.FindElement(By.CssSelector("input[type=Submit]"));
            selectSubmitterButton.Click();

            System.Threading.Thread.Sleep(2000);

        }
    }
}
