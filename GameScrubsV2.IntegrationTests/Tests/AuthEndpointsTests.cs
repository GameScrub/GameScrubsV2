using System.Net;
using System.Text.Json;
using FluentAssertions;
using GameScrubsV2.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static GameScrubsV2.Common.Json.SerializerOptions;

namespace GameScrubsV2.IntegrationTests.Tests;

public class AuthEndpointsTests : IntegrationTestBase
{
    public AuthEndpointsTests(IntegrationTestFactory factory) : base(factory) { }

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
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new { Email = "nonexistent@example.com", Password = "WrongPassword" };

        // Act
        var response = await HttpClient.PostAsync("/api/auth/login", CreateJsonContent(loginRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var registerRequest = new
        {
            Email = "newuser@example.com",
            Password = "Test123!",
            ConfirmPassword = "Test123!"
        };

        // Act
        var response = await HttpClient.PostAsync("/api/auth/register", CreateJsonContent(registerRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify user was created
        using var scope = Factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var user = await userManager.FindByEmailAsync(registerRequest.Email);
        user.Should().NotBeNull();
    }

    private record LoginResponse(string Token);
}