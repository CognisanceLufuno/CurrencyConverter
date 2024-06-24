using CurrencyConverterApi.Services;
using CurrencyConverterApi.Shared;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Services
{
    public class CurrencyExchangeApiServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly CurrencyExchangeApiService _currencyExchangeApiService;
        private readonly string baseCurrency = "USD";
        private readonly string targetCurrency = "EUR";
        private readonly string accessKey = "test_access_key";

        public CurrencyExchangeApiServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _currencyExchangeApiService = new CurrencyExchangeApiService(_httpClient, _mockConfiguration.Object);
        }

        [Fact]
        public async Task GetExchangeRate_ValidResponse_ReturnsExchangeRate()
        {
            // Arrange
           
            decimal expectedRate = 1.2m;

            _mockConfiguration.Setup(config => config.GetSection("CurrencyStackAPIKey").Value).Returns(accessKey);

            var responseContent = new JObject
            {
                { "success", true },
                { "quotes", new JObject { { $"{baseCurrency}{targetCurrency}", expectedRate } } }
            }.ToString();

            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    It.IsAny<HttpRequestMessage>(),
                    It.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponseMessage);

            // Act
            var actualRate = await _currencyExchangeApiService.GetExchangeRate(baseCurrency, targetCurrency);

            // Assert
            Assert.Equal(expectedRate, actualRate);
        }

        [Fact]
        public async Task GetExchangeRate_ApiReturnsError_ThrowsAppException()
        {
            // Arrange
            string errorMessage = "Invalid API key";

            _mockConfiguration.Setup(config => config.GetSection("CurrencyStackAPIKey").Value).Returns(accessKey);

            var responseContent = new JObject
            {
                { "success", false },
                { "error", new JObject { { "info", errorMessage } } }
            }.ToString();

            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            };

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    It.IsAny<HttpRequestMessage>(),
                    It.IsAny<CancellationToken>()
                )
                .ReturnsAsync(httpResponseMessage);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _currencyExchangeApiService.GetExchangeRate(baseCurrency, targetCurrency));
            Assert.Equal($"Error retrieving data from API: {errorMessage}", exception.Message);
        }

        [Fact]
        public async Task GetExchangeRate_HttpRequestFails_ThrowsHttpRequestException()
        {
            // Arrange

            _mockConfiguration.Setup(config => config.GetSection("CurrencyStackAPIKey").Value).Returns(accessKey);

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    It.IsAny<HttpRequestMessage>(),
                    It.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Request failed"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => _currencyExchangeApiService.GetExchangeRate(baseCurrency, targetCurrency));
            Assert.Equal("the error ", exception.Message);
        }
    }
}
