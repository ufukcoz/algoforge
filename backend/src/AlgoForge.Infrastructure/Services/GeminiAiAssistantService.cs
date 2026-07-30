using AlgoForge.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace AlgoForge.Infrastructure.Services;

// Google AI Studio'nun ucretsiz Gemini API'sini kullanir - kredi karti gerektirmez.
// Baska bir LLM saglayicisina gecmek istersek sadece bu sinifi degistirmemiz yeterli,
// Application katmani IAiAssistantService arayuzu uzerinden calisir, Gemini'ye ozgu
// hicbir sey bilmez.
public class GeminiAiAssistantService : IAiAssistantService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GeminiAiAssistantService> _logger;

    public GeminiAiAssistantService(HttpClient httpClient, IConfiguration configuration, ILogger<GeminiAiAssistantService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> GenerateAsync(string prompt, CancellationToken cancellationToken)
    {
        var apiKey = _configuration["Gemini:ApiKey"];
        var model = _configuration["Gemini:Model"] ?? "gemini-2.5-flash";

        if (string.IsNullOrWhiteSpace(apiKey) || apiKey == "PUT_YOUR_GEMINI_API_KEY_HERE")
        {
            return "AI Assistant henuz yapilandirilmamis. appsettings.json icindeki Gemini:ApiKey " +
                   "alanina Google AI Studio'dan aldigin ucretsiz API key'i eklemen gerekiyor.";
        }

        var requestBody = new GeminiRequest
        {
            Contents = new[]
            {
                new GeminiContent
                {
                    Parts = new[] { new GeminiPart { Text = prompt } },
                },
            },
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"v1beta/models/{model}:generateContent?key={apiKey}",
                requestBody,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Gemini API istegi basarisiz: {StatusCode} {Body}", response.StatusCode, errorBody);

                // Ucretsiz katmanin rate limit'i asildiginda 429 doner - kullaniciya anlamli mesaj verelim.
                if ((int)response.StatusCode == 429)
                    return "AI Assistant su an cok yogun (ucretsiz kullanim limiti asildi). Biraz sonra tekrar dene.";

                return "AI Assistant su an yanit veremiyor, birazdan tekrar dene.";
            }

            var result = await response.Content.ReadFromJsonAsync<GeminiResponse>(cancellationToken: cancellationToken);
            var text = result?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

            return string.IsNullOrWhiteSpace(text)
                ? "AI Assistant bos bir yanit dondurdu, tekrar dener misin?"
                : text.Trim();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Gemini API cagrisi sirasinda beklenmeyen hata");
            return "AI Assistant'a baglanilamadi, internet baglantini kontrol et.";
        }
    }

    private class GeminiRequest
    {
        [JsonPropertyName("contents")]
        public GeminiContent[] Contents { get; set; } = default!;
    }

    private class GeminiContent
    {
        [JsonPropertyName("parts")]
        public GeminiPart[] Parts { get; set; } = default!;
    }

    private class GeminiPart
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = default!;
    }

    private class GeminiResponse
    {
        [JsonPropertyName("candidates")]
        public List<GeminiCandidate>? Candidates { get; set; }
    }

    private class GeminiCandidate
    {
        [JsonPropertyName("content")]
        public GeminiContent? Content { get; set; }
    }
}
