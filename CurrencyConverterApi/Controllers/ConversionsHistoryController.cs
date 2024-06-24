using CurrencyConverterApi.DTOs;
using CurrencyConverterApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverterApi.Controllers
{
    public class ConversionsHistoryController(IConversionsHistoryService conversionsHistoryService) : Controller
    {
        private readonly IConversionsHistoryService _conversionsHistoryService = conversionsHistoryService;

        /// <summary>
        /// Gets Conversion History using filters
        /// </summary>
        /// <param name="rateKey">The string that represents a Base Currency - Target Currency pair. E.g, ZARUSD, "USDGBP"</param>
        /// <param name="dateFrom">The earliest date the conversion could have occured. Human readable dates such as "1 Jan 2024", "1 January 2024" and system dates such as "2024-03-12 20:39:06.3133333" and "2024-03-12" are valid inputs. If dateFrom is not provided as part of the date range, it will return entries recorded on or after exactly a month before the day the call is made (e.g if the call is made on 1 Feb 2024, dateFrom will be defaulted to 1 Jan 2024).</param>
        /// <param name="dateTo">The latest date that the conversion could have occured. Human readable dates such as "1 Jan 2024", "1 January 2024" and system dates such as "2024-03-12 20:39:06.3133333" and "2024-03-12" are valid inputs. If dateTo is not provided as part of the date range, it will default dateTo to the date the call is made (e.g if the call is made on 2 Feb 2024, dateTo will default to 2 Feb 2024.</param>
        /// <param name="pageNumber">The number of the page to be returned in a paginated UI. If pageNumber is not provided, the first page of the results is returned.</param>
        /// <param name="pageSize">Defines the number of results to return per page in a paginated UI. If pageSize is not provided, the default page size is 20 records per page.</param>
        /// <returns>Returns a list of previous Conversions</returns>
        [HttpGet("getFiltered")]
        [ProducesResponseType(type: typeof(List<ConversionDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFiltered([FromQuery] string? rateKey, [FromQuery] string? dateFrom, [FromQuery] string? dateTo, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            var result = await _conversionsHistoryService.GetFiltered(rateKey, dateFrom, dateTo, pageNumber, pageSize);
            return Ok(result);
        }
    }
}
