namespace FinancialInstrumentsApi.Models
{
    public class MarketData
    {
        public string Symbol { get; set; }
        public decimal Bid { get; set; }
        public decimal Ask { get; set; }
        public decimal LastPrice { get; set; }
        public decimal Volume { get; set; }
        public DateTime Timestamp { get; set; }
    }
}