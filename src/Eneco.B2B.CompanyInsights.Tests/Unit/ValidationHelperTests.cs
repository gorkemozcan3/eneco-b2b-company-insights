using Eneco.B2B.CompanyInsights.Api.Utils;

namespace Eneco.B2B.CompanyInsights.Tests.Unit
{
    public class ValidationHelperTests
    {
        [Theory]
        [InlineData("12345678")]
        [InlineData("87654326")]
        [InlineData("10000003")]
        public void IsValidKvkNumber_EightDigits_ReturnsTrueWithoutErrorMessage(string kvkNumber)
        {
            // Act
            var (isValid, errorMessage) = ValidationHelper.IsValidKvkNumber(kvkNumber);

            // Assert
            Assert.True(isValid);
            Assert.Equal(string.Empty, errorMessage);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("1234567")]
        [InlineData("123456789")]
        public void IsValidKvkNumber_NumberIsNotEightCharactersLong_ReturnsLengthErrorMessage(string? kvkNumber)
        {
            // Act
            var (isValid, errorMessage) = ValidationHelper.IsValidKvkNumber(kvkNumber!);

            // Assert
            Assert.False(isValid);
            Assert.Equal("KVK number must be exactly 8 characters long.", errorMessage);
        }

        [Theory]
        [InlineData("1234567A")]
        [InlineData("-1234567")]
        public void IsValidKvkNumber_NumberContainsNonNumericCharacters_ReturnsNumericErrorMessage(string kvkNumber)
        {
            // Act
            var (isValid, errorMessage) = ValidationHelper.IsValidKvkNumber(kvkNumber);

            // Assert
            Assert.False(isValid);
            Assert.Equal("KVK number must contain only numeric characters.", errorMessage);
        }
    }
}
