using AlgoForge.IntegrationTests.Infrastructure;
using System.Net;
using Xunit;

namespace AlgoForge.IntegrationTests.Categories;

public class CategoriesControllerTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CategoriesControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCategories_ShouldReturnOk()
    {
        var response = await _client.GetAsync("/api/categories");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}