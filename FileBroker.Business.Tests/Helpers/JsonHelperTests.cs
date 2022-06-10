using FileBroker.Business.Helpers;
using FileBroker.Model;
using System.Collections.Generic;
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
            var result = JsonHelper.Validate<MEPInterceptionFileData>(source, out List<UnknownTag> unknownTags);

            // assert
            Assert.Single(result);
            Assert.Empty(unknownTags);
        }

        [Fact]
        public void Validate_MEPInterceptionFileData_Test2()
        {
            // arrange
            var fileName = @"TestDataFiles\IncomingInterceptionTest1.json";
            string source = File.ReadAllText(fileName);

            // act
            var result = JsonHelper.Validate<MEPInterceptionFileData>(source, out List<UnknownTag> unknownTags);

            // assert
            Assert.Empty(result);
            Assert.Empty(unknownTags);
        }
        
        [Fact]
        public void Validate_MEPInterceptionFileData_Test3()
        {
            // arrange
            var fileName = @"TestDataFiles\IncomingInterceptionTest2.json";
            string source = File.ReadAllText(fileName);

            // act
            var result = JsonHelper.Validate<MEPInterceptionFileData>(source, out List<UnknownTag> unknownTags);

            // assert
            Assert.Empty(result);
            Assert.Single(unknownTags);
            Assert.Contains("INTAPPIN10", unknownTags[0].Section);
            Assert.Contains("extra_tag", unknownTags[0].Tag);
        }

        [Fact]
        public void Validate_MEPInterceptionFileData_Test4()
        {
            // arrange
            var fileName = @"TestDataFiles\IncomingInterceptionTest3.json";
            string source = File.ReadAllText(fileName);

            // act
            var result = JsonHelper.Validate<MEPInterceptionFileData>(source, out List<UnknownTag> unknownTags);

            // assert
            Assert.Empty(result);
            Assert.Single(unknownTags);
            Assert.Contains("INTAPPIN12", unknownTags[0].Section);
            Assert.Contains("dat_IntFinH_NextRecalc_Dte2", unknownTags[0].Tag);
        }
    }
}
