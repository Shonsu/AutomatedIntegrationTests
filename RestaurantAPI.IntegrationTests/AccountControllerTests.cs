using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;

namespace RestaurantAPI.IntegrationTests;

public class AccountControllerTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private HttpClient _client;
    public AccountControllerTests(WebApplicationFactory<Startup> factory)
    {
        _client = factory.WithWebHostBuilder(builder => builder.ConfigureServices(services =>
        {
            ServiceDescriptor? dbContextOptions = services.SingleOrDefault(services => services.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));
            services.Remove(dbContextOptions);
            services.AddDbContext<RestaurantDbContext>(options => options.UseInMemoryDatabase("RestaurantDBIntegration"));
        })).CreateClient();
    }

    [Fact]
    public async void RegisterUser_ForValidModel_ReturnsOk()
    {
        // Given
        var registerUser = new RegisterUserDto()
        {
            Email = "plum1@plum.com",
            Password = "qwe123",
            ConfirmPassword = "qwe123"
        };
        HttpContent httpContent = registerUser.ToJsonHttpContent();

        // When
        HttpResponseMessage response = await _client.PostAsync("/api/account/register", httpContent);

        // Then
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async void RegisterUser_ForInvalidModel_ReturnsOk()
    {
        // Given
        var registerUser = new RegisterUserDto()
        {
            Password = "qwe12",
            ConfirmPassword = "qwe123"
        };
        HttpContent httpContent = registerUser.ToJsonHttpContent();

        // When
        HttpResponseMessage response = await _client.PostAsync("/api/account/register", httpContent);

        // Then
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
}