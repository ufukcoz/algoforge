using AlgoForge.Application.Common.Interfaces;
using AlgoForge.Infrastructure.Persistence;
using AlgoForge.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlgoForge.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(NormalizeConnectionString(configuration.GetConnectionString("DefaultConnection"))));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        // Judge0 icin HttpClient. Varsayilan olarak ce.judge0.com (resmi, ucretsiz,
        // kimlik dogrulama gerektirmeyen genel deneme sunucusu) kullanilir.
        // RapidApi:ApiKey doldurulursa RapidAPI uzerinden barindirilan surume gecilir.
        services.AddHttpClient<IJudgeService, Judge0RapidApiService>(client =>
        {
            var judge0Settings = configuration.GetSection("Judge0");
            var baseUrl = judge0Settings["BaseUrl"] ?? "https://ce.judge0.com/";
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);

            var rapidApiKey = judge0Settings["RapidApiKey"];
            if (!string.IsNullOrWhiteSpace(rapidApiKey))
            {
                client.DefaultRequestHeaders.Add("X-RapidAPI-Key", rapidApiKey);
                client.DefaultRequestHeaders.Add("X-RapidAPI-Host", judge0Settings["RapidApiHost"] ?? "judge0-ce.p.rapidapi.com");
            }
        });

        // Gemini API icin HttpClient - Google AI Studio'nun ucretsiz katmani, kredi karti gerektirmez.
        services.AddHttpClient<IAiAssistantService, GeminiAiAssistantService>(client =>
        {
            client.BaseAddress = new Uri("https://generativelanguage.googleapis.com/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }

    // Render, Heroku ve bazi diger platformlar PostgreSQL baglanti bilgisini
    // "postgres://user:pass@host:port/dbname" seklinde (URI formati) veriyor,
    // ama Npgsql "Host=...;Username=...;Password=...;Database=..." formatini bekliyor.
    // Bu metod, hangisi verilirse verilsin dogru formata cevirir - boylece local'de
    // (Host=... formati) ve Render'da (postgres:// formati) ayni kod calisir.
    private static string? NormalizeConnectionString(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            return connectionString;

        if (!connectionString.StartsWith("postgres://") && !connectionString.StartsWith("postgresql://"))
            return connectionString; // zaten Npgsql formatinda, dokunma

        var uri = new Uri(connectionString);
        var userInfo = uri.UserInfo.Split(':', 2);

        return $"Host={uri.Host};Port={(uri.Port > 0 ? uri.Port : 5432)};" +
               $"Database={uri.AbsolutePath.TrimStart('/')};" +
               $"Username={userInfo[0]};Password={userInfo[1]};" +
               "SSL Mode=Require;Trust Server Certificate=true";
    }
}
