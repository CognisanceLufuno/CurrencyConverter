using CurrencyConverterApi.Controllers;
using CurrencyConverterApi.DTOs;
using CurrencyConverterApi.Services.Interfaces;
using CurrencyConverterApi.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Controllers
{
    public class ConversionsHistoryControllerTests
    {
        private readonly Mock<IConversionsHistoryService> _mockConversionsHistoryService;
        private readonly ConversionsHistoryController _conversionsHistoryController;

        public ConversionsHistoryControllerTests()
        {
            _mockConversionsHistoryService = new Mock<IConversionsHistoryService>();
            _conversionsHistoryController = new ConversionsHistoryController(_mockConversionsHistoryService.Object);
        }

        [Fact]
        public async Task GetFiltered_ValidParameters_ReturnsOkResult()
        {
            // Arrange
            var rateKey = "USDGBP";
            var dateFrom = "2024-01-01";
            var dateTo = "2024-01-31";
            var pageNumber = 1;
            var pageSize = 20;
            var pagedResult = new PagedResult<ConversionsHistoryDTO>
            {
                Data = new List<ConversionsHistoryDTO>
                {
                    new ConversionsHistoryDTO { RateKey = rateKey, ExchangeRate = 0.74m }
                },
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = 1,
                TotalPages = 1
            };

            _mockConversionsHistoryService.Setup(service => service.GetFiltered(rateKey, dateFrom, dateTo, pageNumber, pageSize))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _conversionsHistoryController.GetFiltered(rateKey, dateFrom, dateTo, pageNumber, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PagedResult<ConversionsHistoryDTO>>(okResult.Value);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.Equal(pagedResult.Data.Count(), returnValue.Data.Count());
            Assert.Equal(pagedResult.PageNumber, returnValue.PageNumber);
            Assert.Equal(pagedResult.PageSize, returnValue.PageSize);
            Assert.Equal(pagedResult.TotalCount, returnValue.TotalCount);
            Assert.Equal(pagedResult.TotalPages, returnValue.TotalPages);
        }

        [Fact]
        public async Task GetFiltered_InvalidDateRange_ReturnsBadRequest()
        {
            // Arrange
            _mockConversionsHistoryService.Setup(service => service.GetFiltered(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new AppException("Invalid date range. The start date cannot be after the end date."));

            // Act
            var result = await _conversionsHistoryController.GetFiltered("USDGBP", "2024-01-31", "2024-01-01", 1, 20);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }
    }
}