using AlgoForge.IntegrationTests.Infrastructure;
using System.Net;
using Xunit;

namespace AlgoForge.IntegrationTests.Profile;

public class ProfileControllerTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProfileControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetMyProfile_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        var response = await _client.GetAsync("/api/profile");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}