namespace Eneco.B2B.CompanyInsights.Api.Utils
{
    /// <summary>
    /// Central place for input-format rules shared by the API boundary.
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Checks whether <paramref name="kvkNumber"/> is a well-formed KVK number: exactly eight ASCII digits.
        /// </summary>
        /// <param name="kvkNumber">The candidate KVK number; empty and whitespace are invalid.</param>
        /// <returns>
        /// A tuple whose <c>IsValid</c> reports the outcome and it is empty when the value is valid.
        /// </returns>
        public static (bool IsValid, string ErrorMessage) IsValidKvkNumber(string? kvkNumber)
        {
            // Check if the KVK number is exactly 8 characters long
            if (string.IsNullOrWhiteSpace(kvkNumber) || kvkNumber.Length != 8)
            {
                return (false, "KVK number must be exactly 8 characters long.");
            }

            // Check if the KVK number contains only ASCII numeric characters.
            if (!kvkNumber.All(c => c is >= '0' and <= '9'))
            {
                return (false, "KVK number must contain only numeric characters.");
            }

            // If both checks pass, the KVK number is valid.
            return (true, string.Empty);
        }
    }
}
