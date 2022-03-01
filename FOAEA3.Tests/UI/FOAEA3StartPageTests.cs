using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using FOAEA3.Tests.UI.Pages;

namespace FOAEA3.Tests.UI
{
    public class FOAEA3StartPageTests
    {
        [Fact]
        public void HomeIndexTest()
        {
            // arrange
            using IWebDriver driver = new InternetExplorerDriver();

            // act
            UITestHelper.LoginToFOAEA(driver);

            // assert
            Assert.Equal("Home Page - FOAEA", driver.Title);

        }
        
        [Fact]
        public void HomeIndexQuickSearchByTracingCategoryTest()
        {
            // arrange
            using IWebDriver driver = new InternetExplorerDriver();

            var homePage = new HomeIndexPage(driver);

            UITestHelper.LoginToFOAEA(driver);

            // act

            homePage.SearchCategory = "Tracing Application";
            homePage.Search();

            // assert
            Assert.Equal("Search Results - FOAEA", driver.Title);

        }
        //[Fact]
        //public void HomeIndexCreateT01Test()
        //{
        //    // arrange
        //    using IWebDriver driver = new InternetExplorerDriver();
            
        //    HomeIndexPage homeIndex = new HomeIndexPage(driver);

        //    // act
        //    homeIndex.CreateT01();

        //    // assert
        //    Assert.Equal("T01 - FOAEA", driver.Title);

        //}

    }
}
