using Bamboo_card_currency_convertor.Models.Request;
using Bamboo_card_currency_convertor.Models.Response;

namespace Bamboo_card_currency_convertor.Services.Interface
{
    public interface ICurrencyService
    {
        Task<ExchangeRateResponse> GetLatestRatesAsync(string baseCurrency);
        Task<ExchangeRateResponse> ConvertCurrencyAsync(CurrencyConversionRequest request);
        Task<PagedResult<ExchangeRateResponse>> GetHistoricalRatesAsync(HistoricalRatesRequest request);
    }
}


