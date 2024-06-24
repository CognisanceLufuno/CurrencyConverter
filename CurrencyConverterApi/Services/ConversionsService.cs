using CurrencyConverterApi.DTOs;
using CurrencyConverterApi.Services.Interfaces;
using CurrencyConverterApi.Shared;
using Microsoft.Extensions.Caching.Memory;
using Org.BouncyCastle.Asn1.X509;
using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace CurrencyConverterApi.Services
{
    public class ConversionsService(ICurrencyExchangeApiService currencyExchangeApiService, ICacheService cache) : IConversionsService
    {
        private readonly ICurrencyExchangeApiService _currencyExchangeApiService = currencyExchangeApiService;
        private readonly ICacheService _cache = cache;

        public async Task<ConversionDTO> Convert(string baseCurrency, string targetCurrency, decimal amount)
        {
            var watch = Stopwatch.StartNew();
            ValidateParameters(baseCurrency, targetCurrency, amount);

            //Check the cache to see if there is an existing exchange rate that hasn't expired in the last 15 minutes
            var keyName = $"{baseCurrency}{targetCurrency}";
            //var exchangeRateJson = await _redis.StringGetAsync(keyName);
            var exchangeRate = await _cache.GetCacheValue(keyName);

            if (string.IsNullOrEmpty(exchangeRate))
            {
                //Get exchange rate from external API
                var exchangeRateAsync = await _currencyExchangeApiService.GetExchangeRate(baseCurrency, targetCurrency);
                exchangeRate = exchangeRateAsync.ToString();
                //Save exchange rate in the cache for the next 15 minutes
                await _cache.SetCacheValue(keyName, exchangeRate, TimeSpan.FromMinutes(1));
            }

            //Calculate amount to be returned in the target currency
            decimal convertedAmount = amount * decimal.Parse(exchangeRate.ToString());

            var result = new ConversionDTO { Currency = targetCurrency.ToUpper(), Amount = convertedAmount, ElapsedMilliSeconds = watch.ElapsedMilliseconds };

            watch.Stop();
            return result;
        }

        private static void ValidateParameters(string baseCurrency, string targetCurrency, decimal amount)
        {
            if (string.IsNullOrWhiteSpace(baseCurrency))
                throw new AppException("Base currency cannot be null or empty", nameof(baseCurrency));

            if (string.IsNullOrWhiteSpace(targetCurrency))
                throw new AppException("Target currency cannot be null or empty", nameof(targetCurrency));

            if (amount <= 0)
                throw new AppException("Amount must be greater than zero", nameof(amount));
        }
    }
}
