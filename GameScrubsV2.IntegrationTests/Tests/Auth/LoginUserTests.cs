using System.Net;
using System.Text.Json;
using FluentAssertions;
using GameScrubsV2.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static GameScrubsV2.Common.Json.SerializerOptions;

namespace GameScrubsV2.IntegrationTests.Tests.Auth;

public class LoginUserTests : IntegrationTestBase
{
    public LoginUserTests(DatabaseFixture databaseFixture) : base(databaseFixture) { }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsSuccessAndToken()
    {
        // Arrange
        const string email = "test@example.com";
        const string password = "Test123!";
        
        using var scope = Factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
        await userManager.CreateAsync(user, password);

        var loginRequest = new { Email = email, Password = password };

        // Act
        var response = await HttpClient.PostAsync("/api/auth/login", CreateJsonContent(loginRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<LoginResponse>(content, DefaultJsonSerializerOptions);
        result?.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithInvalidEmail_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new { Email = "nonexistent@example.com", Password = "Test123!" };

        // Act
        var response = await HttpClient.PostAsync("/api/auth/login", CreateJsonContent(loginRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        const string email = "test@example.com";
        const string correctPassword = "Test123!";
        
        using var scope = Factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
        await userManager.CreateAsync(user, correctPassword);

        var loginRequest = new { Email = email, Password = "WrongPassword" };

        // Act
        var response = await HttpClient.PostAsync("/api/auth/login", CreateJsonContent(loginRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData("", "Test123!")]
    [InlineData("test@example.com", "")]
    [InlineData("", "")]
    [InlineData("invalid-email", "Test123!")]
    public async Task Login_WithInvalidRequest_ReturnsBadRequest(string email, string password)
    {
        // Arrange
        var loginRequest = new { Email = email, Password = password };

        // Act
        var response = await HttpClient.PostAsync("/api/auth/login", CreateJsonContent(loginRequest));

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithLockedAccount_ReturnsBadRequest()
    {
        // Arrange
        const string email = "locked@example.com";
        const string password = "Test123!";
        
        using var scope = Factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
        await userManager.CreateAsync(user, password);
        await userManager.SetLockoutEnabledAsync(user, true);
        await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddHours(1));

        var loginRequest = new { Email = email, Password = password };

        // Act
        var response = await HttpClient.PostAsync("/api/auth/login", CreateJsonContent(loginRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private record LoginResponse(string Token);
}