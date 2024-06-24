using AutoMapper;
using CurrencyConverterApi.DTOs;
using CurrencyConverterApi.Models;
using CurrencyConverterApi.Repository.Interfaces;
using CurrencyConverterApi.Services;
using CurrencyConverterApi.Shared;
using Moq;
using System.Linq.Expressions;

namespace UnitTests.Services
{
    public class ConversionsHistoryServiceTests
    {
        private readonly Mock<IConversionsHistoryRepository> _mockConversionsHistoryRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ConversionsHistoryService _conversionsHistoryService;

        public ConversionsHistoryServiceTests()
        {
            _mockConversionsHistoryRepository = new Mock<IConversionsHistoryRepository>();
            _mockMapper = new Mock<IMapper>();
            _conversionsHistoryService = new ConversionsHistoryService(_mockConversionsHistoryRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetFiltered_ValidParameters_ReturnsPagedResult()
        {
            // Arrange
            string? rateKey = "USD";
            string? dateFrom = null;
            string? dateTo = null;
            int pageNumber = 1;
            int pageSize = 20;

            var conversionsHistoryList = new List<ConversionsHistory>
            {
                new ConversionsHistory { Id = 1, RateKey = "USD", CreateAt = DateTime.Now, ExchangeRate = 1.0m }
            };

            var conversionsHistoryDtoList = new List<ConversionsHistoryDTO>
            {
                new ConversionsHistoryDTO { Id = 1, RateKey = "USD", CreateAt = DateTime.Now, ExchangeRate = 1.0m }
            };

            _mockConversionsHistoryRepository.Setup(repo => repo.SearchBy(It.IsAny<Expression<Func<ConversionsHistory, bool>>>()))
                .ReturnsAsync(conversionsHistoryList.AsQueryable());

            _mockMapper.Setup(mapper => mapper.Map<List<ConversionsHistoryDTO>>(It.IsAny<List<ConversionsHistory>>()))
                .Returns(conversionsHistoryDtoList);

            // Act
            var result = await _conversionsHistoryService.GetFiltered(rateKey, dateFrom, dateTo, pageNumber, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(pageNumber, result.PageNumber);
            Assert.Equal(pageSize, result.PageSize);
            Assert.Equal(conversionsHistoryDtoList.Count, result.TotalCount);
        }

        [Fact]
        public async Task GetFiltered_InvalidDateRange_ThrowsAppException()
        {
            // Arrange
            string? rateKey = null;
            string? dateFrom = "2023-01-01";
            string? dateTo = "2022-01-01";
            int pageNumber = 1;
            int pageSize = 20;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _conversionsHistoryService.GetFiltered(rateKey, dateFrom, dateTo, pageNumber, pageSize));
            Assert.Equal("Invalid date range. The start date cannot be after the end date.", exception.Message);
        }

        [Fact]
        public async Task Save_ValidConversion_ReturnsSavedConversionDTO()
        {
            // Arrange
            var conversionDto = new ConversionsHistoryDTO { RateKey = "USD", ExchangeRate = 1.0m, CreateAt = DateTime.Now };
            var conversionHistory = new ConversionsHistory { Id = 1, RateKey = "USD", ExchangeRate = 1.0m, CreateAt = DateTime.Now };

            _mockMapper.Setup(mapper => mapper.Map<ConversionsHistory>(conversionDto)).Returns(conversionHistory);
            _mockConversionsHistoryRepository.Setup(repo => repo.AddAsync(conversionHistory)).ReturnsAsync(conversionHistory);
            _mockMapper.Setup(mapper => mapper.Map<ConversionsHistoryDTO>(conversionHistory)).Returns(conversionDto);

            // Act
            var result = await _conversionsHistoryService.Save(conversionDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(conversionDto.RateKey, result.RateKey);
            Assert.Equal(conversionDto.ExchangeRate, result.ExchangeRate);
        }

        [Fact]
        public async Task Save_InvalidConversion_ThrowsAppException()
        {
            // Arrange
            var conversionDto = new ConversionsHistoryDTO { RateKey = "USD", ExchangeRate = 1.0m, CreateAt = DateTime.Now };
            var conversionHistory = new ConversionsHistory { Id = 1, RateKey = "USD", ExchangeRate = 1.0m, CreateAt = DateTime.Now };

            _mockMapper.Setup(mapper => mapper.Map<ConversionsHistory>(conversionDto)).Returns(conversionHistory);
            _mockConversionsHistoryRepository.Setup(repo => repo.AddAsync(conversionHistory)).ReturnsAsync((ConversionsHistory)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AppException>(() => _conversionsHistoryService.Save(conversionDto));
            Assert.Equal($"An error occured while saving Conversion History: {conversionDto.RateKey} - {conversionDto.ExchangeRate}.", exception.Message);
        }
    }
}