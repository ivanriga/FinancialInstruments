using FinancialInstrumentsApi.Services;
using FinancialInstrumentsApi.Services.FinancialInstrumentsApi.Services;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Register services
builder.Services.AddHttpClient();

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IPriceCache, PriceCache>();
builder.Services.AddSingleton<ITiingoService, TiingoService>();
//builder.Services.Decorate<IMarketDataProvider, CachedMarketDataProvider>();

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("MarketDataPolicy", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1)
            }));
});


var app = builder.Build();
// Register in Program.cs
app.UseMiddleware<ExceptionMiddleware>();
// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
// Start WebSocket connection
var tiingoService = app.Services.GetRequiredService<ITiingoService>();
var cancellationTokenSource = new CancellationTokenSource();



try
{
    await tiingoService.ConnectAsync(cancellationTokenSource.Token);

    // Subscribe to some default pairs
    await tiingoService.SubscribeToPair("eurusd");
    await tiingoService.SubscribeToPair("usdjpy");
    await tiingoService.SubscribeToPair("btcusd");
    await tiingoService.SubscribeToPair("audusd");
}
catch (Exception ex)
{
    Console.WriteLine($"Failed to connect to Tiingo: {ex.Message}");
}


app.Run();
