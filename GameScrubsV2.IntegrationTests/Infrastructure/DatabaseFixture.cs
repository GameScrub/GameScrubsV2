using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;
using Xunit;

namespace GameScrubsV2.IntegrationTests.Infrastructure;

public class DatabaseFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer;
    
    public DatabaseFixture()
    {
        _dbContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("Test123!")
            .WithName($"gamecrubsv2-shared-{Guid.NewGuid():N}")
            .WithCleanUp(true)
            .Build();
    }

    public string ConnectionString => _dbContainer.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        try
        {
            await _dbContainer.DisposeAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Failed to dispose shared container: {ex.Message}");
        }
    }
}

[CollectionDefinition("Database")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
}