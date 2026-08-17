using AlgoForge.IntegrationTests.Infrastructure;
using System.Net;
using System.Text.Json;
using Xunit;

namespace AlgoForge.IntegrationTests.Health;

public class HealthControllerTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public HealthControllerTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_ShouldReturnOk()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task Health_ShouldReturnHealthyStatus()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var content =
            await response.Content.ReadAsStringAsync();

        using var document =
            JsonDocument.Parse(content);

        Assert.Equal(
            "healthy",
            document.RootElement
                .GetProperty("status")
                .GetString());
    }
}
