using CurrencyConverterApi.Services.Interfaces;
using Newtonsoft.Json.Linq;
using CurrencyConverterApi.Shared;

namespace CurrencyConverterApi.Services
{
    public class CurrencyExchangeApiService(HttpClient client, IConfiguration configuration) : ICurrencyExchangeApiService
    {
        private readonly HttpClient _client = client;
        private readonly IConfiguration _configuration = configuration;

        public async Task<decimal> GetExchangeRate(string baseCurrency, string targetCurrency)
        {
            // Construct the URL with the appropriate parameters
            string accessKey = _configuration.GetSection("CurrencyStackAPIKey").Value;
            string url = $"http://apilayer.net/api/live?access_key={accessKey}&currencies={targetCurrency}&source={baseCurrency}&format=1";

            // Make the HTTP request
            HttpResponseMessage response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            // Read and parse the response content
            string responseBody = await response.Content.ReadAsStringAsync();
            var jsonResponse = JObject.Parse(responseBody);

            // Check if the request was successful
            if (jsonResponse["success"] != null && (bool)jsonResponse["success"] == false)
            {
                throw new AppException("Error retrieving data from API: " + jsonResponse["error"]["info"]);
            }

            // Extract the exchange rate
            string rateKey = $"{baseCurrency}{targetCurrency}";

            var exchangeRate = (decimal)jsonResponse["quotes"][rateKey.ToUpper()];
            return exchangeRate;
        }
    }
}
