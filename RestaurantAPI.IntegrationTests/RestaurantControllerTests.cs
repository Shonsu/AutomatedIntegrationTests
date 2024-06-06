using FluentAssertions;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
// 
namespace RestaurantAPI.IntegrationTests;

public class RestaurantControllerTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Startup> _factory;
    public RestaurantControllerTests(WebApplicationFactory<Startup> factory)
    {
        // var factory = new WebApplicationFactory<Startup>();

        _factory = factory.WithWebHostBuilder(builder => builder.ConfigureServices(services =>
        {
            ServiceDescriptor? dbContextOptions = services.SingleOrDefault(services => services.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));
            services.Remove(dbContextOptions);
            services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
            services.AddDbContext<RestaurantDbContext>(options => options.UseInMemoryDatabase("RestaurantDBIntegration"));
        }));
        _client = _factory.CreateClient();
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

    [Fact]
    public async void CreateRestaurant_WithValidModel_ReturnsCreatedStatus()
    {
        // arrange
        var model = new CreateRestaurantDto()
        {
            Name = "Some restaurant",
            City = "Warsaw",
            Street = "Długa"
        };

        var httpContent = model.ToJsonHttpContent();

        // act
        HttpResponseMessage response = await _client.PostAsync("/api/restaurant", httpContent);

        // assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async void CreateRestaurant_WithInvalidModel_ReturnsBadRequest()
    {
        // Given
        var model = new CreateRestaurantDto()
        {
            ContactEmail = "asd@spsd.pl",
            Description = "opsi",
            ContactNumber = "888-888-123"
        };

        var httpContent = model.ToJsonHttpContent();

        // When
        HttpResponseMessage response = await _client.PostAsync("/api/restaurant", httpContent);
        // Then
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async void Delete_ForNonExistingRestaurant_ReturnsNotFound()
    {
        // Given

        // When
        HttpResponseMessage response = await _client.DeleteAsync("/api/restaurant/321");
        // Then
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async void Delete_ForRestaurantOwner_ReturnsNoContent()
    {
        // Given
        var restaurant = new Restaurant()
        {
            CreatedById = 1,
            Name = "Test"
        };
        await SeedRestaurant(restaurant);

        // When
        HttpResponseMessage response = await _client.DeleteAsync("/api/restaurant/" + restaurant.Id);

        // Then
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    }

    [Fact]
    public async void Delete_ForNonRestaurantOwner_ReturnsForbidden()
    {
        // Given
        var restaurant = new Restaurant()
        {
            CreatedById = 990,
            Name = "Test"
        };
        await SeedRestaurant(restaurant);

        // When
        HttpResponseMessage response = await _client.DeleteAsync("/api/restaurant/" + restaurant.Id);

        // Then
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
    }

    private async Task SeedRestaurant(Restaurant restaurant)
    {
        IServiceScopeFactory? scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
        using IServiceScope scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<RestaurantDbContext>();
        await dbContext.Restaurants.AddAsync(restaurant);
        await dbContext.SaveChangesAsync();

    }
}
