using AlgoForge.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace AlgoForge.Infrastructure.Services;

// Judge0 CE'nin RapidAPI uzerinden barindirilan versiyonunu kullanir.
// Kendi Docker instance'imiza gecmek istersek sadece bu sinifin BaseUrl/header
// ayarlarini degistirmemiz (ya da IJudgeService'in yeni bir implementasyonunu yazmamiz) yeterli,
// Application katmani hicbir sekilde etkilenmez.
public class Judge0RapidApiService : IJudgeService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<Judge0RapidApiService> _logger;

    // Judge0 CE dil ID'leri (RapidAPI /languages endpoint'inden dogrulanmali,
    // surumler arasi degisebilir). Su an en yaygin kullanilan ID'ler.
    private static readonly Dictionary<string, int> LanguageIds = new()
    {
        ["javascript"] = 63, // Node.js 12.14.0
        ["python"] = 71,     // Python 3.8.1
        ["cpp"] = 54,        // C++ (GCC 9.2.0)
        ["java"] = 62,       // Java (OpenJDK 13.0.1)
        ["csharp"] = 51,     // C# (Mono 6.6.0.161)
    };

    public Judge0RapidApiService(HttpClient httpClient, ILogger<Judge0RapidApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<JudgeExecutionResult> ExecuteAsync(
        string sourceCode,
        string language,
        string stdin,
        string expectedOutput,
        int timeLimitMs,
        int memoryLimitMb,
        CancellationToken cancellationToken)
    {
        if (!LanguageIds.TryGetValue(language.ToLowerInvariant(), out var languageId))
        {
            return new JudgeExecutionResult(
                false, null, null, null, null, null,
                $"Desteklenmeyen dil: {language}");
        }

        var requestBody = new Judge0SubmissionRequest
        {
            // Judge0'in Java calistiricisi dosyayi "Main.java" olarak kaydedip
            // "java Main" ile calistirir - yani public sinif adi mutlaka "Main" olmali.
            // Kullanicinin kendi sinif adini (orn. "Solution") kullanabilmesi icin
            // burada seffafca "Main"e ceviriyoruz; veritabanina kaydedilen orijinal kod degismez.
            SourceCode = language.ToLowerInvariant() == "java" ? NormalizeJavaClassName(sourceCode) : sourceCode,
            LanguageId = languageId,
            Stdin = stdin,
            ExpectedOutput = expectedOutput,
            // Judge0 saniye cinsinden bekliyor, bizim limitimiz ms.
            CpuTimeLimit = Math.Max(1, timeLimitMs / 1000.0),
            MemoryLimit = memoryLimitMb * 1024, // Judge0 KB bekliyor
        };

        try
        {
            // wait=true: Judge0'in kendi kuyruk sistemini beklemeden senkron sonuc almayi dener.
            // Ancak bazi Judge0 sunuculari bu ozelligi kapatabiliyor - o durumda sadece
            // bir "token" doner ve sonucu polling ile beklememiz gerekir. Asagidaki kod
            // her iki senaryoyu da destekler.
            var response = await _httpClient.PostAsJsonAsync(
                "submissions?base64_encoded=false&wait=true",
                requestBody,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Judge0 istegi basarisiz: {StatusCode} {Body}", response.StatusCode, errorBody);
                return new JudgeExecutionResult(
                    false, null, null, null, null, null,
                    $"Judge servisi hatasi ({response.StatusCode})");
            }

            var result = await response.Content.ReadFromJsonAsync<Judge0SubmissionResult>(cancellationToken: cancellationToken);

            if (result is null)
            {
                return new JudgeExecutionResult(false, null, null, null, null, null, "Judge servisinden bos yanit alindi.");
            }

            // wait=true desteklenmiyorsa yanitta sadece "token" olur, "status" null/eksik kalir.
            // Bu durumda sonucu polling ile bekliyoruz.
            if (result.Status is null && !string.IsNullOrEmpty(result.Token))
            {
                result = await PollForResultAsync(result.Token, cancellationToken);
                if (result is null)
                {
                    return new JudgeExecutionResult(false, null, null, null, null, null, "Judge sonucu zamaninda alinamadi (timeout).");
                }
            }

            var statusDescription = result.Status?.Description ?? "Bilinmeyen durum";
            var actualOutput = result.Stdout?.TrimEnd('\n', '\r');
            var expectedTrimmed = expectedOutput.TrimEnd('\n', '\r');

            // Judge0 zaten expected_output karsilastirmasi yapiyor (Accepted/Wrong Answer donuyor),
            // ama guvence icin kendi tarafimizda da string karsilastirmasi yapiyoruz.
            var passed = statusDescription.Equals("Accepted", StringComparison.OrdinalIgnoreCase)
                         && string.Equals(actualOutput?.Trim(), expectedTrimmed.Trim(), StringComparison.Ordinal);

            int? runtimeMs = result.Time is not null
                ? (int)(double.Parse(result.Time, System.Globalization.CultureInfo.InvariantCulture) * 1000)
                : null;

            int? memoryKb = result.Memory;

            return new JudgeExecutionResult(
                passed,
                actualOutput,
                result.Stderr,
                result.CompileOutput,
                runtimeMs,
                memoryKb,
                statusDescription
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Judge0 cagrisi sirasinda beklenmeyen hata");
            return new JudgeExecutionResult(false, null, ex.Message, null, null, null, "Judge servisine baglanilamadi");
        }
    }

    // wait=true desteklenmiyorsa bu metodla sonucu belirli araliklarla kontrol ederiz.
    // status.id 1 (In Queue) veya 2 (Processing) oldugu surece beklemeye devam ederiz.
    private async Task<Judge0SubmissionResult?> PollForResultAsync(string token, CancellationToken cancellationToken)
    {
        const int maxAttempts = 15;
        const int delayMs = 1000;

        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            await Task.Delay(delayMs, cancellationToken);

            var response = await _httpClient.GetAsync($"submissions/{token}?base64_encoded=false", cancellationToken);
            if (!response.IsSuccessStatusCode)
                continue;

            var result = await response.Content.ReadFromJsonAsync<Judge0SubmissionResult>(cancellationToken: cancellationToken);

            // status.id: 1 = In Queue, 2 = Processing - bunlar disinda her sey "bitmis" demektir.
            if (result?.Status is not null && result.Status.Id != 1 && result.Status.Id != 2)
                return result;
        }

        return null;
    }

    // Java kodundaki "public class X" (veya sadece "class X") tanimini bulup
    // X'i "Main" ile degistirir - hem tanimda hem de kod icindeki tum referanslarinda
    // (orn. "new X()" gibi kurucu cagrilari), boylece kullanici istedigi sinif adini
    // kullanabilir. Birden fazla ust-duzey sinif varsa sadece ilk (public olan varsa o) sinif
    // yeniden adlandirilir - basit tek-sinifli cozumler icin yeterlidir.
    private static string NormalizeJavaClassName(string sourceCode)
    {
        var match = Regex.Match(sourceCode, @"public\s+class\s+(\w+)");
        if (!match.Success)
        {
            match = Regex.Match(sourceCode, @"\bclass\s+(\w+)");
        }

        if (!match.Success)
            return sourceCode;

        var originalClassName = match.Groups[1].Value;
        if (originalClassName == "Main")
            return sourceCode;

        // \b ile tam kelime eslesmesi yapiyoruz ki "Solution" gibi bir isim baska
        // bir kelimenin (orn. "SolutionHelper") parcasi olarak yanlislikla degistirilmesin.
        return Regex.Replace(sourceCode, $@"\b{Regex.Escape(originalClassName)}\b", "Main");
    }

    private class Judge0SubmissionRequest
    {
        [JsonPropertyName("source_code")]
        public string SourceCode { get; set; } = default!;

        [JsonPropertyName("language_id")]
        public int LanguageId { get; set; }

        [JsonPropertyName("stdin")]
        public string Stdin { get; set; } = default!;

        [JsonPropertyName("expected_output")]
        public string ExpectedOutput { get; set; } = default!;

        [JsonPropertyName("cpu_time_limit")]
        public double CpuTimeLimit { get; set; }

        [JsonPropertyName("memory_limit")]
        public int MemoryLimit { get; set; }
    }

    private class Judge0SubmissionResult
    {
        [JsonPropertyName("token")]
        public string? Token { get; set; }

        [JsonPropertyName("stdout")]
        public string? Stdout { get; set; }

        [JsonPropertyName("stderr")]
        public string? Stderr { get; set; }

        [JsonPropertyName("compile_output")]
        public string? CompileOutput { get; set; }

        [JsonPropertyName("time")]
        public string? Time { get; set; }

        [JsonPropertyName("memory")]
        public int? Memory { get; set; }

        [JsonPropertyName("status")]
        public Judge0Status? Status { get; set; }
    }

    private class Judge0Status
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}
