using AlgoForge.IntegrationTests.Infrastructure;
using System.Net;
using Xunit;

namespace AlgoForge.IntegrationTests.Leaderboard;

public class LeaderboardControllerTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public LeaderboardControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetLeaderboard_ShouldReturnOk()
    {
        var response = await _client.GetAsync("/api/leaderboard");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetLeaderboard_ShouldAcceptTopParameter()
    {
        var response = await _client.GetAsync("/api/leaderboard?top=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}