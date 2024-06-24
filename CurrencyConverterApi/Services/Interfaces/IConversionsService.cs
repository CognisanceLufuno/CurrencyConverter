using CurrencyConverterApi.DTOs;

namespace CurrencyConverterApi.Services.Interfaces
{
    public interface IConversionsService
    {
        Task<ConversionDTO> Convert(string @base, string target, decimal amount);
    }
}
