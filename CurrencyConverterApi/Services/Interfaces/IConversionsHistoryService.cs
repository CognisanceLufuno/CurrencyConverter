using CurrencyConverterApi.DTOs;

namespace CurrencyConverterApi.Services.Interfaces
{
    public interface IConversionsHistoryService
    {
        Task<PagedResult<ConversionsHistoryDTO>> GetFiltered(string? rateKey, string? dateFrom, string? dateTo, int pageNumber, int pageSize);
        Task<ConversionsHistoryDTO> Save(ConversionsHistoryDTO conversion);
    }
}
