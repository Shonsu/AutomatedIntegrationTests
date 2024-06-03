using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using RestaurantAPI.Controllers;

namespace RestaurantAPI.IntegrationTests;

public class StartupTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly List<Type> _controllerTypes;
    private readonly WebApplicationFactory<Startup> _factory;

    public StartupTests(WebApplicationFactory<Startup> factory)
    {
        _controllerTypes = typeof(Startup).Assembly.GetTypes().Where(c => c.IsSubclassOf(typeof(ControllerBase))).ToList();
        _factory = factory.WithWebHostBuilder(builder =>
                                        builder.ConfigureServices(services => _controllerTypes.ForEach(c => services.AddScoped(c))));
    }

    [Fact]
    public void ConfigureServices_ForControllers_RegistersAllDependencies()
    {
        // Given
        IServiceScopeFactory? scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
        using IServiceScope scope = scopeFactory.CreateScope();

        // When
        _controllerTypes.ForEach(t =>
        {
            var controller = scope.ServiceProvider.GetService<AccountController>();
            controller.Should().NotBeNull();
        });

        // Then
    }
}