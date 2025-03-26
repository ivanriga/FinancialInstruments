using FinancialInstrumentsApi.Models;

namespace FinancialInstrumentsApi.Services
{
    public class InstrumentPriceService : IInstrumentPriceService
    {
        private static readonly Dictionary<string, FinancialInstrument> _instruments = new()
        {
            { "EURUSD", new FinancialInstrument { Symbol = "EURUSD", Name = "Euro/US Dollar" } },
            { "USDJPY", new FinancialInstrument { Symbol = "USDJPY", Name = "US Dollar/Japanese Yen" } },
            { "BTCUSD", new FinancialInstrument { Symbol = "BTCUSD", Name = "Bitcoin/US Dollar" } }
        };

        private static readonly Random _random = new();

        public IEnumerable<FinancialInstrument> GetAvailableInstruments()
        {
            return _instruments.Values;
        }

        public PriceResponse GetCurrentPrice(string symbol)
        {
            if (!_instruments.ContainsKey(symbol.ToUpper()))
            {
                throw new KeyNotFoundException($"Instrument {symbol} not found");
            }

            // Simulate price generation with random values
            // In real application, this would fetch from market data provider
            return new PriceResponse
            {
                Symbol = symbol.ToUpper(),
                Price = symbol.ToUpper() switch
                {
                    "EURUSD" => Math.Round(1.05m + (decimal)(_random.NextDouble() * 0.1 - 0.05), 4),
                    "USDJPY" => Math.Round(130.0m + (decimal)(_random.NextDouble() * 2 - 1), 2),
                    "BTCUSD" => Math.Round(30000m + (decimal)(_random.NextDouble() * 5000 - 2500), 2),
                    _ => 0m
                },
                Timestamp = DateTime.UtcNow
            };
        }
    }
}
