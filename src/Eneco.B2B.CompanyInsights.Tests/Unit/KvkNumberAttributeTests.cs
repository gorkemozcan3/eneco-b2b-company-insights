using Eneco.B2B.CompanyInsights.Api.Utils.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Eneco.B2B.CompanyInsights.Tests.Unit
{
    public class KvkNumberAttributeTests
    {
        private static ValidationResult? Validate(string? kvkNumber)
        {
            var attribute = new KvkNumberAttribute();
            var context = new ValidationContext(new object()) { MemberName = "kvkNumber" };

            return attribute.GetValidationResult(kvkNumber, context);
        }

        [Theory]
        [InlineData("12345678")]
        [InlineData("12345679")]
        [InlineData("87654326")]
        public void ValidKvkNumber_ReturnsSuccess(string kvkNumber)
        {
            Assert.Equal(ValidationResult.Success, Validate(kvkNumber));
        }

        [Theory]
        [InlineData(null, "KVK number must be exactly 8 characters long.")]
        [InlineData("1234567", "KVK number must be exactly 8 characters long.")]
        [InlineData("1234567A", "KVK number must contain only numeric characters.")]
        public void InvalidKvkNumber_ReturnsHelperErrorMessage(string? kvkNumber, string expectedMessage)
        {
            var result = Validate(kvkNumber);

            Assert.NotNull(result);
            Assert.Equal(expectedMessage, result!.ErrorMessage);
        }
    }
}
