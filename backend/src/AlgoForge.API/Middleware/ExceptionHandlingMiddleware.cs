using System.Text.Json;

namespace AlgoForge.API.Middleware;

// Su ana kadar tum hatalar (yanlis sifre, zaten kayitli email, bulunamayan kayit vb.)
// 500 Internal Server Error olarak donuyordu - bu hem yanlis (bunlar sunucu hatasi degil,
// beklenen is kurallari) hem de istemci tarafinda hata ayirt etmeyi zorlastiriyordu.
// Bu middleware, Application katmanindaki exception turlerini doğru HTTP status kodlarina esler.
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (UnauthorizedAccessException ex)
        {
            // Yanlis sifre, gecersiz/suresi dolmus token gibi kimlik dogrulama hatalari.
            await WriteErrorResponse(context, StatusCodes.Status401Unauthorized, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            // Is kurali ihlalleri: "email zaten kayitli", "kategori bulunamadi" gibi
            // beklenen, istemci hatasindan kaynaklanan durumlar.
            await WriteErrorResponse(context, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            // Beklenmeyen gercek sunucu hatalari - detayini disariya sizdirmiyoruz,
            // ama loglara tam exception'i yaziyoruz ki teshis edebilelim.
            _logger.LogError(ex, "Beklenmeyen bir hata olustu: {Path}", context.Request.Path);
            await WriteErrorResponse(context, StatusCodes.Status500InternalServerError, "Beklenmeyen bir hata olustu.");
        }
    }

    private static async Task WriteErrorResponse(HttpContext context, int statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var payload = JsonSerializer.Serialize(new { message });
        await context.Response.WriteAsync(payload);
    }
}
