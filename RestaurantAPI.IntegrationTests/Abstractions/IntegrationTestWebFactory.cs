using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using Testcontainers.SqlEdge;

namespace RestaurantAPI.IntegrationTests.Abstractions;

public class IntegrationTestWebFactory : WebApplicationFactory<Startup>, IAsyncLifetime
{
    private readonly SqlEdgeContainer _dbContainer = new SqlEdgeBuilder().WithImage("mcr.microsoft.com/azure-sql-edge:latest").Build();

    public Task InitializeAsync() => _dbContainer.StartAsync();
    public new Task DisposeAsync() => _dbContainer.StopAsync();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            ServiceDescriptor? serviceDescriptor = services.SingleOrDefault(services => services.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));
            if (serviceDescriptor is not null)
            {
                services.Remove(serviceDescriptor);
            }
            services.AddDbContext<RestaurantDbContext>(options => options.UseSqlServer(_dbContainer.GetConnectionString()));
        });
    }

}