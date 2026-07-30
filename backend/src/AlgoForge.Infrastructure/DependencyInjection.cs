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
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

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
}
