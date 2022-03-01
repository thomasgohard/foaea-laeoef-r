using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace FOAEA3.Tests.UI.Pages
{
    public class HomeIndexPage
    {
        private readonly IWebDriver driver;
        public HomeIndexPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        public string SearchCategory
        {
            set
            {
                SelectElement categorySelect = new SelectElement(driver.FindElement(By.Name("Category")));
                categorySelect.SelectByText(value);
            }
        }

        public SearchResultPage Search()
        {
            IWebElement searchSubmit = driver.FindElement(By.Id("btnSubmit"));
            searchSubmit.SendKeys(Keys.Return);

            System.Threading.Thread.Sleep(8000);

            return new SearchResultPage(driver);
        }

        public TracingIndexPage CreateT01()
        {
            IWebElement createT01 = driver.FindElement(By.Id("CreateT01"));
            createT01.Click();

            return new TracingIndexPage(driver);
        }
    }
}
