using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Tests.UI.Pages
{
    public class HomeLoginPage
    {
        private readonly IWebDriver driver;

        public HomeLoginPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        public void Go(string url)
        {
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl(url);
        }

        public void Login(string username, string password)
        {
            var usernameInputBox = driver.FindElement(By.Name("UserName"));
            usernameInputBox.SendKeys(username);

            var passwordInputBox = driver.FindElement(By.Name("Password"));
            passwordInputBox.SendKeys(password);

            var submitButton = driver.FindElement(By.CssSelector("input[value=Login]"));
            submitButton.Click();

            // searchSubmit.SendKeys(Keys.Return);
            //System.Threading.Thread.Sleep(2000);
        }
    }
}
