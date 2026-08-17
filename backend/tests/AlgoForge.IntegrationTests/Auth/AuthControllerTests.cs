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
        var suffix = Guid.NewGuid().ToString("N");

        var request = new
        {
            Username = $"integration_{suffix}",
            Email = $"integration_{suffix}@example.com",
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

    [Fact]
    public async Task Refresh_ShouldRotateRefreshToken()
    {
        var suffix = Guid.NewGuid().ToString("N");

        var registerRequest = new
        {
            Username = $"refresh_{suffix}",
            Email = $"refresh_{suffix}@example.com",
            Password = "Test123!Password"
        };

        var registerResponse = await _client.PostAsJsonAsync(
            "/api/auth/register",
            registerRequest);

        Assert.True(
            registerResponse.StatusCode == HttpStatusCode.OK ||
            registerResponse.StatusCode == HttpStatusCode.BadRequest ||
            registerResponse.StatusCode == HttpStatusCode.Conflict);

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                Email = registerRequest.Email,
                Password = registerRequest.Password
            });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginResult =
            await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(loginResult);
        Assert.False(string.IsNullOrWhiteSpace(loginResult!.RefreshToken));

        var refreshResponse = await _client.PostAsJsonAsync(
            "/api/auth/refresh",
            new
            {
                RefreshToken = loginResult.RefreshToken
            });

        Assert.Equal(HttpStatusCode.OK, refreshResponse.StatusCode);

        var refreshResult =
            await refreshResponse.Content.ReadFromJsonAsync<RefreshResponse>();

        Assert.NotNull(refreshResult);
        Assert.False(string.IsNullOrWhiteSpace(refreshResult!.RefreshToken));
        Assert.NotEqual(loginResult.RefreshToken, refreshResult.RefreshToken);
    }

    [Fact]
    public async Task Refresh_ShouldRejectReusedRefreshToken()
    {
        var suffix = Guid.NewGuid().ToString("N");

        var registerRequest = new
        {
            Username = $"reuse_{suffix}",
            Email = $"reuse_{suffix}@example.com",
            Password = "Test123!Password"
        };

        var registerResponse = await _client.PostAsJsonAsync(
            "/api/auth/register",
            registerRequest);

        Assert.True(
            registerResponse.StatusCode == HttpStatusCode.OK ||
            registerResponse.StatusCode == HttpStatusCode.BadRequest ||
            registerResponse.StatusCode == HttpStatusCode.Conflict);

        var loginResponse = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                Email = registerRequest.Email,
                Password = registerRequest.Password
            });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var loginResult =
            await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(loginResult);
        Assert.False(string.IsNullOrWhiteSpace(loginResult!.RefreshToken));

        var firstRefreshResponse = await _client.PostAsJsonAsync(
            "/api/auth/refresh",
            new
            {
                RefreshToken = loginResult.RefreshToken
            });

        Assert.Equal(HttpStatusCode.OK, firstRefreshResponse.StatusCode);

        // Eski refresh token tekrar kullanılıyor.
        var reuseResponse = await _client.PostAsJsonAsync(
            "/api/auth/refresh",
            new
            {
                RefreshToken = loginResult.RefreshToken
            });

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            reuseResponse.StatusCode);
    }

    private sealed record LoginResponse(
        string AccessToken,
        string RefreshToken,
        string Username);

    private sealed record RefreshResponse(
        string AccessToken,
        string RefreshToken);
}

