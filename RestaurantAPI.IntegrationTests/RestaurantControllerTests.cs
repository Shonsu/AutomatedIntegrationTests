using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace RestaurantAPI.IntegrationTests;

public class RestaurantControllerTests
{
    [Theory]
    [InlineData("pageNumber=1&pageSize=5")]
    [InlineData("pageNumber=2&pageSize=5")]
    [InlineData("pageNumber=3&pageSize=15")]
    public async void GetAll_WithQyeryParameters_ReturnsOkResult(string queryParams)
    {
        // arrange
        var factory = new WebApplicationFactory<Startup>();
        HttpClient client = factory.CreateClient();

        // act
        HttpResponseMessage response = await client.GetAsync("/api/restaurant?" + queryParams);

        // assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("pageNumber=1&pageSize=100")]
    [InlineData("pageNumber=2&pageSize=11")]
    [InlineData("")]
    [InlineData(null)]
    public async void GetAll_WithInvalidQueryParams_ReturnsBadRequest(string queryParams)
    {
        // arrange
        var factory = new WebApplicationFactory<Startup>();
        HttpClient client = factory.CreateClient();

        // act
        HttpResponseMessage response = await client.GetAsync("/api/restaurant?" + queryParams);

        // assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
}
