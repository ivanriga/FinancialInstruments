using FinancialInstrumentsApi.Models;
using FinancialInstrumentsApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinancialInstrumentsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstrumentsController : ControllerBase
    {
        private readonly IInstrumentPriceService _priceService;

        public InstrumentsController(IInstrumentPriceService priceService)
        {
            _priceService = priceService;
        }

        /// <summary>
        /// Gets a list of all available financial instruments
        /// </summary>
        /// <returns>List of financial instruments</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FinancialInstrument>), 200)]
        public IActionResult GetAvailableInstruments()
        {
            return Ok(_priceService.GetAvailableInstruments());
        }

        /// <summary>
        /// Gets the current price for a specific financial instrument
        /// </summary>
        /// <param name="symbol">Instrument symbol (e.g., EURUSD, USDJPY, BTCUSD)</param>
        /// <returns>Current price with timestamp</returns>
        [HttpGet("{symbol}/price")]
        [ProducesResponseType(typeof(PriceResponse), 200)]
        [ProducesResponseType(typeof(ApiError), 404)]
        public IActionResult GetCurrentPrice(string symbol)
        {
            try
            {
                var price = _priceService.GetCurrentPrice(symbol);
                return Ok(price);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiError
                {
                    Message = ex.Message,
                    StatusCode = StatusCodes.Status404NotFound
                });
            }
        }
    }
}