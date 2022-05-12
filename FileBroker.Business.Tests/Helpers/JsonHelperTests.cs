using FileBroker.Business.Helpers;
using FileBroker.Model;
using System.IO;
using Xunit;

namespace FileBroker.Business.Tests.Helpers
{
    public class JsonHelperTests
    {
        [Fact]
        public void Validate_MEPInterceptionFileData_Test1()
        {
            // arrange
            string source = "";

            // act
            var result = JsonHelper.Validate<MEPInterceptionFileData>(source);

            // assert
            Assert.Single(result);
        }

        [Fact]
        public void Validate_MEPInterceptionFileData_Test2()
        {
            // arrange
            var fileName = @"TestDataFiles\IncomingInterceptionTest1.json";
            string source = File.ReadAllText(fileName);

            // act
            var result = JsonHelper.Validate<MEPInterceptionFileData>(source);

            // assert
            Assert.Empty(result);
        }
        
        [Fact]
        public void Validate_MEPInterceptionFileData_Test3()
        {
            // arrange
            var fileName = @"TestDataFiles\IncomingInterceptionTest2.json";
            string source = File.ReadAllText(fileName);

            // act
            var result = JsonHelper.Validate<MEPInterceptionFileData>(source);

            // assert
            Assert.Single(result);
            Assert.Contains("unknown tag [extra_tag]", result[0]);
        }
    }
}
