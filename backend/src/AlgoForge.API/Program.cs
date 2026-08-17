using AlgoForge.API.Middleware;
using AlgoForge.Application;
using AlgoForge.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Katmanları kaydet
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Difficulty gibi enum'lar JSON'da "Easy"/"Medium"/"Hard" string olarak gorunsun,
        // 0/1/2 gibi sayilarla ugrasmaya gerek kalmasin.
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!))
    };
});

builder.Services.AddAuthorization();

// Rate limiting - artik gercek internette yayinda oldugumuz icin kotuye kullanimi
// (brute-force login denemeleri, spam kayit, ucretsiz Judge0/Gemini kotasini tuketme) onlemek gerekiyor.
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Auth endpoint'leri (register/login) - IP basina dakikada 5 istek.
    // Brute-force sifre denemelerini ve spam hesap olusturmayi zorlastirir.
    options.AddFixedWindowLimiter("auth", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
    });

    // Judge0/Gemini cagiran endpoint'ler (kod calistirma, submission, AI assistant) -
    // kullanici basina dakikada 15 istek. Bu servisler ucretsiz katmanlarda paylasimli
    // kota kullaniyor, tek bir kullanicinin hepsini tuketmesini engelliyoruz.
    options.AddFixedWindowLimiter("expensive", limiterOptions =>
    {
        limiterOptions.PermitLimit = 15;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
    });

    // Genel API kullanimi icin daha gevsek bir global limit - IP basina dakikada 100 istek.
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(ipAddress, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 100,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
        });
    });
});

// Electron masaüstü istemcisi için CORS. Bearer token (cookie degil) kullandigimiz
// icin CORS'un asil korudugu CSRF riski burada gecerli degil - bu yuzden AllowAnyOrigin
// guvenli. Bu ayrica Electron'un paketlenmis (file://) surumunun bazen "null" origin
// gondermesi gibi durumlari da otomatik kapsar, tek tek origin eklemek gerekmez.
builder.Services.AddCors(options =>
{
    options.AddPolicy("DesktopClient", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// En basta olmali ki asagidaki tum middleware'lerden/controller'lardan gelen
// exception'lari yakalayabilsin.
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("DesktopClient");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
public partial class Program
{
}
