using CurrencyConverterApi.DTOs;

namespace CurrencyConverterApi.Services.Interfaces
{
    public interface ICurrencyExchangeApiService
    {
        Task<decimal> GetExchangeRate(string baseCurrency, string targetCurrency);
    }
}
