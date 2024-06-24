using CurrencyConverterApi.Controllers;
using CurrencyConverterApi.DTOs;
using CurrencyConverterApi.Services.Interfaces;
using CurrencyConverterApi.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.Controllers
{
    public class ConversionsControllerTests
    {
        private readonly Mock<IConversionsService> _mockConversionsService;
        private readonly ConversionsController _conversionsController;

        public ConversionsControllerTests()
        {
            _mockConversionsService = new Mock<IConversionsService>();
            _conversionsController = new ConversionsController(_mockConversionsService.Object);
        }

        [Fact]
        public async Task Convert_ValidParameters_ReturnsOkResult()
        {
            // Arrange
            var baseCurrency = "USD";
            var targetCurrency = "EUR";
            var amount = 100m;
            var conversionResult = new ConversionDTO
            {
                Currency = targetCurrency,
                Amount = 85m,
                ElapsedMilliSeconds = 100
            };

            _mockConversionsService.Setup(service => service.Convert(baseCurrency, targetCurrency, amount))
                .ReturnsAsync(conversionResult);

            // Act
            var result = await _conversionsController.Convert(baseCurrency, targetCurrency, amount);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ConversionDTO>(okResult.Value);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.Equal(conversionResult.Currency, returnValue.Currency);
            Assert.Equal(conversionResult.Amount, returnValue.Amount);
            Assert.Equal(conversionResult.ElapsedMilliSeconds, returnValue.ElapsedMilliSeconds);
        }

        [Fact]
        public async Task Convert_InvalidParameters_ReturnsBadRequest()
        {
            // Arrange
            _mockConversionsService.Setup(service => service.Convert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>()))
                .ThrowsAsync(new AppException("Base currency cannot be null or empty"));

            // Act
            var result = await _conversionsController.Convert("", "", 0);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }
    }
}