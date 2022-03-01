using FOAEA3.Resources.Helpers;
using Xunit;

namespace FOAEA3.Data.Tests.Helpers
{
    public class ValidationHelperTests
    {
        [Fact]
        public void TestIsValidEmailPass()
        {
            // Arrange
            string email = "test.test@gmail.com";

            // Act
            bool isValid = ValidationHelper.IsValidEmail(email);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void TestIsValidEmailFail()
        {
            // Arrange
            string email = "test.testgmail.com";

            // Act
            bool isValid = ValidationHelper.IsValidEmail(email);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void TestIsValidEmailOptionalMissingPass()
        {
            // Arrange
            string email = string.Empty;

            // Act
            bool isValid = ValidationHelper.IsValidEmail(email, true);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void TestIsValidEmailNonOptionalMissingFail()
        {
            // Arrange
            string email = string.Empty;

            // Act
            bool isValid = ValidationHelper.IsValidEmail(email, false);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void TestIsValidPhoneNumberPass()
        {
            // Arrange
            string phoneNumber = "321-1231";

            // Act
            bool isValid = ValidationHelper.IsValidPhoneNumber(phoneNumber);

            // Assert
            Assert.True(isValid);
        }

        [Theory]
        [InlineData("1-3421")]
        [InlineData("1-3")]
        [InlineData("")]
        public void TestIsValidPhoneNumbeFail(string phoneNumber)
        {
            // Arrange

            // Act
            bool isValid = ValidationHelper.IsValidPhoneNumber(phoneNumber);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void TestIsValidIntegerPass()
        {
            // Arrange
            string number = "123";

            // Act
            bool isValid = ValidationHelper.IsValidInteger(number);

            // Assert
            Assert.True(isValid);
        }

        [Theory]
        [InlineData("alpha")]
        [InlineData("alphaNum001")]
        [InlineData("")]
        [InlineData("5.6")]
        [InlineData("5.")]
        public void TestIsValidIntegerFail(string number)
        {
            // Arrange

            // Act
            bool isValid = ValidationHelper.IsValidInteger(number);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void TestIsValidSINPass()
        {
            // Arrange
            string number = "163656788";

            // Act
            bool isValid = ValidationHelper.IsValidSinNumberMod10(number);

            // Assert
            Assert.True(isValid);
        }

        [Theory]
        [InlineData("alpha")]
        [InlineData("alphaalph")]
        [InlineData("alphaNum1")]
        [InlineData("")]
        [InlineData("123456789")]
        public void TestIsValidSINFail(string number)
        {
            // Arrange

            // Act
            bool isValid = ValidationHelper.IsValidSinNumberMod10(number);

            // Assert
            Assert.False(isValid);
        }

    }
}
