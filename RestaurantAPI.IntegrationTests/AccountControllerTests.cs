using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using RestaurantAPI.Services;

namespace RestaurantAPI.IntegrationTests;

public class AccountControllerTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly HttpClient _client;
    private readonly Mock<IAccountService> accountServiceMock = new Mock<IAccountService>();
    public AccountControllerTests(WebApplicationFactory<Startup> factory)
    {
        _client = factory.WithWebHostBuilder(builder => builder.ConfigureServices(services =>
        {
            ServiceDescriptor? dbContextOptions = services.SingleOrDefault(services => services.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));
            services.Remove(dbContextOptions);
            services.AddSingleton<IAccountService>(accountServiceMock.Object);
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

    [Fact]
    public async void Login_ForRegisteredUser_ReturnsOk()
    {
        // Given
        var loginDto = new LoginDto() { Email = "email@zoz.pl", Password = "somePassword123" };
        HttpContent httpContent = loginDto.ToJsonHttpContent();
        accountServiceMock.Setup(a => a.GenerateJwt(It.IsAny<LoginDto>())).Returns("jwt");
        // When
        HttpResponseMessage response = await _client.PostAsync("/api/account/login", httpContent);
        // Then

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
}