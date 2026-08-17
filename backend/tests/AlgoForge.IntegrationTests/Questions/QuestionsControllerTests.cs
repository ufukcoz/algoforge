using AlgoForge.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace AlgoForge.IntegrationTests.Questions;

public class QuestionsControllerTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public QuestionsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetQuestions_ShouldReturnSuccessStatusCode()
    {
        var response = await _client.GetAsync("/api/questions");

        Assert.True(
            response.IsSuccessStatusCode,
            $"Expected success status code but got {(int)response.StatusCode}.");
    }

    [Fact]
    public async Task GetQuestions_ShouldReturnJson()
    {
        var response = await _client.GetAsync("/api/questions");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        Assert.False(string.IsNullOrWhiteSpace(content));

        using var document = JsonDocument.Parse(content);

        Assert.Equal(
            JsonValueKind.Object,
            document.RootElement.ValueKind);
    }

    [Fact]
    public async Task SecurityHeaders_ShouldBePresent()
    {
        var response = await _client.GetAsync("/api/questions");

        Assert.Equal(
            "nosniff",
            response.Headers.GetValues("X-Content-Type-Options").Single());

        Assert.Equal(
            "DENY",
            response.Headers.GetValues("X-Frame-Options").Single());

        Assert.Equal(
            "no-referrer",
            response.Headers.GetValues("Referrer-Policy").Single());

        Assert.Equal(
            "camera=(), microphone=(), geolocation=()",
            response.Headers.GetValues("Permissions-Policy").Single());
    }

    [Fact]
    public async Task Cors_ShouldAllowConfiguredOrigin()
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/api/questions");

        request.Headers.Add(
            "Origin",
            "https://example.com");

        var response = await _client.SendAsync(request);

        Assert.True(
            response.Headers.TryGetValues(
                "Access-Control-Allow-Origin",
                out var values));

        Assert.Equal(
            "*",
            values.Single());
    }

    [Fact]
    public async Task CreateQuestion_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        var request = new
        {
            Title = "Unauthorized Test",
            Difficulty = "Easy",
            Description = "Test question",
            TimeLimitMs = 1000,
            MemoryLimitMb = 128,
            CategoryId = Guid.NewGuid()
        };

        var response = await _client.PostAsJsonAsync(
            "/api/questions",
            request);

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateQuestion_ShouldReturnForbidden_WhenAuthenticatedUserIsNotAdmin()
    {
        using var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Add(
            "X-Test-User-Id",
            Guid.NewGuid().ToString());

        client.DefaultRequestHeaders.Add(
            "X-Test-Role",
            "User");

        client.DefaultRequestHeaders.Add(
            "X-Test-Username",
            "normal-user");

        var request = new
        {
            Title = "User Test",
            Difficulty = "Easy",
            Description = "Test question",
            TimeLimitMs = 1000,
            MemoryLimitMb = 128,
            CategoryId = Guid.NewGuid()
        };

        var response = await client.PostAsJsonAsync(
            "/api/questions",
            request);

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateQuestion_ShouldPassAuthorization_WhenUserIsAdmin()
    {
        using var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Add(
            "X-Test-User-Id",
            Guid.NewGuid().ToString());

        client.DefaultRequestHeaders.Add(
            "X-Test-Role",
            "Admin");

        client.DefaultRequestHeaders.Add(
            "X-Test-Username",
            "admin-user");

        var request = new
        {
            Title = "Admin Test",
            Difficulty = "Easy",
            Description = "Test question",
            TimeLimitMs = 1000,
            MemoryLimitMb = 128,
            CategoryId = Guid.NewGuid()
        };

        var response = await client.PostAsJsonAsync(
            "/api/questions",
            request);

        Assert.NotEqual(
            HttpStatusCode.Unauthorized,
            response.StatusCode);

        Assert.NotEqual(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }
}
