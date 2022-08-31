using FOAEA3.Resources.Helpers;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace FOAEA3.Data.Tests.Helpers
{
    [ExcludeFromCodeCoverage]
    public class StringHelperTests
    {
        [Fact]
        public void TestSplitKeysPassTwoValues()
        {
            // arrange
            string value = "1234-5678";

            // act
            var applKey = new ApplKey(value);

            // assert
            Assert.True(applKey.EnfSrv == "1234" && applKey.CtrlCd == "5678");
        }

        [Fact]
        public void TestSplitKeysPassSingleValue()
        {
            // arrange
            string value = "1234";

            // act
            var applKey = new ApplKey(value);

            // assert
            Assert.True(applKey.EnfSrv == string.Empty && applKey.CtrlCd == string.Empty);
        }

    }
}
