using Bamboo_card_currency_convertor.Models;
using Bamboo_card_currency_convertor.Repositories.Interface;
using Bamboo_card_currency_convertor.Utilities;
using Bamboo_card_currency_convertor.Utilities.Helper;

namespace Bamboo_card_currency_convertor.Repositories
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly HttpClient _httpClient;

        public CurrencyRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(Constant.BaseURL);
        }

        public async Task<CurrencyConversionResponse> GetLatestConversion(string? from = null, List<string>? to = null)
        {
            var queryParam = new List<string>();

            //from currency check
            if (!string.IsNullOrEmpty(from))
            {
                queryParam.Add($"base={from}");
            }

            //to currency check
            if (to != null && to.Any())
            {
                var toCurrencies = string.Join(",", to);
                queryParam.Add($"symbols={toCurrencies}");
            }

            var queryParamString = string.Join("&", queryParam);
            var url = $"latest?{queryParamString}";

            var response = await _httpClient.GetAsync(url);

            return await ValidateAndReturnResponse<CurrencyConversionResponse>(response);
        }

        private async Task<T> ValidateAndReturnResponse<T>(HttpResponseMessage responseMessage)
        {
            if (!responseMessage.IsSuccessStatusCode)
            {
                throw new Exception($"");
            }

            var content = await responseMessage.Content.ReadAsStringAsync();

            var jsonResponse = JsonHelper.Deserialize<T>(content);

            if (jsonResponse == null)
            {
                throw new Exception("Json deserialie failed.");
            }
            return jsonResponse;
        }
    }
}
