using CurrencyConverterApi.Services;
using CurrencyConverterApi.Services.Interfaces;
using CurrencyConverterApi.Shared;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace UnitTests.Services
{
    public class ConversionsServiceTests
    {
        private readonly Mock<ICurrencyExchangeApiService> _mockCurrencyExchangeApiService;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly ConversionsService _conversionsService;

        public ConversionsServiceTests()
        {
            _mockCurrencyExchangeApiService = new Mock<ICurrencyExchangeApiService>();
            _mockCacheService = new Mock<ICacheService>();
            _conversionsService = new ConversionsService(_mockCurrencyExchangeApiService.Object, _mockCacheService.Object);
        }

        [Fact]
        public async Task Convert_ValidParameters_ReturnsConversionDTO()
        {
            // Arrange
            var baseCurrency = "USD";
            var targetCurrency = "EUR";
            var amount = 100m;
            var exchangeRate = 0.85m;
            var keyName = $"{baseCurrency}{targetCurrency}";

            _mockCacheService.Setup(c => c.GetCacheValue(keyName)).ReturnsAsync((string)null);
            _mockCurrencyExchangeApiService.Setup(s => s.GetExchangeRate(baseCurrency, targetCurrency)).ReturnsAsync(exchangeRate);

            // Act
            var result = await _conversionsService.Convert(baseCurrency, targetCurrency, amount);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(targetCurrency.ToUpper(), result.Currency);
            Assert.Equal(amount * exchangeRate, result.Amount);
            _mockCacheService.Verify(c => c.SetCacheValue(keyName, exchangeRate.ToString(), TimeSpan.FromMinutes(1)), Times.Once);
        }

        [Fact]
        public async Task Convert_CacheHit_ReturnsConversionDTO()
        {
            // Arrange
            var baseCurrency = "USD";
            var targetCurrency = "EUR";
            var amount = 100m;
            var exchangeRate = 0.85m;
            var keyName = $"{baseCurrency}{targetCurrency}";

            _mockCacheService.Setup(c => c.GetCacheValue(keyName)).ReturnsAsync(exchangeRate.ToString());

            // Act
            var result = await _conversionsService.Convert(baseCurrency, targetCurrency, amount);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(targetCurrency.ToUpper(), result.Currency);
            Assert.Equal(amount * exchangeRate, result.Amount);
            _mockCurrencyExchangeApiService.Verify(s => s.GetExchangeRate(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [InlineData("", "EUR", 100)]
        [InlineData("USD", "", 100)]
        [InlineData("USD", "EUR", 0)]
        public async Task Convert_InvalidParameters_ThrowsAppException(string baseCurrency, string targetCurrency, decimal amount)
        {
            // Act & Assert
            await Assert.ThrowsAsync<AppException>(() => _conversionsService.Convert(baseCurrency, targetCurrency, amount));
        }
    }
}
