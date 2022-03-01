using FOAEA3.Tests.UI.Pages;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;
using System;
using TechTalk.SpecFlow;
using Xunit;
using static FOAEA3.Tests.UI.UITestHelper;

namespace FOAEA3.Tests.UI
{
    [Binding]
    public class OpenTracingApplicationSteps
    {
        private IWebDriver driver;
        private TracingIndexPage tracingIndexPage;

        [Given(@"I have logged in")]
        public void GivenIHaveLoggedIn()
        {
            // driver = new InternetExplorerDriver();

            //ScenarioContext.Current.Pending();
        }

        [Given(@"Accepted the Terms of Reference")]
        public void AcceptedTheTermsOfReference()
        {
            //driver = new InternetExplorerDriver();

            //ScenarioContext.Current.Pending();
        }

        [Given(@"Selected the ON2D68 submitter")]
        public void SelectSubmitterON2D68()
        {
            //driver = new InternetExplorerDriver();

            //ScenarioContext.Current.Pending();
        }

        [Given(@"I have done a search for Tracing Applications")]
        public void GivenIHaveDoneASearchForTracingApplications()
        {
            driver = new InternetExplorerDriver();

            var homePage = new HomeIndexPage(driver);

            UITestHelper.LoginToFOAEA(driver);

            // act

            homePage.SearchCategory = "Tracing Application";
            homePage.Search();

        }

        [Given(@"I see a list of Tracing applications")]
        public void GivenISeeAListOfTracingApplications()
        {
        }

        [When(@"I click on an application control code link in the list")]
        public void WhenIClickOnAnApplicationControlCodeLinkInTheList()
        {
            new WebDriverWait(driver, MAX_WAIT).Until(d => d.Title == "Search Results - FOAEA");

            var searchResultPage = new SearchResultPage(driver);
            tracingIndexPage = searchResultPage.OpenAppplication("AB01-E37711");
        }

        [Then(@"the tracing details page opens")]
        public void ThenTheTracingDetailsPageOpens()
        {
            try
            {
                new WebDriverWait(driver, MAX_WAIT).Until(d => d.Title == "T01 - FOAEA");
                Assert.Equal("E37711", tracingIndexPage.ControlCode);
            }
            finally
            {
                driver.Dispose();
            }
        }

        [Given(@"I clicked on an application control code link in the list")]
        public void GivenIClickedOnAnApplicationControlCodeLinkInTheList()
        {
            //ScenarioContext.Current.Pending();
        }

        [Given(@"The Tracing details page opened")]
        public void GivenTheTracingDetailsPageOpened()
        {
            //ScenarioContext.Current.Pending();
        }

        [When(@"I change the second name of the person to be located")]
        public void WhenIChangeTheSecondNameOfThePersonToBeLocated()
        {
            //ScenarioContext.Current.Pending();
        }

        [When(@"I save the changes")]
        public void WhenISaveTheChanges()
        {
            //ScenarioContext.Current.Pending();
        }

        [Then(@"I get a confirmation that the save was successful")]
        public void ThenIGetAConfirmationThatTheSaveWasSuccessful()
        {
            //ScenarioContext.Current.Pending();
        }

        [Given(@"I am on the start page of FOAEA")]
        public void GivenIAmOnTheStartPageOfFOAEA()
        {
            //ScenarioContext.Current.Pending();
        }

        [When(@"I click on the T01 Quick Create button")]
        public void WhenIClickOnTheTQuickCreateButton()
        {
            //ScenarioContext.Current.Pending();
        }

        [When(@"I enter the key information for the new person to locate")]
        public void WhenIEnterTheKeyInformationForTheNewPersonToLocate()
        {
            //ScenarioContext.Current.Pending();
        }

        [Then(@"Close the browser")]
        public void CloseTheBrowser()
        {
            try
            {
                driver.Close();
                driver.Dispose();
            }
            catch 
            {

            }
        }

    }
}
