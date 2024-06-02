using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;

namespace RestaurantAPI.IntegrationTests;

public class RestaurantControllerTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly HttpClient _client;

    public RestaurantControllerTests(WebApplicationFactory<Startup> factory)
    {
        // var factory = new WebApplicationFactory<Startup>();
        _client = factory.WithWebHostBuilder(builder => builder.ConfigureServices(services =>
        {
            ServiceDescriptor? dbContextOptions = services.SingleOrDefault(services => services.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));
            services.Remove(dbContextOptions);
            services.AddDbContext<RestaurantDbContext>(options => options.UseInMemoryDatabase("RestaurantDBIntegration"));
        })).CreateClient();
    }

    [Theory]
    [InlineData("pageNumber=1&pageSize=5")]
    [InlineData("pageNumber=2&pageSize=5")]
    [InlineData("pageNumber=3&pageSize=15")]
    public async void GetAll_WithQyeryParameters_ReturnsOkResult(string queryParams)
    {
        // arrange

        // act
        HttpResponseMessage response = await _client.GetAsync("/api/restaurant?" + queryParams);

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

        // act
        HttpResponseMessage response = await _client.GetAsync("/api/restaurant?" + queryParams);

        // assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
}
