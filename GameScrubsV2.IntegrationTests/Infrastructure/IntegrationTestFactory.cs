using GameScrubsV2.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace GameScrubsV2.IntegrationTests.Infrastructure;

public class IntegrationTestFactory : WebApplicationFactory<Program>
{
    private readonly DatabaseFixture _databaseFixture;

    public IntegrationTestFactory(DatabaseFixture databaseFixture)
    {
        _databaseFixture = databaseFixture;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });

        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<GameScrubsV2DbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<GameScrubsV2DbContext>(options =>
            {
                options.UseSqlServer(_databaseFixture.ConnectionString);
            });
        });

        builder.UseEnvironment("Testing");
    }

    public async Task EnsureDatabaseCreatedAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GameScrubsV2DbContext>();
        await context.Database.MigrateAsync();
    }
}