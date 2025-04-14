using Bamboo_card_currency_convertor.Factory.Interface;
using Bamboo_card_currency_convertor.Models.Request;
using Bamboo_card_currency_convertor.Models.Response;
using Bamboo_card_currency_convertor.Provider.Interface;
using Bamboo_card_currency_convertor.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Bamboo_card_currency_convertor.Tests
{
    [TestFixture]
    public class CurrencyServiceTests
    {
        private Mock<ICurrencyProviderFactory> _mockFactory;
        private Mock<IMemoryCache> _mockCache;
        private Mock<ILogger<CurrencyService>> _mockLogger;
        private CurrencyService _currencyService;

        [SetUp]
        public void SetUp()
        {
            // Mock dependencies
            _mockFactory = new Mock<ICurrencyProviderFactory>();
            _mockCache = new Mock<IMemoryCache>();
            _mockLogger = new Mock<ILogger<CurrencyService>>();

            // Create instance of CurrencyService with mocked dependencies
            _currencyService = new CurrencyService(
                _mockFactory.Object,
                _mockCache.Object,
                _mockLogger.Object
            );
        }

        [Test]
        public async Task GetLatestRatesAsync_ShouldReturnRatesFromCache_WhenCacheExists()
        {
            // Arrange
            var baseCurrency = "USD";
            var cachedResponse = new ExchangeRateResponse
            {
                Base = "USD",
                Rates = new Dictionary<string, decimal> { { "EUR", 0.85m } }
            };

            // Mock cache to return a cached response
            _mockCache.Setup(m => m.TryGetValue(It.IsAny<string>(), out cachedResponse)).Returns(true);

            // Act
            var result = await _currencyService.GetLatestRatesAsync(baseCurrency);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(cachedResponse, result);
            _mockCache.Verify(m => m.TryGetValue(It.IsAny<string>(), out cachedResponse), Times.Once);
        }

        [Test]
        public async Task GetLatestRatesAsync_ShouldCallProvider_WhenCacheDoesNotExist()
        {
            // Arrange
            var baseCurrency = "USD";
            var expectedResponse = new ExchangeRateResponse
            {
                Base = "USD",
                Rates = new Dictionary<string, decimal> { { "EUR", 0.85m } }
            };

            // Mock cache miss
            _mockCache.Setup(m => m.TryGetValue(It.IsAny<string>(), out It.Ref<ExchangeRateResponse>.IsAny)).Returns(false);

            // Mock provider to return expected response
            var mockProvider = new Mock<ICurrencyProvider>();
            mockProvider.Setup(x => x.GetLatestRatesAsync(baseCurrency)).ReturnsAsync(expectedResponse);
            _mockFactory.Setup(x => x.GetProvider(It.IsAny<string>())).Returns(mockProvider.Object);

            // Act
            var result = await _currencyService.GetLatestRatesAsync(baseCurrency);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResponse, result);
            _mockCache.Verify(m => m.Set(It.IsAny<string>(), expectedResponse, It.IsAny<TimeSpan>()), Times.Once);
        }

        [Test]
        public async Task ConvertCurrencyAsync_ShouldReturnConvertedCurrency_WhenCacheExists()
        {
            // Arrange
            var request = new CurrencyConversionRequest
            {
                From = "USD",
                To = "EUR",
                Amount = 100m,
                Provider = "frankfurter"
            };

            var cachedResponse = new ExchangeRateResponse
            {
                Base = "USD",
                ConvertedAmount = 85m,
                Rates = new Dictionary<string, decimal> { { "USD", 1m }, { "EUR", 0.85m } }
            };

            // Mock cache to return a cached response
            _mockCache.Setup(m => m.TryGetValue(It.IsAny<string>(), out cachedResponse)).Returns(true);

            // Act
            var result = await _currencyService.ConvertCurrencyAsync(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(cachedResponse.ConvertedAmount, result.ConvertedAmount);
            _mockCache.Verify(m => m.TryGetValue(It.IsAny<string>(), out cachedResponse), Times.Once);
        }

        [Test]
        public async Task ConvertCurrencyAsync_ShouldCallProvider_WhenCacheDoesNotExist()
        {
            // Arrange
            var request = new CurrencyConversionRequest
            {
                From = "USD",
                To = "EUR",
                Amount = 100m,
                Provider = "frankfurter"
            };

            var expectedResponse = new ExchangeRateResponse
            {
                Base = "USD",
                ConvertedAmount = 85m,
                Rates = new Dictionary<string, decimal> { { "USD", 1m }, { "EUR", 0.85m } }
            };

            // Mock cache miss
            _mockCache.Setup(m => m.TryGetValue(It.IsAny<string>(), out It.Ref<ExchangeRateResponse>.IsAny)).Returns(false);

            // Mock provider to return expected response
            var mockProvider = new Mock<ICurrencyProvider>();
            mockProvider.Setup(x => x.ConvertCurrencyAsync(It.IsAny<CurrencyConversionRequest>())).ReturnsAsync(expectedResponse);
            _mockFactory.Setup(x => x.GetProvider(It.IsAny<string>())).Returns(mockProvider.Object);

            // Act
            var result = await _currencyService.ConvertCurrencyAsync(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResponse.ConvertedAmount, result.ConvertedAmount);
            _mockCache.Verify(m => m.Set(It.IsAny<string>(), expectedResponse, It.IsAny<TimeSpan>()), Times.Once);
        }

        [Test]
        public async Task GetHistoricalRatesAsync_ShouldReturnRates_WhenCalled()
        {
            // Arrange
            var request = new HistoricalRatesRequest
            {
                BaseCurrency = "USD",
                Page = 1,
                PageSize = 10
            };

            var expectedResponse = new PagedResult<ExchangeRateResponse>
            {
                Items = new List<ExchangeRateResponse>
                {
                    new ExchangeRateResponse
                    {
                        Base = "USD",
                        Rates = new Dictionary<string, decimal> { { "EUR", 0.85m } }
                    }
                },
                TotalCount = 1
            };

            // Mock provider to return expected response
            var mockProvider = new Mock<ICurrencyProvider>();
            mockProvider.Setup(x => x.GetHistoricalRatesAsync(It.IsAny<HistoricalRatesRequest>())).ReturnsAsync(expectedResponse);
            _mockFactory.Setup(x => x.GetProvider(It.IsAny<string>())).Returns(mockProvider.Object);

            // Act
            var result = await _currencyService.GetHistoricalRatesAsync(request);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Items.Count());
            Assert.AreEqual(expectedResponse.Items.First().Base, result.Items.First().Base);
            Assert.AreEqual(expectedResponse.Items.First().Rates.FirstOrDefault(), result.Items.First().Rates.FirstOrDefault());
        }
    }
}
