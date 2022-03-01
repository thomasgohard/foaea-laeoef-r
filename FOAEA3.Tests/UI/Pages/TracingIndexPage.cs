using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using static FOAEA3.Tests.UI.UITestHelper;

namespace FOAEA3.Tests.UI.Pages
{
    public class TracingIndexPage
    {
        private readonly IWebDriver driver;

        public TracingIndexPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        public string ControlCode
        {
            get
            {
                var wait = new WebDriverWait(driver, MAX_WAIT);
                IWebElement applCtrlCd = wait.Until(d => d.FindElement(By.Id("Tracing.Appl_CtrlCd")));

                //IJavaScriptExecutor exec = (IJavaScriptExecutor)driver;
                //string returnValue = (string)exec.ExecuteScript("return someOperations()");

                return applCtrlCd.GetAttribute("value");
            }
        }

        public void Save()
        {
            IWebElement saveSubmit = driver.FindElement(By.Id("btnSubmit"));
            saveSubmit.Click();
        }
    }
}
