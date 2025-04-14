using Bamboo_card_currency_convertor.Models.Request;
using Bamboo_card_currency_convertor.Models.Response;
using Bamboo_card_currency_convertor.Provider;
using Bamboo_card_currency_convertor.Utilities.Helper;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using System.Net;

namespace Bamboo_card_currency_convertor.UnitTests.Provider
{
    public class FrankfurterCurrencyProviderTests
    {
        private HttpClient _httpClient;
        private Mock<HttpMessageHandler> _httpHandlerMock;
        private FrankfurterCurrencyProvider _provider;

        [SetUp]
        public void Setup()
        {
            _httpHandlerMock = new Mock<HttpMessageHandler>();

            _httpClient = new HttpClient(_httpHandlerMock.Object)
            {
                BaseAddress = new Uri("https://api.frankfurter.app/")
            };

            _provider = new FrankfurterCurrencyProvider(_httpClient);
        }

        [Test]
        public async Task ConvertCurrencyAsync_ReturnsExpectedResult()
        {
            // Arrange
            var expected = new ExchangeRateResponse
            {
                Base = "EUR",
                ConvertedAmount = 100,
                Rates = new Dictionary<string, decimal> { { "USD", 107.21m } },
                Date = DateTime.Today
            };

            var responseContent = JsonHelper.Serialize<ExchangeRateResponse>(expected);

            _httpHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent)
                });

            var request = new CurrencyConversionRequest
            {
                Amount = 100,
                From = "EUR",
                To = "USD"
            };

            // Act
            var result = await _provider.ConvertCurrencyAsync(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("EUR", result.Base);
            Assert.AreEqual(107.21m, result.Rates["USD"]);
        }

        [Test]
        public async Task GetLatestRatesAsync_ReturnsExpectedResult()
        {
            var expected = new ExchangeRateResponse
            {
                Base = "EUR",
                ConvertedAmount = 1,
                Rates = new Dictionary<string, decimal> { { "INR", 89.23m } },
                Date = DateTime.Today
            };

            var json = JsonHelper.Serialize<ExchangeRateResponse>(expected);

            _httpHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json)
                });

            var result = await _provider.GetLatestRatesAsync("EUR");

            Assert.IsNotNull(result);
            Assert.AreEqual("EUR", result.Base);
            Assert.AreEqual(89.23m, result.Rates["INR"]);
        }

        [Test]
        public async Task GetHistoricalRatesAsync_ReturnsPagedResult()
        {
            var mockResponse = new ExchangeRateResponse
            {
                Base = "EUR",
                ConvertedAmount = 1,
                Rates = new Dictionary<string, decimal> { { "USD", 1.11m } },
                Date = DateTime.Today
            };

            var json = JsonHelper.Serialize<ExchangeRateResponse>(mockResponse);

            _httpHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                   ItExpr.IsAny<HttpRequestMessage>(),
                   ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(json)
                });

            var request = new HistoricalRatesRequest
            {
                From = DateTime.Today.AddDays(-10),
                To = DateTime.Today,
                BaseCurrency = "EUR",
                Page = 1,
                PageSize = 10
            };

            var result = await _provider.GetHistoricalRatesAsync(request);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.TotalCount);
            Assert.AreEqual(mockResponse.Base, result.Items.First().Base);
        }
    }
}
