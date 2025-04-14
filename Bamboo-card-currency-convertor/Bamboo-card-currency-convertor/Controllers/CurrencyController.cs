using Bamboo_card_currency_convertor.Models.Request;
using Bamboo_card_currency_convertor.Services.Interface;
using Bamboo_card_currency_convertor.Utilities;
using Bamboo_card_currency_convertor.Utilities.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bamboo_card_currency_convertor.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CurrencyController(ILogger<CurrencyController> logger, ICurrencyService currencyService) : ControllerBase
    {
        private readonly ILogger<CurrencyController> _logger = logger;

        private readonly ICurrencyService _currencyService = currencyService;

        [HttpGet("latest")]
        [Authorize(Roles = Roles.Manager)]
        public async Task<IActionResult> GetLatestRates([FromQuery] string baseCurrency = "EUR")
        {
            if (CurrencyHelper.IsRestricted(baseCurrency)) return BadRequest("Restricted currency.");
            var result = await _currencyService.GetLatestRatesAsync(baseCurrency);
            return Ok(result);
        }

        [HttpPost("convert")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> ConvertCurrency([FromBody] CurrencyConversionRequest request)
        {
            if (CurrencyHelper.IsRestricted(request.From) || CurrencyHelper.IsRestricted(request.To)) return BadRequest("Restricted currency.");
            var result = await _currencyService.ConvertCurrencyAsync(request);
            return Ok(result);
        }

        [HttpGet("historical")]
        [Authorize(Roles = Roles.User)]
        public async Task<IActionResult> GetHistoricalRates([FromQuery] HistoricalRatesRequest request)
        {
            if (CurrencyHelper.IsRestricted(request.BaseCurrency)) return BadRequest("Restricted currency.");
            var result = await _currencyService.GetHistoricalRatesAsync(request);
            return Ok(result);
        }
    }
}
