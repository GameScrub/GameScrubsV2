using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using GameScrubsV2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static GameScrubsV2.Common.Json.SerializerOptions;

namespace GameScrubsV2.IntegrationTests.Infrastructure;

[Collection("Database")]
public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected readonly HttpClient HttpClient;
    protected readonly IntegrationTestFactory Factory;

    protected IntegrationTestBase(DatabaseFixture databaseFixture)
    {
        Factory = new IntegrationTestFactory(databaseFixture);
        HttpClient = Factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await Factory.EnsureDatabaseCreatedAsync();
    }

    public virtual Task DisposeAsync()
    {
        HttpClient?.Dispose();
        Factory?.Dispose();
        return Task.CompletedTask;
    }

    protected Task<GameScrubsV2DbContext> GetDbContext()
    {
        var scope = Factory.Services.CreateScope();
        return Task.FromResult(scope.ServiceProvider.GetRequiredService<GameScrubsV2DbContext>());
    }

    protected async Task<string> CreateUserAndGetToken(string email = "test@example.com", string password = "Test123!")
    {
        using var scope = Factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        var user = new IdentityUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(user, password);

        var loginRequest = new
        {
            Email = email,
            Password = password
        };

        var loginResponse = await HttpClient.PostAsync("/api/auth/login",
            new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json"));

        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var loginResult = JsonSerializer.Deserialize<LoginResponse>(loginContent, DefaultJsonSerializerOptions);

        return loginResult?.Token ?? throw new InvalidOperationException("Failed to get token");
    }

    protected void SetAuthorizationHeader(string token)
	    => HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    protected static StringContent CreateJsonContent(object obj)
    {
        var json = JsonSerializer.Serialize(obj);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private record LoginResponse(string Token);
}