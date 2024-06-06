using RestaurantAPI.Entities;

namespace RestaurantAPI.IntegrationTests.Abstractions;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebFactory>
{
    private readonly IServiceScope _scope;
    protected readonly RestaurantDbContext DbContext;
    protected HttpClient HttpClient { get; set; }

    protected BaseIntegrationTest(IntegrationTestWebFactory factory)
    {
        _scope = factory.Services.CreateScope();
        HttpClient = factory.CreateClient();
        DbContext = _scope.ServiceProvider.GetRequiredService<RestaurantDbContext>();
    }
}