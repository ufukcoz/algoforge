using AlgoForge.IntegrationTests.Infrastructure;
using System.Net.Http.Json;
using System.Net;
using Xunit;

namespace AlgoForge.IntegrationTests.Contests;

public class ContestsControllerTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ContestsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetContests_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/contests");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetContestById_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var id = Guid.NewGuid();

        var response = await _client.GetAsync($"/api/contests/{id}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateContest_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/contests",
            new
            {
                title = "Integration Test Contest",
                description = "Test contest",
                startTime = DateTime.UtcNow.AddHours(1),
                endTime = DateTime.UtcNow.AddHours(2),
                isPublic = true,
                questions = Array.Empty<object>()
            });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task JoinContest_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var id = Guid.NewGuid();

        var response = await _client.PostAsJsonAsync(
            $"/api/contests/{id}/join",
            new
            {
                inviteCode = (string?)null
            });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetLeaderboard_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var id = Guid.NewGuid();

        var response = await _client.GetAsync(
            $"/api/contests/{id}/leaderboard");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}