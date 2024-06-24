using CurrencyConverterApi.DTOs;
using CurrencyConverterApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverterApi.Controllers
{
    public class ConversionsController(IConversionsService conversionsService) : Controller
    {
        private readonly IConversionsService _conversionsService = conversionsService;

        /// <summary>
        /// Converts an amount from the base currency to the target currency
        /// </summary>
        /// <param name="base">This is the amount that is to be converted from one currency to another based on the current Exchange Rate.</param>
        /// <param name="target">This parameter is the ISO representation of the currency that the "amount" is currently in. Examples of these are: ZAR - South African Rand; USD - American Dollar; GBP - Great Britain Pount; etc.</param>
        /// <param name="amount">This parameter is the ISO representation of the currency that the "amount" is supposed to be converted into based on the exchange rate.</param>
        /// <returns>Returns a conversion object</returns>
        [HttpGet("convert")]
        [ProducesResponseType(type: typeof(ConversionDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> Convert([FromQuery] string @base, [FromQuery] string target, [FromQuery] decimal amount)
        {
            var result = await _conversionsService.Convert(@base, target, amount);
            return Ok(result);
        }
    }
}
