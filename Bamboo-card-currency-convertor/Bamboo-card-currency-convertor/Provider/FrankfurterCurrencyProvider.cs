using Bamboo_card_currency_convertor.Models.Request;
using Bamboo_card_currency_convertor.Models.Response;
using Bamboo_card_currency_convertor.Provider.Interface;
using Bamboo_card_currency_convertor.Utilities;
using Bamboo_card_currency_convertor.Utilities.Helper;
using System.Text.Json;

namespace Bamboo_card_currency_convertor.Provider
{
    public class FrankfurterCurrencyProvider : ICurrencyProvider
    {
        private readonly HttpClient _httpClient;

        public FrankfurterCurrencyProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<ExchangeRateResponse> ConvertCurrencyAsync(CurrencyConversionRequest request)
        {
            var url = $"{Constant.BaseURL}/latest?amount={request.Amount}";
            if (!string.IsNullOrEmpty(request.From)) url += $"&from={request.From}";
            if (!string.IsNullOrEmpty(request.To)) url += $"&to={request.To}";

            var response = await _httpClient.GetStringAsync(url);
            return JsonHelper.Deserialize<ExchangeRateResponse>(response);
        }

        public async Task<PagedResult<ExchangeRateResponse>> GetHistoricalRatesAsync(HistoricalRatesRequest request)
        {
            var url = $"{Constant.BaseURL}/{request.From:yyyy-MM-dd}..{request.To:yyyy-MM-dd}?from={request.BaseCurrency}";
            var response = await _httpClient.GetStringAsync(url);
            var allData = JsonSerializer.Deserialize<ExchangeRateResponse>(response);

            // Manual pagination logic
            return new PagedResult<ExchangeRateResponse>
            {
                Items = [allData],
                TotalCount = 1,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };
        }

        public async Task<ExchangeRateResponse> GetLatestRatesAsync(string baseCurrency)
        {
            var response = await _httpClient.GetStringAsync($"{Constant.BaseURL}/latest?from={baseCurrency}");
            return JsonHelper.Deserialize<ExchangeRateResponse>(response);
        }
    }
}
