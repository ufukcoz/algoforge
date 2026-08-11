using AlgoForge.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace AlgoForge.IntegrationTests.Ai;

public class AiControllerTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AiControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Assist_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var request = new
        {
            questionId = Guid.NewGuid(),
            code = "Console.WriteLine(\"Hello\");",
            language = "CSharp",
            action = "Explain"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/ai/assist",
            request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}