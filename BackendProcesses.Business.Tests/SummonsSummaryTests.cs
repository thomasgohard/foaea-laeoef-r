using System.Threading.Tasks;
using TestData.Data;
using TestData.TestDataBase;
using Xunit;

namespace BackendProcesses.Business.Tests
{
    public class SummonsSummaryTests
    {
        public SummonsSummaryTests()
        {
            ApplicationTestData.SetupApplicationTestData(2);
            InterceptionFinHoldbackTestData.SetupInterceptionFinHoldbackTestData();
            SummSmryTestData.SetupAmountOwedTestData();

            GarnPeriodTestData.SetupGarnPeriodTestData();
            SummFAFRTestData.SetupSummFAFRTestData();
            SummDFTestData.SetupSummDFTestData();
        }

        [Fact]
        public async Task SummonsSummary_GetAmountOwedRecords()
        {
            // Arrange
            var inMemRepositoryFinance = new InMemory_RepositoriesFinance();
            var summSmryDB = inMemRepositoryFinance.SummonsSummaryRepository;

            // Act
            var data = await summSmryDB.GetAmountOwedRecordsAsync();

            // Assert
            Assert.Single(data);
        }


    }
}
