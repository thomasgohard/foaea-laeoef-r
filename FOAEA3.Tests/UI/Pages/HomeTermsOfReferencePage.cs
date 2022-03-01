using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Tests.UI.Pages
{
    public class HomeTermsOfReferencePage
    {
        private readonly IWebDriver driver;

        public HomeTermsOfReferencePage(IWebDriver driver)
        {
            this.driver = driver;
            // <title>TermsOfReference - FOAEA</title>
        }

        public void Accept()
        {
            var submitButton = driver.FindElement(By.CssSelector("input[value=Login]"));
            submitButton.Click();
        }
    }
}
