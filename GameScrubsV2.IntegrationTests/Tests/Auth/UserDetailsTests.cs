using System.Net;
using System.Text.Json;
using FluentAssertions;
using GameScrubsV2.IntegrationTests.Infrastructure;
using Xunit;
using static GameScrubsV2.Common.Json.SerializerOptions;

namespace GameScrubsV2.IntegrationTests.Tests.Auth;

public class UserDetailsTests : IntegrationTestBase
{
    public UserDetailsTests(IntegrationTestFactory factory) : base(factory) { }

    [Fact]
    public async Task UserDetails_WithValidToken_ReturnsUserInfo()
    {
        // Arrange
        const string email = "userdetails@example.com";
        var token = await CreateUserAndGetToken(email);
        SetAuthorizationHeader(token);

        // Act
        var response = await HttpClient.GetAsync("/api/auth/user");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var userDetails = JsonSerializer.Deserialize<UserDetailsResponse>(content, DefaultJsonSerializerOptions);
        
        userDetails.Should().NotBeNull();
        userDetails!.Email.Should().Be(email);
    }

    [Fact]
    public async Task UserDetails_WithoutToken_ReturnsUnauthorized()
    {
        // Act
        var response = await HttpClient.GetAsync("/api/auth/user");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UserDetails_WithInvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        SetAuthorizationHeader("invalid-token");

        // Act
        var response = await HttpClient.GetAsync("/api/auth/user");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UserDetails_WithExpiredToken_ReturnsUnauthorized()
    {
        // Arrange
        SetAuthorizationHeader("eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJPbmxpbmUgSldUIEJ1aWxkZXIiLCJpYXQiOjE1OTg4ODI0MDAsImV4cCI6MTU5ODg4MjQwMCwiYXVkIjoid3d3LmV4YW1wbGUuY29tIiwic3ViIjoianJvY2tldEBleGFtcGxlLmNvbSJ9.invalid");

        // Act
        var response = await HttpClient.GetAsync("/api/auth/user");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private record UserDetailsResponse(
        string Id,
        string Email,
        string? UserName
    );
}