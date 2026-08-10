using AlgoForge.IntegrationTests.Infrastructure;
using System.Net;
using System.Text.Json;
using Xunit;

namespace AlgoForge.IntegrationTests.Questions;

public class QuestionsControllerTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public QuestionsControllerTests(CustomWebApplicationFactory factory)
    {
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

        Assert.Equal(JsonValueKind.Object, document.RootElement.ValueKind);
    }
}