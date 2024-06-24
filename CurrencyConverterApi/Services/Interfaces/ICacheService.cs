using CurrencyConverterApi.DTOs;

namespace CurrencyConverterApi.Services.Interfaces
{
    public interface ICacheService
    {
        public Task SetCacheValue(string key, object value, TimeSpan expiration);
        public Task<string> GetCacheValue(string key);
    }
}
