using Eneco.B2B.CompanyInsights.Api.Dtos;
using Eneco.B2B.CompanyInsights.Api.Services.Interfaces;
using Eneco.B2B.CompanyInsights.Api.Utils.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Eneco.B2B.CompanyInsights.Api.Controllers
{
    /// <summary>
    /// Exposes company information and competitor pricing lookups by KvK number.
    /// </summary>
    [Route("api/companies")]
    [ApiController]
    [Produces("application/json")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        /// <summary>
        /// Retrieves the registered company details for the given KvK number.
        /// </summary>
        /// <remarks>
        /// Looks up the company in the external KvK Finder registry and returns its
        /// identifying and address information.
        /// </remarks>
        /// <param name="kvkNumber">
        /// The 8-digit Dutch Chamber of Commerce (KvK) number. Must contain only digits
        /// </param>
        /// <param name="cancellationToken"></param>
        /// <returns>The company details matching the supplied KvK number.</returns>
        /// <response code="200">The company was found and returned.</response>
        /// <response code="400">The supplied KvK number is not a valid KvK number.</response>
        /// <response code="404">No company is registered under the supplied KvK number.</response>
        /// <response code="500">An unexpected error occurred while processing the request.</response>
        /// <response code="502">The KvK Finder API could not be reached or returned an unusable response.</response>
        /// <response code="504">The KvK Finder API did not respond in time.</response>
        [ProducesResponseType(typeof(CompanyResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status502BadGateway)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status504GatewayTimeout)]
        [HttpGet("{kvkNumber}")]
        public async Task<IActionResult> GetCompanyByKvkNumber([KvkNumber] string kvkNumber, CancellationToken cancellationToken)
        {
            var company = await _companyService.GetCompanyAsync(kvkNumber, cancellationToken);

            return Ok(company);
        }

        /// <summary>
        /// Retrieves competitor electricity pricing offered to the company with the given KvK number.
        /// </summary>
        /// <remarks>
        /// Queries the external competitor pricing provider and returns only electricity
        /// products, identified by the presence of a price per kWh. Gas-only products are excluded.
        /// </remarks>
        /// <param name="kvkNumber">
        /// The 8-digit Dutch Chamber of Commerce (KvK) number. Must contain only digits
        /// </param>
        /// <param name="cancellationToken"></param>
        /// <returns>The competitor electricity prices associated with the supplied KvK number.</returns>
        /// <response code="200">Competitor pricing was found and returned. The price list may be empty when no electricity products are offered.</response>
        /// <response code="400">The supplied KvK number is not a valid KvK number.</response>
        /// <response code="404">No competitor pricing is available for the supplied KvK number.</response>
        /// <response code="500">An unexpected error occurred while processing the request.</response>
        /// <response code="502">The competitor pricing API could not be reached or returned an unusable response.</response>
        /// <response code="504">The competitor pricing API did not respond in time.</response>
        [ProducesResponseType(typeof(CompetitorPricingResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status502BadGateway)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status504GatewayTimeout)]
        [HttpGet("{kvkNumber}/competitor-pricing")]
        public async Task<IActionResult> GetCompetitorPricingByKvkNumber([KvkNumber] string kvkNumber, CancellationToken cancellationToken)
        {
            var competitorPricing = await _companyService.GetCompetitorPricingAsync(kvkNumber, cancellationToken);

            return Ok(competitorPricing);
        }
    }
}
