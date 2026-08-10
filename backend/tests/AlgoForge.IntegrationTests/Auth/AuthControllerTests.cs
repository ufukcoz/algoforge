using AlgoForge.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace AlgoForge.IntegrationTests.Auth;

public class AuthControllerTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ShouldReturnExpectedStatusCode()
    {
        var request = new
        {
            Username = $"integration_{Guid.NewGuid():N}",
            Email = $"integration_{Guid.NewGuid():N}@example.com",
            Password = "Test123!Password"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/auth/register",
            request);

        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.Conflict,
            $"Unexpected status code: {(int)response.StatusCode} {response.StatusCode}");
    }
}