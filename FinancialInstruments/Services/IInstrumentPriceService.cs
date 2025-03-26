using FinancialInstrumentsApi.Models;

namespace FinancialInstrumentsApi.Services
{
    public interface IInstrumentPriceService
    {
        IEnumerable<FinancialInstrument> GetAvailableInstruments();
        PriceResponse GetCurrentPrice(string symbol);
    }
}
