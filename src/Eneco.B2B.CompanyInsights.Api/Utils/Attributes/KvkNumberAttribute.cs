using System.ComponentModel.DataAnnotations;

namespace Eneco.B2B.CompanyInsights.Api.Utils.Attributes
{
    /// <summary>
    /// Validates that a value is a syntactically well-formed KVK number, that is, exactly eight ASCII digits.
    /// The rule itself lives in <see cref="ValidationHelper.IsValidKvkNumber"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class KvkNumberAttribute : ValidationAttribute
    {
        /// <summary>
        /// Applies the KVK format rule to <paramref name="value"/>.
        /// </summary>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var (isValid, errorMessage) = ValidationHelper.IsValidKvkNumber(value as string);

            return isValid
                ? ValidationResult.Success
                : new ValidationResult(errorMessage);
        }
    }
}
