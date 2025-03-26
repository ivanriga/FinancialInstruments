using System.Text.Json.Serialization;

namespace FinancialInstrumentsApi.Models
{
    public class TiingoMessage
    {
        [JsonPropertyName("service")]
        public string Service { get; set; }

        [JsonPropertyName("messageType")]
        public string MessageType { get; set; }

        [JsonPropertyName("data")]
        public List<ForexPrice> Data { get; set; }
    }
}
