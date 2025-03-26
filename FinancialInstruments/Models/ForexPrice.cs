using System.Text.Json.Serialization;

namespace FinancialInstrumentsApi.Models
{
    public class ForexPrice
    {
        [JsonPropertyName("ticker")]
        public string? Ticker { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime? Timestamp { get; set; }

        [JsonPropertyName("bidPrice")]
        public decimal? BidPrice { get; set; }

        [JsonPropertyName("askPrice")]
        public decimal? AskPrice { get; set; }

        [JsonPropertyName("midPrice")]
        public decimal? MidPrice { get; set; }
    }
}
