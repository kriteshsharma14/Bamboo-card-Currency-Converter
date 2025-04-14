using Bamboo_card_currency_convertor.Factory.Interface;
using Bamboo_card_currency_convertor.Models.Request;
using Bamboo_card_currency_convertor.Models.Response;
using Bamboo_card_currency_convertor.Services.Interface;
using Microsoft.Extensions.Caching.Memory;

namespace Bamboo_card_currency_convertor.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyProviderFactory _factory;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CurrencyService> _logger;

        private readonly string providername = "frankfurter";

        public CurrencyService(ICurrencyProviderFactory factory, IMemoryCache cache, ILogger<CurrencyService> logger)
        {
            _factory = factory;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ExchangeRateResponse> GetLatestRatesAsync(string baseCurrency)
        {
            var cacheKey = $"latest_{baseCurrency}";
            if (_cache.TryGetValue(cacheKey, out ExchangeRateResponse cached))
                return cached;

            var provider = _factory.GetProvider(providername);
            var result = await provider.GetLatestRatesAsync(baseCurrency);
            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
            return result;
        }

        public async Task<ExchangeRateResponse> ConvertCurrencyAsync(CurrencyConversionRequest request)
        {
            var cacheKey = $"convert_{request.From}_{request.To}_{request.Amount}";
            if (_cache.TryGetValue(cacheKey, out ExchangeRateResponse cached))
                return cached;

            var provider = _factory.GetProvider(request.Provider);
            var result = await provider.ConvertCurrencyAsync(request);
            if (result.Rates.Any())
            {
                result.ConvertedAmount = result.Rates.FirstOrDefault(c => c.Key.Equals(request.From)).Value;
            }
            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));
            return result;
        }

        public async Task<PagedResult<ExchangeRateResponse>> GetHistoricalRatesAsync(HistoricalRatesRequest request)
        {
            //var cacheKey = $"hist_{request.BaseCurrency}_{request.From}_{request.To}_{request.Page}_{request.PageSize}";
            //if (_cache.TryGetValue(cacheKey, out PagedResult<ExchangeRateResponse> cached))
            //    return cached;

            var provider = _factory.GetProvider(providername);
            var result = await provider.GetHistoricalRatesAsync(request);
            //_cache.Set(cacheKey, result, TimeSpan.FromMinutes(30));
            return result;
        }
    }
}
