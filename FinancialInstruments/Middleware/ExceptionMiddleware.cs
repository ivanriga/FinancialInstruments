using FinancialInstrumentsApi.Models;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        //catch (Exception ex) //MarketDataException
        //{
        //    _logger.LogError(ex, "Market data error");
        //    context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        //    await context.Response.WriteAsJsonAsync(new ApiError
        //    {
        //        Message = "Market data service unavailable",
        //        StatusCode = StatusCodes.Status503ServiceUnavailable
        //    });
        //}
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new ApiError
            {
                Message = "Internal server error",
                StatusCode = StatusCodes.Status500InternalServerError
            });
        }
    }
}

