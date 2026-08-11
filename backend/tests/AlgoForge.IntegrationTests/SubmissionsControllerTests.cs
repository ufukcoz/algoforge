using AlgoForge.IntegrationTests.Infrastructure;
using System.Net;
using Xunit;

namespace AlgoForge.IntegrationTests;

public class SubmissionsControllerTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public SubmissionsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetMySubmissions_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/submissions");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateSubmission_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        var response = await _client.PostAsync(
            "/api/submissions",
            new StringContent(
                """
                {
                    "questionId": "00000000-0000-0000-0000-000000000000",
                    "language": "CSharp",
                    "sourceCode": "Console.WriteLine(\"Hello\");"
                }
                """,
                System.Text.Encoding.UTF8,
                "application/json"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}