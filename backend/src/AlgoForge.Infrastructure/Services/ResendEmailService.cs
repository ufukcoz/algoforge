using AlgoForge.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace AlgoForge.Infrastructure.Services;

// Resend'in ucretsiz katmanini kullanir - kredi karti gerektirmez.
// NOT: Domain dogrulanmadan (bkz appsettings.json Resend:FromEmail) sadece Resend
// hesabinin sahibinin kendi email adresine gonderim yapilabilir (sandbox kisitlamasi).
public class ResendEmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ResendEmailService> _logger;

    public ResendEmailService(HttpClient httpClient, IConfiguration configuration, ILogger<ResendEmailService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken)
    {
        var apiKey = _configuration["Resend:ApiKey"];
        var fromEmail = _configuration["Resend:FromEmail"] ?? "onboarding@resend.dev";

        if (string.IsNullOrWhiteSpace(apiKey) || apiKey == "PUT_YOUR_RESEND_API_KEY_HERE")
        {
            _logger.LogWarning("Resend API key yapilandirilmamis, email gonderilmedi: {ToEmail}", toEmail);
            return;
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var requestBody = new ResendEmailRequest
        {
            From = fromEmail,
            To = new[] { toEmail },
            Subject = subject,
            Html = htmlBody,
        };

        var response = await _httpClient.PostAsJsonAsync("emails", requestBody, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogWarning("Resend email gonderimi basarisiz: {StatusCode} {Body}", response.StatusCode, errorBody);
            // Sandbox kisitlamasinda (dogrulanmamis domain) baskasinin email'ine gonderim
            // 403 ile reddedilir - bu beklenen bir durum, exception firlatmiyoruz ki
            // register akisini bozmasin.
        }
    }

    private class ResendEmailRequest
    {
        [JsonPropertyName("from")]
        public string From { get; set; } = default!;

        [JsonPropertyName("to")]
        public string[] To { get; set; } = default!;

        [JsonPropertyName("subject")]
        public string Subject { get; set; } = default!;

        [JsonPropertyName("html")]
        public string Html { get; set; } = default!;
    }
}
