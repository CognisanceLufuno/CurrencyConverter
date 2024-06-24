using CurrencyConverterApi.DTOs;
using CurrencyConverterApi.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;

namespace CurrencyConverterApi.Services
{
    public class CacheService(IMemoryCache cache, ILogger<CacheService> logger, IConversionsHistoryService conversionsHistoryService, IConnectionMultiplexer muxer) : ICacheService
    {
        private readonly IMemoryCache _cache = cache;
        private readonly ILogger<CacheService> _logger = logger;
        private readonly IConversionsHistoryService _conversionsHistoryService = conversionsHistoryService;
        private readonly IDatabase _redis = muxer.GetDatabase();

        public async Task SetCacheValue(string key, object value, TimeSpan expiration)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };

            cacheEntryOptions.RegisterPostEvictionCallback(async (evictedKey, evictedValue, reason, state) =>
            {
                _logger.LogInformation(message: $"Cache item with key '{evictedKey}' was evicted due to {reason}");
                await SaveExpiredCacheValue(evictedKey.ToString(), decimal.Parse(evictedValue.ToString()));
            });

            _cache.Set(key, value, cacheEntryOptions);
        }

        public async Task<string> GetCacheValue(string key)
        {
            _cache.TryGetValue(key, out string value);
            return value;
        }

        private async Task<ConversionsHistoryDTO> SaveExpiredCacheValue(string key, decimal value)
        {
            _logger.LogInformation($"Saving expired cache value: Key = {key}, Value = {value}");
            return await _conversionsHistoryService.Save(new ConversionsHistoryDTO { RateKey = key, ExchangeRate = value });
        }
    }
}
