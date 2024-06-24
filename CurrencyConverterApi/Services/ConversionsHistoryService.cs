using AutoMapper;
using CurrencyConverterApi.DTOs;
using CurrencyConverterApi.Repository.Interfaces;
using CurrencyConverterApi.Services.Interfaces;
using CurrencyConverterApi.Models;
using CurrencyConverterApi.Shared;
using System.Linq.Expressions;

namespace CurrencyConverterApi.Services
{
    public class ConversionsHistoryService(IConversionsHistoryRepository conversionHistoryRepository, IMapper mapper) : IConversionsHistoryService
    {
        private readonly IConversionsHistoryRepository _conversionHistoryRepository = conversionHistoryRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<PagedResult<ConversionsHistoryDTO>> GetFiltered(string? rateKey, string? dateFrom, string? dateTo, int pageNumber, int pageSize = 20)
        {

                var fromDate = string.IsNullOrEmpty(dateFrom) ? DateTime.Now.AddMonths(-1) : DateTime.Parse(dateFrom);
                var toDate = string.IsNullOrEmpty(dateTo) ? DateTime.Now : DateTime.Parse(dateTo);
                pageNumber = pageNumber < 1 ? 1 : pageNumber;

                if (fromDate > toDate)
                {
                    throw new AppException("Invalid date range. The start date cannot be after the end date.");
                }

                Expression<Func<ConversionsHistory, bool>> expression = x =>
                    (string.IsNullOrEmpty(rateKey) || x.RateKey.ToLower() == rateKey.ToLower()) &&
                    (string.IsNullOrEmpty(dateFrom) || x.CreateAt >= fromDate) &&
                    (string.IsNullOrEmpty(dateTo) || x.CreateAt <= toDate);

                var response = await _conversionHistoryRepository.SearchBy(expression);
                /*if (!response.Any())
                {
                    throw new AppException("No results found.");
                }*/

                var totalCount = response.Count();
                var totalPages = (totalCount > 0 && pageSize > 0)
                    ? (int)Math.Ceiling((double)totalCount / pageSize)
                    : 0;

                var pagedConversionsHistory = response
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var ConversionsHistoryDtoList = _mapper.Map<List<ConversionsHistoryDTO>>(pagedConversionsHistory);

                return new PagedResult<ConversionsHistoryDTO>
                {
                    Data = ConversionsHistoryDtoList,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages
                };
        }

        public async Task<ConversionsHistoryDTO> Save(ConversionsHistoryDTO conversion)
        {
            var conversionHistory = _mapper.Map<ConversionsHistory>(conversion);
            var result = await _conversionHistoryRepository.AddAsync(conversionHistory);

            return (result is null)
                ? throw new AppException($"An error occured while saving Conversion History: {conversion.RateKey} - {conversion.ExchangeRate}.")
                : _mapper.Map<ConversionsHistoryDTO>(result); ;
        }
    }
}
