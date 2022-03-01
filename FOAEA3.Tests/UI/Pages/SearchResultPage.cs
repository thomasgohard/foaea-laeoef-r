using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace FOAEA3.Tests.UI.Pages
{
    public class SearchResultPage
    {
        private readonly IWebDriver driver;

        public SearchResultPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        public TracingIndexPage OpenAppplication(string applKey)
        {
            try
            {
                IWebElement applicationLink = driver.FindElement(By.Id(applKey));
                string a = applicationLink.GetAttribute("id");
                applicationLink.Click();
            
                return new TracingIndexPage(driver);
            }
            catch (Exception e)
            {
                driver.Dispose();
                throw new Exception(e.Message);
            }

            
        }
    }
}
