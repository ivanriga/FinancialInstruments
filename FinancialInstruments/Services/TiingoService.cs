using FinancialInstrumentsApi.Models;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text;
using NJson = Newtonsoft.Json;

namespace FinancialInstrumentsApi.Services
{
    public class TiingoService : ITiingoService, IDisposable
    {
        private readonly IConfiguration _config;
        private readonly IPriceCache _priceCache;
        private readonly ILogger<TiingoService> _logger;
        private ClientWebSocket _webSocket;

        public TiingoService(
            IConfiguration config,
            IPriceCache priceCache,
            ILogger<TiingoService> logger)
        {
            _config = config;
            _priceCache = priceCache;
            _logger = logger;
        }

        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            _webSocket = new ClientWebSocket();
            var uri = new Uri(_config["Tiingo:WebSocketUrl"]);

            await _webSocket.ConnectAsync(uri, cancellationToken);

            _ = Task.Run(() => ReceiveMessages(cancellationToken), cancellationToken);

            _logger.LogInformation("Connected to Tiingo WebSocket API");
        }

        public async Task SubscribeToPair(string pair)
        {
            if (_webSocket?.State != WebSocketState.Open)
            {
                throw new InvalidOperationException("WebSocket is not connected");
            }

            var subscriptionMessage = new
            {
                eventName = "subscribe",
                authorization = _config["Tiingo:ApiKey"],
                eventData = new
                {
                    thresholdLevel = 5,
                    tickers = new[] { pair }
                }
            };

            var json = JsonSerializer.Serialize(subscriptionMessage);
            var bytes = Encoding.UTF8.GetBytes(json);

            await _webSocket.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }

        private async Task ReceiveMessages(CancellationToken cancellationToken)
        {
            var buffer = new byte[4096];

            while (!cancellationToken.IsCancellationRequested &&
                   _webSocket?.State == WebSocketState.Open)
            {
                try
                {
                    var result = await _webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer),
                        cancellationToken);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        ProcessMessage(message);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _webSocket.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            string.Empty,
                            cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error receiving WebSocket message");
                    // Implement reconnection logic here
                }
            }
        }

        private void ProcessMessage(string message)
        {
            try
            {

                dynamic tiingoMessage = NJson.JsonConvert.DeserializeObject<dynamic>(message);

                //  var tiingoMessage = JsonSerializer.Deserialize<TiingoMessage>(message);

                if (tiingoMessage != null && tiingoMessage?.messageType == "A" && tiingoMessage?.data != null)
                {
                    //foreach (var price in tiingoMessage?.data)
                    //{
                    var price = tiingoMessage?.data;
                    ForexPrice forexPrice = new ForexPrice();
                    forexPrice.Ticker = price[1];
                    forexPrice.Timestamp = DateTime.Parse(price[2].ToString());
                    forexPrice.BidPrice = price[3];
                    forexPrice.AskPrice = price[4];

                    forexPrice.MidPrice = price[5];




                    _priceCache.UpdatePrice(forexPrice);
                    _logger.LogDebug("Updated price for {Ticker}: {Bid}/{Ask}",
                        forexPrice.Ticker, forexPrice.BidPrice, forexPrice.AskPrice);
                    //}
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing WebSocket message");
            }
        }

        public void Dispose()
        {
            _webSocket?.Dispose();
        }
    }
}
