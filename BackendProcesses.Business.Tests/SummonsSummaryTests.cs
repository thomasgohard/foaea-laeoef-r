using TestData.Data;
using TestData.TestDataBase;
using Xunit;

namespace BackendProcesses.Business.Tests
{
    public class SummonsSummaryTests
    {
        public SummonsSummaryTests()
        {
            ApplicationTestData.SetupApplicationTestData();
            InterceptionFinHoldbackTestData.SetupInterceptionFinHoldbackTestData();
            SummSmryTestData.SetupAmountOwedTestData();

            GarnPeriodTestData.SetupGarnPeriodTestData();
            SummFAFRTestData.SetupSummFAFRTestData();
            SummDFTestData.SetupSummDFTestData();
        }

        [Fact]
        public void SummonsSummary_GetAmountOwedRecords()
        {
            // Arrange
            var inMemRepositoryFinance = new InMemory_RepositoriesFinance();
            var summSmryDB = inMemRepositoryFinance.SummonsSummaryRepository;

            // Act
            var data = summSmryDB.GetAmountOwedRecords();

            // Assert
            Assert.Single(data);
        }


    }
}
