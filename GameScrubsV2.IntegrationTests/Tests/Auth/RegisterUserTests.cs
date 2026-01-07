using System.Net;
using System.Text.Json;
using FluentAssertions;
using GameScrubsV2.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GameScrubsV2.IntegrationTests.Tests.Auth;

public class RegisterUserTests : IntegrationTestBase
{
    public RegisterUserTests(IntegrationTestFactory factory) : base(factory) { }

    [Fact]
    public async Task Register_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var registerRequest = new
        {
            Email = "newuser@example.com",
            Password = "Test123!"
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
        user!.Email.Should().Be(registerRequest.Email);
    }

    [Fact]
    public async Task Register_WithExistingEmail_ReturnsBadRequest()
    {
        // Arrange
        const string email = "existing@example.com";
        
        using var scope = Factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var existingUser = new IdentityUser { UserName = email, Email = email };
        await userManager.CreateAsync(existingUser, "Test123!");

        var registerRequest = new
        {
            Email = email,
            Password = "Test123!"
        };

        // Act
        var response = await HttpClient.PostAsync("/api/auth/register", CreateJsonContent(registerRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("", "Test123!")]
    [InlineData("test@example.com", "")]
    [InlineData("test@example.com", "weak")]
    public async Task Register_WithInvalidData_ReturnsBadRequest(string email, string password)
    {
        // Arrange
        var registerRequest = new
        {
            Email = email,
            Password = password
        };

        // Act
        var response = await HttpClient.PostAsync("/api/auth/register", CreateJsonContent(registerRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithWeakPassword_ReturnsBadRequest()
    {
        // Arrange
        var registerRequest = new
        {
            Email = "test@example.com",
            Password = "123"
        };

        // Act
        var response = await HttpClient.PostAsync("/api/auth/register", CreateJsonContent(registerRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}